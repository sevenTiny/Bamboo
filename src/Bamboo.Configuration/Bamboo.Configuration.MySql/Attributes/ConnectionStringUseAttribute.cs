using System;
using System.Reflection;

namespace Bamboo.Configuration
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ConnectionStringUseAttribute : Attribute
    {
        public string ConnectionStringKey { get; }
        public ConnectionStringUseAttribute(string key)
        {
            ConnectionStringKey = key;
        }

        internal static string GetName(Type type)
        {
            if (type.GetCustomAttribute(typeof(ConnectionStringUseAttribute)) is ConnectionStringUseAttribute connectionStringKey)
                return connectionStringKey.ConnectionStringKey;

            return null;
        }
    }
}
