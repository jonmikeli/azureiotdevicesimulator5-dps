using AutoMapper;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using A = Microsoft.Azure.Devices;
using AS = Microsoft.Azure.Devices.Shared;
using S = IoT.Simulator.API.DeviceManagement.Services.Model;

namespace IoT.Simulator.API.DeviceManagement.IoC.Configuration.AutoMapper.Profiles
{
    public class ServicesMappingProfile : Profile
    {
        public ServicesMappingProfile()
        {
            CreateMap<A.Device, S.IoT.Device>()
                .ForMember(dest => dest.AuthenticationType, opts => opts.MapFrom(src => src.Authentication.Type))
                .ForMember(dest => dest.AuthenticationPrimaryKey, opts => opts.MapFrom(src => src.Authentication.SymmetricKey != null ? src.Authentication.SymmetricKey.PrimaryKey : "NA"))
                .ForMember(dest => dest.AuthenticationSecondaryKey, opts => opts.MapFrom(src => src.Authentication.SymmetricKey != null ? src.Authentication.SymmetricKey.PrimaryKey : "NA"))
                .ReverseMap();

            CreateMap<A.Module, S.IoT.Module>()
                .ForMember(dest => dest.AuthenticationType, opts => opts.MapFrom(src => src.Authentication.Type))
                .ForMember(dest => dest.AuthenticationPrimaryKey, opts => opts.MapFrom(src => src.Authentication.SymmetricKey != null ? src.Authentication.SymmetricKey.PrimaryKey : "NA"))
                .ForMember(dest => dest.AuthenticationSecondaryKey, opts => opts.MapFrom(src => src.Authentication.SymmetricKey != null ? src.Authentication.SymmetricKey.PrimaryKey : "NA"))
                .ReverseMap();

            CreateMap<AS.Twin, S.IoT.Twins>()
                .ForMember(dest => dest.Tags, opts => opts.MapFrom(src => JsonConvert.DeserializeObject<S.IoT.Tags>(src.Tags.ToJson(Formatting.Indented))))
                .ForPath(dest => dest.Properties.Desired, opts => opts.MapFrom(src => JObject.Parse(src.Properties.Desired.ToJson(Formatting.Indented))))
                .ForPath(dest => dest.Properties.Reported, opts => opts.MapFrom(src => JObject.Parse(src.Properties.Reported.ToJson(Formatting.Indented))));

            CreateMap<S.IoT.Twins, AS.Twin>()
                .ForMember(dest => dest.Tags, opts => opts.MapFrom(src => src.Tags != null ? new AS.TwinCollection(JsonConvert.SerializeObject(src.Tags)) : null))
                .ForPath(dest => dest.Properties.Desired, opts => opts.MapFrom(src => src.Properties.Desired != null ? new AS.TwinCollection(src.Properties.Desired.ToString()) : null))
                .ForPath(dest => dest.Properties.Reported, opts => opts.MapFrom(src => src.Properties.Reported != null ? new AS.TwinCollection(src.Properties.Reported.ToString()) : null));
        }
    }
}
