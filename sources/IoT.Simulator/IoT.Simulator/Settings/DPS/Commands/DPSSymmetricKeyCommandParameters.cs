using CommandLine;

using Microsoft.Azure.Devices.Client;

namespace IoT.Simulator.Settings.DPS
{
    /// <summary>
    /// Parameters for the application
    /// </summary>
    [Verb("symmetrickey", HelpText = "Define settings for DPS Symmetric key.")]
    internal class DPSSymmetricKeyCommandParameters : DPSCommandParametersBase
    {
        [Option(
           'p',
           "PrimaryKey",
            SetName = "symmetrickey",
            Required = true,
           HelpText = "The primary key of the individual or group enrollment.")]
        public string PrimaryKey { get; set; }
    }
}
