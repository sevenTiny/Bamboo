using System;
using System.Reflection;

namespace Bamboo.Configuration
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ConfigGroupAttribute : Attribute
    {
        public string ConfigGroup { get; }
        public ConfigGroupAttribute(string group = "Default")
        {
            ConfigGroup = group;
        }

        internal static string GetGroup(Type type)
        {
            if (type.GetCustomAttribute(typeof(ConfigGroupAttribute)) is ConfigGroupAttribute configSettingUse)
                return configSettingUse.ConfigGroup;

            return null;
        }
    }
}
