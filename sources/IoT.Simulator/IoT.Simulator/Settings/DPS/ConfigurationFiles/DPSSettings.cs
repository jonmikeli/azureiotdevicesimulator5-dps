using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Simulator.Settings.DPS
{
    public class DPSSettings
    {
        [JsonProperty("individualEnrollment")]
        public IndividualEnrollment IndividualEnrollment {get;set;}
        [JsonProperty("groupEnrollment")]
        public GroupEnrollment GroupEnrollment { get; set; }

    }

    public class IndividualEnrollment
    {

    }

    public class GroupEnrollment
    {
        [JsonProperty("symetricKeySettings")]
        public DPSSymmetricKeySettings SymetricKeySettings { get; set; }
        [JsonProperty("X509Settings")]
        public DPSX509Settings X509Settings { get; set; }
        [JsonProperty("tpmSettings")]
        public DPSTPMSettings TMPSettings { get; set; }
    }
}
