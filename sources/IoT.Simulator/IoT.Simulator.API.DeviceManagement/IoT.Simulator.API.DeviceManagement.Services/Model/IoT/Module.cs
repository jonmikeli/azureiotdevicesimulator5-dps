using System;

namespace IoT.Simulator.API.DeviceManagement.Services.Model.IoT
{
    public class Module
    {
        public string Id { get; set; }
        public string DeviceId { get; set; }
        public string GenerationId { get; set; }
        public string ETag { get; set; }
        public string ConnectionState { get; set; }      
        public string ManagedBy { get; set; }
        public DateTime ConnectionStateUpdatedTime { get; set; }        
        public DateTime LastActivityTime { get; set; }
        public int CloudToDeviceMessageCount { get; set; }
        public string AuthenticationType { get; set; }
        public string AuthenticationPrimaryKey { get; set; }
        public string AuthenticationSecondaryKey { get; set; }        
    }
}
