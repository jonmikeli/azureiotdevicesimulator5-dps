using IoT.Simulator.Exceptions;

using Newtonsoft.Json.Linq;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Simulator.Tools
{
    public static class ConfigurationHelpers
    {
        public static readonly string APP_SETTINGS_FILE_NAME = "appsettings.json";
        public static readonly string DEVICE_SETTINGS_FILE_NAME = "devicesettings.json";
        public static readonly string MODULE_SETTINGS_FILE_NAME = "modulessettings.json";
        public static readonly string DPS_SETTINGS_FILE_NAME = "dpssettings.json";

        public static void CheckEnvironmentConfigurationFiles()
        {

            StringBuilder sb = new StringBuilder();

            if (!File.Exists(APP_SETTINGS_FILE_NAME))
                sb.AppendLine($"{APP_SETTINGS_FILE_NAME} not found.");

            if (!File.Exists(DEVICE_SETTINGS_FILE_NAME))
                sb.AppendLine($"{DEVICE_SETTINGS_FILE_NAME} not found.");

            if (!File.Exists(MODULE_SETTINGS_FILE_NAME))
                sb.AppendLine($"{MODULE_SETTINGS_FILE_NAME} not found.");

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

            var devicesettings = $"{APP_SETTINGS_FILE_NAME}.{environment}.json";
            if (!File.Exists(devicesettings))
                sb.AppendLine($"{devicesettings} not found.");

            var modulessettings = $"{APP_SETTINGS_FILE_NAME}.{environment}.json";
            if (!File.Exists(modulessettings))
                sb.AppendLine($"{modulessettings} not found.");

            if (sb.Length > 0)
                throw new MissingEnvironmentConfigurationFileException(sb.ToString());

        }


        public static async Task WriteDeviceSettings(JToken content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            //Store the content at the application root
        }

    }
}
