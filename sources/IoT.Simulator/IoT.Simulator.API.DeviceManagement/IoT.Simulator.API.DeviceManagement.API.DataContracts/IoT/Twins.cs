using System;

namespace IoT.Simulator.API.DeviceManagement.API.DataContracts.IoT
{
    public class Twins
    {
        public Tags Tags { get; set; }

        public Properties Properties { get; set; }

        public string ETag { get; set; }

        public string DeviceId { get; set; }

        public DateTime? LastActivityTime { get; set; }

        public string Version { get; set; }

        public string Status { get; set; }

        public string StatusReason { get; set; }

        public DateTime? StatusUpdatedTime { get; set; }
    }
}
