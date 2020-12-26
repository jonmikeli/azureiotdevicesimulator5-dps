using IoT.Simulator.Extensions;
using IoT.Simulator.Settings;
using IoT.Simulator.Settings.DPS;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Threading.Tasks;

namespace IoT.Simulator.Services
{
    public class DPSProvisioningService: IProvisioningService
    {
        private readonly ILogger<DPSProvisioningService> _logger;

        public DPSProvisioningService(IOptions<DeviceSettings> deviceSettings, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory), "No logger factory has been provided.");

            string logPrefix = "system.dps.provisioning".BuildLogPrefix();
            _logger.LogDebug($"{logPrefix}::Logger created.");
            _logger.LogDebug($"{logPrefix}::DPS Provisioning service created.");
        }

        public async Task<string> ProvisionDevice(DPSSettings dpsSettings)
        {
            if (dpsSettings == null)
                throw new ArgumentNullException(nameof(dpsSettings));

            string result = string.Empty;

            //TODO: implement the logic

            return result;
        }
    }
}
