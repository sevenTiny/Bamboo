namespace Bamboo.Configuration
{
    internal class ConfigManager
    {
        public static T GetSection<T>(string configName)
        {
            T obj = LocalConfigurationManager.Instance.GetSection<T>(configName);

            if (obj != null)
                return obj;
            else
                return RemoteConfigurationManager.Instance.GetSection<T>(configName);
        }
    }
}
