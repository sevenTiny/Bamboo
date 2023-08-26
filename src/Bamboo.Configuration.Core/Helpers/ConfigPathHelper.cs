using System;
using System.IO;

namespace Bamboo.Configuration.Core.Helpers
{
    internal class ConfigPathHelper
    {
        /// <summary>
        /// config base directory
        /// </summary>
        public static string BaseConfigDir = Path.Combine(AppContext.BaseDirectory, ConfigurationConst.OUTPUT_PATH_SCHEMA);

        /// <summary>
        /// config file full path xxx.json
        /// </summary>
        public static string GetConfigFileFullPath(string configName) => $"{Path.Combine(BaseConfigDir, configName)}.json";
        public static string GetConfigFileName(string configName) => $"{configName}.json";
    }
}
