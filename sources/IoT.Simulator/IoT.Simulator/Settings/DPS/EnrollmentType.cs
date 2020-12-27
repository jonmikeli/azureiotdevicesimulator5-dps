using System.Runtime.Serialization;

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
        [EnumMember(Value = "Individual")]
        Individual,

        /// <summary>
        /// Enrollment for a group of devices.
        /// </summary>
        [EnumMember(Value = "Group")]
        Group
    }
}
