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

        public static void CheckConfigurationFiles()
        {

            StringBuilder sb = new StringBuilder();

            if (!File.Exists(APP_SETTINGS_FILE_NAME))
                sb.AppendLine($"{APP_SETTINGS_FILE_NAME} not found.");

            if (!File.Exists(DEVICE_SETTINGS_FILE_NAME))
                sb.AppendLine($"{DEVICE_SETTINGS_FILE_NAME} not found.");

            if (!File.Exists(MODULES_SETTINGS_FILE_NAME))
                sb.AppendLine($"{MODULES_SETTINGS_FILE_NAME} not found.");

            if (!File.Exists(DPS_SETTINGS_FILE_NAME))
                sb.AppendLine($"{DPS_SETTINGS_FILE_NAME} not found.");

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

            var dpssettings = $"{DPS_SETTINGS_FILE_NAME}.{environment}.json";
            if (!File.Exists(dpssettings))
                sb.AppendLine($"{dpssettings} not found.");

            if (sb.Length > 0)
                throw new MissingEnvironmentConfigurationFileException(sb.ToString());

        }


        public static async Task WriteDeviceSettings(DeviceSettings content, string environment)
        {
            string fileName = $"{DEVICE_SETTINGS_FILE_NAME}.json";
            if (!string.IsNullOrEmpty(environment))
                fileName = $"{DEVICE_SETTINGS_FILE_NAME}.{environment}.json";

            await WriteSettings(JsonConvert.SerializeObject(content, Formatting.Indented), fileName);
        }

        public static async Task WriteModulesSettings(ModuleSettings content, string environment)
        {
            string fileName = $"{MODULES_SETTINGS_FILE_NAME}.json";
            if (!string.IsNullOrEmpty(environment))
                fileName = $"{MODULES_SETTINGS_FILE_NAME}.{environment}.json";

            await WriteModuleSettings(JsonConvert.SerializeObject(content, Formatting.Indented), fileName);
        }

        public static async Task WriteDpsSettings(DPSSettings content, string environment)
        {
            string fileName = $"{DPS_SETTINGS_FILE_NAME}.json";
            if (!string.IsNullOrEmpty(environment))
                fileName = $"{DPS_SETTINGS_FILE_NAME}.{environment}.json";

            await WriteSettings(JsonConvert.SerializeObject(content, Formatting.Indented), fileName);
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

        private static async Task WriteModuleSettings(string content, string fileName)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            string fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);

            //TODO: implement a thread safe access
            JObject jModulesContainer = null;
            if(File.Exists(fullPath))
            {
                jModulesContainer = JObject.Parse(File.ReadAllText(fullPath));
                JArray jModulesArray = jModulesContainer["modules"] as JArray;

                if (jModulesArray != null)
                    jModulesArray.Add(JObject.Parse(content));
            }
            else
            {
                JArray jModulesArray = new JArray();
                jModulesArray.Add(JObject.Parse(content));

                jModulesContainer = new JObject();                
                jModulesContainer.Add("modules", jModulesArray);
            }

            string dataToSerialize = JsonConvert.SerializeObject(jModulesContainer, Formatting.Indented);

            await File.WriteAllTextAsync(fullPath, dataToSerialize);            
        }

    }
}
