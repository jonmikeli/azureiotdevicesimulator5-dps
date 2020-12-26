using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Simulator.Settings.DPS
{
    public class DPSSettings
    {
        [JsonProperty("enrollmentType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public EnrollmentType EnrollmentType { get; set; }

        [JsonProperty("individualEnrollmentSettings")]
        public IndividualEnrollment IndividualEnrollment {get;set;}

        [JsonProperty("groupEnrollmentSettings")]
        public GroupEnrollment GroupEnrollment { get; set; }        
    }

    public class IndividualEnrollment
    {

    }

    public class GroupEnrollment
    {
        [JsonProperty("securityType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SecurityType SecurityType { get; set; }

        [JsonProperty("symetricKeySettings")]
        public DPSSymmetricKeySettings SymetricKeySettings { get; set; }

        [JsonProperty("X509Settings")]
        public DPSX509Settings X509Settings { get; set; }

        [JsonProperty("tpmSettings")]
        public DPSTPMSettings TMPSettings { get; set; }
    }
}
