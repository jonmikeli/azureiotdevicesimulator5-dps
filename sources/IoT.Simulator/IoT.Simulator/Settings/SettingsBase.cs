using IoT.Simulator.Extensions;
using Newtonsoft.Json;

namespace IoT.Simulator.Settings
{
    public class SettingsBase
    {
        /// <summary>
        /// This property should be persisted in a secured location with monitoring and controlled access.
        /// The connection string is provided at the provisioning stage (either by the IoT Hub or the DPS, which relies in turn on IoT Hub to get the Connection String.
        /// </summary>
        private string _connectionString;
        [JsonProperty("connectionString")]
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }

            set
            {
                if (value != _connectionString)
                {
                    _connectionString = value;

                    if (!string.IsNullOrEmpty(_connectionString))
                    {
                        DeviceId = ConnectionString.ExtractValue("DeviceId");
                        HostName = ConnectionString.ExtractValue("HostName");
                    }
                }
            }
        }

        [JsonProperty("deviceId", Required = Required.Always)]
        public string DeviceId { get;set; }

        public string HostName
        {
            get;set;
        }
    }
}
