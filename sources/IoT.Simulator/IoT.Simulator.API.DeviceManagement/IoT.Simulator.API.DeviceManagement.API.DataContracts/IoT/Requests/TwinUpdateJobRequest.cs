﻿using System.ComponentModel.DataAnnotations;

namespace IoT.Simulator.API.DeviceManagement.API.DataContracts.IoT.Requests
{
    public class TwinUpdateJobRequest
    {
        [Required]
        public string WhereConstraint { get; set; }

        public Properties TwinProperties { get; set; }

        public Tags TwinTags { get; set; }
    }
}
