using AutoMapper;

using DCIoT = IoT.Simulator.API.DeviceManagement.API.DataContracts.IoT;
using SIoT = IoT.Simulator.API.DeviceManagement.Services.Model.IoT;

namespace IoT.Simulator.API.DeviceManagement.IoC.Configuration.AutoMapper.Profiles
{
    public class APIMappingProfile : Profile
    {
        public APIMappingProfile()
        {
            CreateMap<SIoT.Device, DCIoT.Device>().ReverseMap();
            CreateMap<SIoT.DeviceIoTSettings, DCIoT.DeviceIoTSettings>().ReverseMap();
            CreateMap<SIoT.Location, DCIoT.Location>().ReverseMap();
            CreateMap<SIoT.Tags, DCIoT.Tags>().ReverseMap();
            CreateMap<SIoT.Twins, DCIoT.Twins>().ReverseMap();
            CreateMap<SIoT.Properties, DCIoT.Properties>().ReverseMap();
        }
    }
}
