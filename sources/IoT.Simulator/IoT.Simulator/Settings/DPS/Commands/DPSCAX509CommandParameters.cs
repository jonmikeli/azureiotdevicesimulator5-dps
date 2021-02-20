using CommandLine;

using Microsoft.Azure.Devices.Client;

namespace IoT.Simulator.Settings.DPS
{
    /// <summary>
    /// Parameters for the application
    /// </summary>
    internal class DPSCAX509CommandParameters : DPSCommandParametersBase
    {
        [Option(
           'c',
           "DeviceCertificate",
           Required = true,
           HelpText = "The device X509 certificate relative path (leaf) for DPS group enrollment.")]
        public string DeviceCertificatePath { get; set; }
    }
}
