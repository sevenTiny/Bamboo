using Bamboo.Configuration;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Bamboo.ScriptEngine.CSharp")]
namespace Bamboo.ScriptEngine.Configs
{
    [ConfigName("appsettings")]
    internal class AppSettingsConfig : JsonConfigBase<AppSettingsConfig>
    {
        [JsonProperty("AppName")]
        public string AppName { get; set; }
    }

    internal static class AppSettingsConfigHelper
    {
        public static string GetAppName()
        {
            return AppSettingsConfig.Instance?.AppName ?? "UnKnown";
        }
    }
}
