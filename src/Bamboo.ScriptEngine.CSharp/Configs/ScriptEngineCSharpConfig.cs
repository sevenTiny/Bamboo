using Bamboo.Configuration;
using Bamboo.Logging;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Bamboo.ScriptEngine.CSharp.Configs
{
    /// <summary>
    /// 该配置文件采用git作为远程配置，请参考Bamboo.Configuration组件的git配置
    /// </summary>
    [ConfigFile("ScriptEngine.CSharp.json")]
    internal class ScriptEngineCSharpConfig : ConfigBase<ScriptEngineCSharpConfig>
    {
        /// <summary>
        /// Nuget源，配置在根的源会应用到所有的应用节点
        /// </summary>
        public string[] NugetSources { get; set; }
        /// <summary>
        /// 是否DebugMode编译，默认Release模式编译
        /// </summary>
        public bool IsDebugModeCompile { get; set; }
        /// <summary>
        /// 输出编译文件
        /// </summary>
        public bool IsOutPutCompileFiles { get; set; }
        /// <summary>
        /// 系统dll目录下需要加载的dll文件名，因为不同操作系统dotnet dll文件地址不同，文件路径会根据不同系统动态查找
        /// </summary>
        public string[] SystemDllScanAndLoadPath { get; set; }
        /// <summary>
        /// dll扫描并全部加载的路径，该目录下的dll会全部加载
        /// </summary>
        public string[] DllScanAndLoadPath { get; set; }
        /// <summary>
        /// 需要下载的包
        /// </summary>
        public Package[] InstallPackages { get; set; }
    }

    internal class Package
    {
        /// <summary>
        /// 包名称
        /// </summary>
        public string PackageId { get; set; }
        /// <summary>
        /// 包版本
        /// </summary>
        public string Version { get; set; }
    }

    internal static class ScriptEngineCSharpConfigHelper
    {
        private static readonly ILogger _logger = new BambooLogger<CSharpScriptEngine>();

        private static ScriptEngineCSharpConfig GetScriptEngineCSharpConfig()
        {
            var config = ScriptEngineCSharpConfig.Get();

            if (config == null)
                _logger.LogError("Get ScriptEngine.Csharp Config Faild!");

            return config;
        }

        public static bool IsDebugModeCompile()
        {
            return GetScriptEngineCSharpConfig()?.IsDebugModeCompile ?? false;
        }

        public static bool IsIsOutPutCompileFiles()
        {
            return GetScriptEngineCSharpConfig()?.IsOutPutCompileFiles ?? false;
        }

        public static string[] GetSystemDllScanAndLoadPath()
        {
            return GetScriptEngineCSharpConfig()?.SystemDllScanAndLoadPath ?? new string[0];
        }

        /// <summary>
        /// dll扫描并全部加载的路径，该目录下的dll会全部加载
        /// </summary>
        /// <returns></returns>
        public static string[] GetDllScanAndLoadPath()
        {
            /*
              //windows默认扫描当前用户.nuget目录
                $"C:\\Users\\{Environment.UserName}\\.nuget\\packages",
                //linux默认扫描目录
                //macos默认扫描目录
             */

            return GetScriptEngineCSharpConfig()?.DllScanAndLoadPath ?? new string[0];
        }

        /// <summary>
        /// 获取安装nuget的源
        /// </summary>
        /// <returns></returns>
        public static string[] GetNugetSources()
        {
            return GetScriptEngineCSharpConfig()?.NugetSources?.Distinct().ToArray() ?? new string[0];
        }

        /// <summary>
        /// Get the packages that need to be installed
        /// </summary>
        /// <returns></returns>
        public static Package[] GetInstallPackages()
        {
            return GetScriptEngineCSharpConfig()?.InstallPackages ?? new Package[0];
        }

        internal static string GetCurrentAppName()
        {
            return AppSettingsConfig.GetValue<string>("AppName");
        }
    }
}
