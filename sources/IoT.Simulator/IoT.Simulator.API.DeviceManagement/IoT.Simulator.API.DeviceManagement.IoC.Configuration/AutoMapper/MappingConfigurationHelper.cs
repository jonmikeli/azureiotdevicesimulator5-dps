using AutoMapper;

using IoT.Simulator.API.DeviceManagement.IoC.Configuration.AutoMapper.Profiles;

namespace IoT.Simulator.API.DeviceManagement.IoC.Configuration.AutoMapper
{
    public static class MappingConfigurationsHelper
    {
        public static void ConfigureMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(typeof(APIMappingProfile));
                cfg.AddProfile(typeof(ServicesMappingProfile));
            });
        }
    }
}
