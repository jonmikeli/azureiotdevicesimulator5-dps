using AutoMapper;

using IoT.Simulator.API.DeviceManagement.API.Controllers.IoT.V1;
using IoT.Simulator.API.DeviceManagement.Services.Contracts;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace IoT.Simulator.API.DeviceManagement.API.Tests.Controllers.ControllerTests
{
    [TestClass]
    public class UserControllerV1Tests : TestBase
    {
        //NOTE: should be replaced by an interface
        DevicesControllerV1 _controller;

        public UserControllerV1Tests() : base()
        {
            var serviceProvider = _services.BuildServiceProvider();
            var businessService = serviceProvider.GetRequiredService<IDeviceManagementService>();
            var mapper = serviceProvider.GetRequiredService<IMapper>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<DevicesControllerV1>();

            _controller = new DevicesControllerV1(businessService, mapper);
        }

        [TestMethod]
        public async Task GetDevices_Nominal_OK()
        {
            //Simple test
            var devices = await _controller.GetDevicesAsync();

            Assert.IsNotNull(devices);

            //TODO: complete
        }

    }
}
