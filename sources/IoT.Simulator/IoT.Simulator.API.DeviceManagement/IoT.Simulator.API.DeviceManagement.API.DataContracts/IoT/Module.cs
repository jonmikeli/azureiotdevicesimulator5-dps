using System;

namespace IoT.Simulator.API.DeviceManagement.API.DataContracts.IoT
{
    public class Module
    {
        public string Id { get; set; }
        public string DeviceId { get; }
        public string GenerationId { get; }
        public string ETag { get; set; }
        public string ConnectionState { get; set; }      
        public string ManagedBy { get; set; }
        public DateTime ConnectionStateUpdatedTime { get; set; }        
        public DateTime LastActivityTime { get; }
        public int CloudToDeviceMessageCount { get; set; }
        public string AuthenticationMechanism { get; set; }
        public string AuthenticationPrimaryKey { get; set; }
        public string AuthenticationSecondaryKey { get; set; }        
    }
}
