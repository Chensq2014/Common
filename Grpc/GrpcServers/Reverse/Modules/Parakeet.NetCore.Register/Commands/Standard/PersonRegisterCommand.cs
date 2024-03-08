using Microsoft.Extensions.Logging;
using Parakeet.NetCore.Devices;
using Parakeet.NetCore.Dtos;
using Parakeet.NetCore.Entities;
using Parakeet.NetCore.Enums;
using Parakeet.NetCore.Equipment;
using Parakeet.NetCore.Extensions;
using Parakeet.NetCore.RabbitMQModule.Producers;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using IObjectMapper = Volo.Abp.ObjectMapping.IObjectMapper;

namespace Parakeet.NetCore.Register.Commands.Standard
{
    /// <summary>
    /// 人员注册命令
    /// </summary>
    public class PersonRegisterCommand : BaseHandlerType, IReverseCommand
    {
        public string Name => AppConstants.REGISTER_PERSON_COMMAND;
        public string Area => AppConstants.STANDARD;
        public string SupplierCode => AppConstants.REGISTER;
        public override string HandlerType => $"{Area}_{SupplierCode}_{Name}";

        private readonly IProducerContainer _producerContainer;
        private readonly ILogger<PersonRegisterCommand> _logger;

        private readonly IRepository<DeviceWorker, Guid> _deviceWorkerRepository;
        private readonly IRepository<Worker, Guid> _workerRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IObjectMapper _objectMapper;

        //允许10个线程调用，当1个线程进入就开启线程锁
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 10);

        public PersonRegisterCommand(IProducerContainer producerContainer, ILogger<PersonRegisterCommand> logger, IRepository<DeviceWorker, Guid> deviceWorkerRepository, IRepository<Worker, Guid> weviceWorkerRepository, IObjectMapper objectMapper, IUnitOfWorkManager unitOfWorkManager)
        {
            _producerContainer = producerContainer;
            _logger = logger;
            _deviceWorkerRepository = deviceWorkerRepository;
            _workerRepository = weviceWorkerRepository;
            _objectMapper = objectMapper;
            _unitOfWorkManager = unitOfWorkManager;
        }
        public async Task<ResponseWrapper<string>> ExecuteAsync(DeviceDto device, string body)
        {
            await _semaphore.WaitAsync();
            try
            {
                _logger.LogInformation($"[采集]设备[{device.FakeNo}]正在执行{Name}命令");

                var commandBody = TextJsonConvert.DeserializeObject<PersonRegisterDto>(body);
                var personId = commandBody.PersonnelId ?? Guid.NewGuid().ToString("N");
                var worker = new WorkerDto
                {
                    Id = Guid.NewGuid(),
                    //PersonnelId = personId,
                    IdCard = commandBody.IdCard,
                    Name = commandBody.Name,
                    Gender = commandBody.Gender,
                    Nation = commandBody.Nation,
                    Birthday = commandBody.Birthday,
                    Address = commandBody.Address,
                    IssuedBy = commandBody.IssuedBy,
                    TermValidityStart = commandBody.TermValidityStart,
                    TermValidityEnd = commandBody.TermValidityEnd,
                    IdPhoto = commandBody.IdPhoto,
                    Photo = commandBody.Photo,
                    InfraredPhoto = commandBody.InfraredPhoto,
                    IdPhotoUrl = commandBody.IdPhotoUrl,
                    PhotoUrl = commandBody.PhotoUrl,
                    InfraredPhotoUrl = commandBody.InfraredPhotoUrl,
                    PhoneNumber = commandBody.PhoneNumber
                };
                using var unintOfWork = _unitOfWorkManager.Begin();
                //1、判断worker是否重复
                var existWorker = await _workerRepository
                    .Where(m => m.Name == worker.Name && m.IdCard == worker.IdCard)
                    .WhereIf(worker.PhoneNumber.HasValue(), m => m.PhoneNumber == worker.PhoneNumber)
                    .FirstOrDefaultAsync();

                if (existWorker == null)
                {
                    //插入worker   
                    await _workerRepository.InsertAsync(_objectMapper.Map<WorkerDto, Worker>(worker));
                }
                else
                {
                    worker.Id = existWorker.Id;
                    //todo:更新existWorker信息

                }
                _logger.LogDebug($"[采集]设备[{device.FakeNo}]正在注册人员{worker.Name}_{worker.IdCard}_{personId}...");

                var entity = new DeviceWorkerDto
                {
                    Id = Guid.NewGuid(),
                    Device = device,
                    //FakeNo = device.FakeNo,
                    //SerialNo = device.SerialNo,
                    IcCard = commandBody.IcCard,
                    PersonnelType = commandBody.PersonnelType.ToEnum<StaffType>(),
                    RegisterType = commandBody.RegisterType.ToEnum<RegType>(),
                    PersonId = personId,
                    RecordTime = DateTime.Now,
                    WorkerNo = commandBody.WorkerNo,
                    PhoneNumber = commandBody.PhoneNumber,
                    GroupLeader = commandBody.GroupLeader,
                    Marital = commandBody.Marital,
                    PoliticsType = commandBody.PoliticsType,
                    IsJoin = commandBody.IsJoin,
                    JoinTime = commandBody.JoinTime,
                    Education = commandBody.Education,
                    HasBadMedicalHistory = commandBody.HasBadMedicalHistory,
                    IsSpecial = commandBody.IsSpecial,
                    CorpCode = commandBody.CorpCode,
                    CorpName = commandBody.CorpName,
                    WorkerTypeId = commandBody.WorkerTypeId,
                    WorkerTypeCode = commandBody.WorkerTypeCode,
                    WorkerTypeName = commandBody.WorkerTypeName,
                    WorkPostId = commandBody.WorkPostId,
                    WorkPostCode = commandBody.WorkPostCode,
                    WorkPostName = commandBody.WorkPostName,
                    WorkerGroupId = commandBody.WorkerGroupId,
                    WorkerGroupCode = commandBody.WorkerGroupCode,
                    WorkerGroupName = commandBody.WorkerGroupName,
                    WorkerId = worker.Id,
                    Worker = worker
                };

                //2、判断workerDevice是否重复
                var existDeviceWorker = await _deviceWorkerRepository
                    .Where(m => m.DeviceId == entity.DeviceId && m.WorkerId == entity.WorkerId)
                    .FirstOrDefaultAsync();
                if (existDeviceWorker == null)
                {
                    //插入existDeviceWorker
                    var deviceWorker = _objectMapper.Map<DeviceWorkerDto, DeviceWorker>(entity);
                    await _deviceWorkerRepository.InsertAsync(deviceWorker);
                }
                else
                {
                    //todo:更新existDeviceWorker
                }

                await unintOfWork.CompleteAsync();

                //todo：下发人员到设备上


                //todo:是否还需要转发？ 下发到设备的就进行广播转发
                var wrapperData = new WrapperData<DeviceWorkerDto>
                {
                    Data = entity
                };
                var producer = _producerContainer.GetProducer(groupName: Area, name: AppConstants.GATE_EXCHANGE);
                await producer.PublishAsync(wrapperData, $"{AppConstants.STANDARD}.{AppConstants.GATE}.{AppConstants.REGISTER_HANDLER_TYPE}").ConfigureAwait(false);

                _logger.LogInformation($"[采集]设备[{device.FakeNo}]执行{Name}命令完毕,人员[{commandBody.Name}]-[{commandBody.IdCard}]-[{personId}]已推送");
            }
            catch (Exception e)
            {
                _logger.LogError($"设备[{device.FakeNo}]执行{Name}命令失败:{e.Message}");
                return ResponseWrapper.Error($"设备[{device.FakeNo}]执行{Name}命令失败：{e.Message}");
            }
            finally
            {
                _semaphore.Release();
            }

            return ResponseWrapper.Succeed($"设备[{device.FakeNo}]执行{Name}命令完毕");
        }
    }
}
