using CommandLine;

using Microsoft.Azure.Devices.Client;

namespace IoT.Simulator.Settings.DPS
{
    /// <summary>
    /// Parameters for the application
    /// </summary>
    [Verb("x509ca", HelpText ="Define settings for DPS CA X509.")]
    internal class DPSCAX509CommandParameters : DPSCommandParametersBase
    {
        [Option(
           'c',
           "DeviceCertificate",
            SetName = "x509ca",
           HelpText = "The device X509 certificate relative path (leaf) for DPS group enrollment. Required for X509CA security type.")]
        public string DeviceCertificatePath { get; set; }

        [Option(
           'p',
           "CertificatePassword",
            SetName = "x509ca",
           HelpText = "The device X509 certificate password. Required for X509CA security type.")]
        public string DeviceCertificatePassword { get; set; }
    }
}
