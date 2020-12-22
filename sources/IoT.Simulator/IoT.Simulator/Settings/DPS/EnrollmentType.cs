using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Simulator.Settings.DPS
{
    /// <summary>
    /// The type of enrollment for a device in the provisioning service.
    /// </summary>
    public enum EnrollmentType
    {
        /// <summary>
        ///  Enrollment for a single device.
        /// </summary>
        Individual,

        /// <summary>
        /// Enrollment for a group of devices.
        /// </summary>
        Group,
    }
}
