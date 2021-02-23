using System.Runtime.Serialization;

namespace IoT.Simulator.Settings.DPS
{
    /// <summary>
    /// The type of enrollment for a device in the provisioning service.
    /// </summary>
    public enum SecurityType
    {
        [EnumMember(Value ="SymmetricKey")]
        SymmetricKey,
        [EnumMember(Value = "X509SelfSigned")]
        X509SelfSigned,
        [EnumMember(Value = "X509CA")]
        X509CA,
        [EnumMember(Value = "TPM")]
        TPM
    }
}
