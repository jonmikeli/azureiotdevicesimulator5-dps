using CommandLine;

using Microsoft.Azure.Devices.Client;

namespace IoT.Simulator.Settings.DPS
{
    /// <summary>
    /// Parameters for the application
    /// </summary>
    internal class DPSSymmetricKeyCommandParameters : DPSCommandParametersBase
    {
        [Option(
           'p',
           "PrimaryKey",
            SetName = "symmetrickey",
           HelpText = "The primary key of the individual or group enrollment. Required for SymmetricKey security type.")]
        public string PrimaryKey { get; set; }
    }
}
