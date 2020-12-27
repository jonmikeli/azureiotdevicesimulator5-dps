using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IoT.Simulator.Settings.DPS
{
    public class DPSSettings
    {

        public const string DPSSettingsSection = "dpsSettings";

        [JsonProperty("enrollmentType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public EnrollmentType EnrollmentType { get; set; }

        //[JsonProperty("individualEnrollmentSettings")]
        //public IndividualEnrollmentSettings IndividualEnrollment {get;set;}

        [JsonProperty("groupEnrollmentSettings")]
        public GroupEnrollmentSettings GroupEnrollment { get; set; }
    }

    public class IndividualEnrollmentSettings
    {

    }

    public class GroupEnrollmentSettings
    {
        [JsonProperty("securityType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SecurityType SecurityType { get; set; }

        [JsonProperty("symetricKeySettings")]
        public DPSSymmetricKeySettings SymetricKeySettings { get; set; }

        //[JsonProperty("X509Settings")]
        //public DPSX509Settings X509Settings { get; set; }

        //[JsonProperty("tpmSettings")]
        //public DPSTPMSettings TMPSettings { get; set; }
    }
}
