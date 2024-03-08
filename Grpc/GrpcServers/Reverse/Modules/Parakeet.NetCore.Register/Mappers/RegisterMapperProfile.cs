using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Parakeet.NetCore.Dtos;
using Parakeet.NetCore.Entities;
using Parakeet.NetCore.Entities;
using Volo.Abp.AutoMapper;

namespace Parakeet.NetCore.Register.Mappers
{
    /// <summary>
    /// 注册模块dto映射 Profile
    /// </summary>
    public class RegisterMapperProfile : Profile
    {
        public RegisterMapperProfile()
        {
            CreateMap<Device, DeviceDto>();
            CreateMap<WorkerDto, Worker>();
            CreateMap<DeviceWorkerDto, DeviceWorker>();
            //.Ignore(m=>m.Worker);

        }
    }
}
