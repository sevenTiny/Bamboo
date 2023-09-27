namespace Bamboo.Configuration
{
    /// <summary>
    /// setting manager
    /// </summary>
    [ConfigFile("appsettings.json")]
    public class AppSettingsConfig : ConfigBase<AppSettingsConfig>
    {
        /// <summary>
        /// Shorthand for GetSection("ConnectionStrings")[name].
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetConnectionString(string name)
        {
            return ConfigurationRoot?.GetSection("ConnectionStrings")[name];
        }
    }
}
