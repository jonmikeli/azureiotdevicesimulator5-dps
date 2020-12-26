using IoT.Simulator.Exceptions;
using IoT.Simulator.Settings;
using IoT.Simulator.Settings.DPS;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Simulator.Tools
{
    public static class ConfigurationHelpers
    {
        public static readonly string APP_SETTINGS_FILE_NAME = "appsettings";
        public static readonly string DEVICE_SETTINGS_FILE_NAME = "devicesettings";
        public static readonly string MODULES_SETTINGS_FILE_NAME = "modulessettings";
        public static readonly string DPS_SETTINGS_FILE_NAME = "dpssettings";

        public static void CheckEnvironmentConfigurationFiles()
        {

            StringBuilder sb = new StringBuilder();

            if (!File.Exists(APP_SETTINGS_FILE_NAME))
                sb.AppendLine($"{APP_SETTINGS_FILE_NAME} not found.");

            if (!File.Exists(DEVICE_SETTINGS_FILE_NAME))
                sb.AppendLine($"{DEVICE_SETTINGS_FILE_NAME} not found.");

            if (!File.Exists(MODULES_SETTINGS_FILE_NAME))
                sb.AppendLine($"{MODULES_SETTINGS_FILE_NAME} not found.");

            if (sb.Length > 0)
                throw new MissingEnvironmentConfigurationFileException(sb.ToString());
        }

        public static void CheckEnvironmentConfigurationFiles(string environment)
        {
            if (string.IsNullOrEmpty(environment))
                throw new ArgumentNullException(nameof(environment));

            StringBuilder sb = new StringBuilder();

            var appsettings = $"{APP_SETTINGS_FILE_NAME}.{environment}.json";
            if (!File.Exists(appsettings))
                sb.AppendLine($"{appsettings} not found.");

            var devicesettings = $"{DEVICE_SETTINGS_FILE_NAME}.{environment}.json";
            if (!File.Exists(devicesettings))
                sb.AppendLine($"{devicesettings} not found.");

            var modulessettings = $"{MODULES_SETTINGS_FILE_NAME}.{environment}.json";
            if (!File.Exists(modulessettings))
                sb.AppendLine($"{modulessettings} not found.");

            if (sb.Length > 0)
                throw new MissingEnvironmentConfigurationFileException(sb.ToString());

        }


        public static async Task WriteDeviceSettings(DeviceSettings content)
        {
            await WriteSettings(JsonConvert.SerializeObject(content, Formatting.Indented), DEVICE_SETTINGS_FILE_NAME);
        }

        public static async Task WriteModulesSettings(ModuleSettings content)
        {
            await WriteSettings(JsonConvert.SerializeObject(content, Formatting.Indented), MODULES_SETTINGS_FILE_NAME);
        }

        public static async Task WriteDpsSettings(DPSSettings content)
        {
            await WriteSettings(JsonConvert.SerializeObject(content, Formatting.Indented), DPS_SETTINGS_FILE_NAME);
        }


        private static async Task WriteSettings(string content, string fileName)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            await File.WriteAllTextAsync(
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName),
                content);            
        }

    }
}
