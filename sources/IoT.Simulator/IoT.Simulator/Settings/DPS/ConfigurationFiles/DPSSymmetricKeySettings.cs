using Microsoft.Azure.Devices.Client;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
        public EnrollmentType EnrollmentType { get; set; }

        [JsonProperty("globalDeviceEndpoint")]
        ///"global.azure-devices-provisioning.net"          
        public string GlobalDeviceEndpoint { get; set; }

        [JsonProperty("transportType")]
        [JsonConverter(typeof(StringEnumConverter))]
        ///Default = TransportType.Mqtt
        public TransportType TransportType { get; set; }
    }
}
