using System;
using System.Reflection;

namespace Bamboo.Configuration
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ConfigSettingUseAttribute : Attribute
    {
        public string ConfigSettingKey { get; }
        public ConfigSettingUseAttribute(string key = "Default")
        {
            ConfigSettingKey = key;
        }

        internal static string GetName(Type type)
        {
            if (type.GetCustomAttribute(typeof(ConfigSettingUseAttribute)) is ConfigSettingUseAttribute configSettingUse)
                return configSettingUse.ConfigSettingKey;

            return null;
        }
    }
}
