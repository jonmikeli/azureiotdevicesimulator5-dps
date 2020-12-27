using Microsoft.Azure.Devices.Client;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using System.ComponentModel;

namespace IoT.Simulator.Settings.DPS
{
    public class DPSSymmetricKeySettings
    {
        [JsonProperty("idScope")]
        public string IdScope { get; set; }

        [JsonProperty("primaryKey")]
        public string PrimaryKey { get; set; }

        [JsonProperty("enrollmentType")]
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(EnrollmentType.Group)]
        public EnrollmentType EnrollmentType { get; set; }

        private string _globalDeviceEndpoint = "global.azure-devices-provisioning.net";
        [JsonProperty("globalDeviceEndpoint")]
        ///"global.azure-devices-provisioning.net"          
        public string GlobalDeviceEndpoint
        {
            get { return _globalDeviceEndpoint; }
            set {
                if (value != _globalDeviceEndpoint)
                    _globalDeviceEndpoint = value;
            }
        }

        [JsonProperty("transportType")]
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(TransportType.Mqtt)]
        ///Default = TransportType.Mqtt
        public TransportType TransportType { get; set; }
    }
}
