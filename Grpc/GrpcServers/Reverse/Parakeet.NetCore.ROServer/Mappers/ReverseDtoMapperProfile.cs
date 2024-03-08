using AutoMapper;
using Parakeet.NetCore.Dtos;
using Parakeet.NetCore.Entities;

namespace Parakeet.NetCore.ROServer.Mappers
{
    public class ReverseDtoMapperProfile : Profile
    {
        public ReverseDtoMapperProfile()
        {
            CreateMap<Device, DeviceDto>();
        }
    }
}
