using CommandLine;

using Microsoft.Azure.Devices.Client;

namespace IoT.Simulator.Settings.DPS
{
    /// <summary>
    /// Parameters for the application
    /// </summary>
    internal class DPSSymetricKeyCommandParameters : DPSCommandParametersBase
    {
        [Option(
           'p',
           "PrimaryKey",
           Required = true,
           HelpText = "The primary key of the individual or group enrollment.")]
        public string PrimaryKey { get; set; }
    }
}
