using System;
using System.IO;

namespace Bamboo.Configuration
{
    internal class ConfigurationConst
    {
        internal const string DefaultPath = "BambooConfig";
        internal static string ConfigurationBaseFolder = Path.Combine(AppContext.BaseDirectory, DefaultPath);
    }
}
