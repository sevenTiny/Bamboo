using Newtonsoft.Json;
using SevenTiny.Bantina.Configuration;

namespace SevenTiny.Bantina.Logging.Infrastructure
{
    [ConfigName("appsettings")]
    internal class AppSettingsConfig : JsonConfigBase<AppSettingsConfig>
    {
        public static AppSettingsConfig Instance = new AppSettingsConfig();

        [JsonProperty("ConnectionStrings")]
        public ConnectionStrings ConnectionStrings { get; set; }
        [JsonProperty("SevenTinyCloud")]
        public SevenTinyCloud SevenTinyCloud { get; set; }
    }

    internal class SevenTinyCloud
    {
        [JsonProperty]
        public string AppName { get; set; }
    }

    internal class ConnectionStrings
    {
        [JsonProperty]
        public string SevenTinyConfig { get; set; }
    }

    internal static class AppSettingsConfigHelper
    {
        public static string GetAppName()
        {
            return AppSettingsConfig.Instance?.Config?.SevenTinyCloud?.AppName ?? "SevenTinyCloud";
        }
    }
}
