using AutoMapper;
using Parakeet.NetCore.Dtos;
using Parakeet.NetCore.Entities;
using Parakeet.NetCore.Dtos;

namespace Parakeet.NetCore.Shunt.Profiles
{
    public class ShuntProfile : Profile
    {
        public ShuntProfile()
        {
            CreateMap<Device, DeviceDto>().ReverseMap();
            CreateMap<DeviceExtend, DeviceExtendDto>().ReverseMap();
            CreateMap<DeviceMediator, DeviceMediatorDto>().ReverseMap();
            CreateMap<Project, ProjectDto>().ReverseMap();
            CreateMap<Supplier, SupplierDto>().ReverseMap();
        }
    }
}
