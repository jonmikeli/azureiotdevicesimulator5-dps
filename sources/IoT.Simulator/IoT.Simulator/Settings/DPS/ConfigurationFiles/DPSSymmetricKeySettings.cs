using Microsoft.Azure.Devices.Client;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Simulator.Settings.DPS
{
    public class DPSSymmetricKeySettings
    {
        [JsonProperty("idScope")]
        public string IdScope { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("primaryKey")]
        public string PrimaryKey { get; set; }

        [JsonProperty("enrollmentType")]
        public EnrollmentType EnrollmentType { get; set; }

        [JsonProperty("globalDeviceEndpoint")]
        ///"global.azure-devices-provisioning.net"          
        public string GlobalDeviceEndpoint { get; set; }

        [JsonProperty("transportType")]
        ///Default = TransportType.Mqtt
        public TransportType TransportType { get; set; }
    }
}
