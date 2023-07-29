using System;
using System.Reflection;

namespace Bamboo.Configuration
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ConfigConnectionStringAttribute : Attribute
    {
        public string ConnectionStringKey { get; }
        public ConfigConnectionStringAttribute(string key)
        {
            ConnectionStringKey = key;
        }

        internal static string GetName(Type type)
        {
            if (type.GetCustomAttribute(typeof(ConfigConnectionStringAttribute)) is ConfigConnectionStringAttribute connectionStringKey)
                return connectionStringKey.ConnectionStringKey;

            return null;
        }
    }
}
