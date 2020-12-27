using System.Runtime.Serialization;

namespace IoT.Simulator.Settings.DPS
{
    /// <summary>
    /// The type of enrollment for a device in the provisioning service.
    /// </summary>
    public enum SecurityType
    {
        [EnumMember(Value ="SymetricKey")]
        SymetricKey,
        [EnumMember(Value = "X509")]
        X509,
        [EnumMember(Value = "TPM")]
        TPM
    }
}
