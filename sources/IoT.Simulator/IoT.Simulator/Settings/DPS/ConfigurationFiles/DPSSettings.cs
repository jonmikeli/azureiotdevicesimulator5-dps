using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using System.ComponentModel;

namespace IoT.Simulator.Settings.DPS
{
    public class DPSSettings
    {

        public const string DPSSettingsSection = "dpsSettings";

        [JsonProperty("enrollmentType")]
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(EnrollmentType.Group)]
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
        [DefaultValue(SecurityType.SymmetricKey)]
        public SecurityType SecurityType { get; set; }

        [JsonProperty("symmetricKeySettings")]
        public DPSSymmetricKeySettings SymmetricKeySettings { get; set; }

        [JsonProperty("CAX509Settings")]
        public DPSCAX509Settings CAX509Settings { get; set; }

        //[JsonProperty("tpmSettings")]
        //public DPSTPMSettings TMPSettings { get; set; }
    }
}
