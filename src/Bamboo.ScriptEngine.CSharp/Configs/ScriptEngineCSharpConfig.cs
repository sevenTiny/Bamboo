using Bamboo.Configuration;
using Bamboo.Logging;
using Bamboo.ScriptEngine.Configs;
using Microsoft.Extensions.Logging;
using SevenTiny.Bantina.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bamboo.ScriptEngine.CSharp.Configs
{
    /// <summary>
    /// 该配置文件采用git作为远程配置，请参考Bamboo.Configuration组件的git配置
    /// </summary>
    [ConfigName("ScriptEngine.CSharp")]
    internal class ScriptEngineCSharpConfig : GitConfigBase<ScriptEngineCSharpConfig>
    {
        /// <summary>
        /// 全局Nuget源，配置在根的源会应用到所有的应用节点
        /// </summary>
        public string[] NugetSources { get; set; }
        /// <summary>
        /// 配置节点
        /// </summary>
        public Node[] Nodes { get; set; }
    }

    internal class Node
    {
        /// <summary>
        /// 所属应用，系统路径用SYSTEM
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// 是否DebugMode编译，默认Release模式编译
        /// </summary>
        public bool IsDebugModeCompile { get; set; }
        /// <summary>
        /// 输出编译文件
        /// </summary>
        public bool IsOutPutCompileFiles { get; set; }
        /// <summary>
        /// dll扫描并全部加载的路径，该目录下的dll会全部加载
        /// </summary>
        public string[] DllScanAndLoadPath { get; set; }
        /// <summary>
        /// Nuget源，应用下的源仅会应用到当前应用的Nuget包，如果已经全局配置，则无需应用下单独配置
        /// </summary>
        public string[] NugetSources { get; set; }
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
        /// <summary>
        /// 默认的全局应用名
        /// </summary>
        private const string DefaultAppName = "SYSTEM";

        private static ScriptEngineCSharpConfig GetScriptEngineCSharpConfig()
        {
            if (ScriptEngineCSharpConfig.Instance == null)
            {
                _logger.LogError("Get ScriptEngine.Csharp Config Faild!");
                return null;
            }

            if (ScriptEngineCSharpConfig.Instance.Nodes == null)
            {
                _logger.LogError("Get ScriptEngine.Csharp Config Faild! 'Nodes' is null.");
                return null;
            }

            return ScriptEngineCSharpConfig.Instance;
        }

        private static Node GetNodeByAppName(string appName)
        {
            if (string.IsNullOrEmpty(appName))
                return null;

            var configInstance = GetScriptEngineCSharpConfig();

            if (configInstance == null)
                return null;

            return configInstance.Nodes.Where(t => appName.Equals(t.AppName)).FirstOrDefault();
        }

        public static bool IsDebugModeCompile()
        {
            return GetNodeByAppName(AppSettingsConfig.Instance.AppName)?.IsDebugModeCompile ?? false;
        }

        public static bool IsIsOutPutCompileFiles()
        {
            return GetNodeByAppName(AppSettingsConfig.Instance.AppName)?.IsOutPutCompileFiles ?? false;
        }

        /// <summary>
        /// dll扫描并全部加载的路径，该目录下的dll会全部加载
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDllScanAndLoadPath()
        {
            List<string> referenceDirs = new List<string>();

            /*
              //windows默认扫描当前用户.nuget目录
                $"C:\\Users\\{Environment.UserName}\\.nuget\\packages",
                //linux默认扫描目录
                //macos默认扫描目录
             */

            //先保证加载系统资源
            referenceDirs.AddRange(GetNodeByAppName(DefaultAppName)?.DllScanAndLoadPath ?? new string[0]);
            //加载应用资源
            referenceDirs.AddRange(GetNodeByAppName(AppSettingsConfig.Instance.AppName)?.DllScanAndLoadPath ?? new string[0]);

            return referenceDirs;
        }

        /// <summary>
        /// 获取安装nuget的源
        /// </summary>
        /// <returns></returns>
        private static string[] GetNugetSources(string appName)
        {
            var configInstance = GetScriptEngineCSharpConfig();

            if (configInstance == null)
                return new string[0];

            var sourcesSet = new HashSet<string>();

            //添加全局源
            foreach (var item in configInstance.NugetSources ?? new string[0])
            {
                if (!string.IsNullOrEmpty(item))
                    sourcesSet.Add(item);
            }

            //添加节点源
            foreach (var item in GetNodeByAppName(appName)?.NugetSources ?? new string[0])
            {
                if (!string.IsNullOrEmpty(item))
                    sourcesSet.Add(item);
            }

            return sourcesSet.ToArray();

        }

        /// <summary>
        /// 获取需要安装的package信息
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Package, string[]> GetInstallPackageSourceDic()
        {
            Dictionary<Package, string[]> packagesInfoDic = new Dictionary<Package, string[]>();

            //先保证加载系统资源
            foreach (var item in GetNodeByAppName(DefaultAppName)?.InstallPackages ?? new Package[0])
            {
                if (item != null)
                    packagesInfoDic.AddOrUpdate(item, GetNugetSources(DefaultAppName));
            }

            //加载应用资源
            foreach (var item in GetNodeByAppName(AppSettingsConfig.Instance.AppName)?.InstallPackages ?? new Package[0])
            {
                if (item != null)
                    packagesInfoDic.AddOrUpdate(item, GetNugetSources(AppSettingsConfig.Instance.AppName));
            }

            return packagesInfoDic;
        }
    }
}
