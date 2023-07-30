using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Bamboo.ScriptEngine.Configs;
using Bamboo.ScriptEngine.CSharp.Configs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bamboo.Logging;
using Newtonsoft.Json;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using NuGet.Protocol;
using System.Threading;
using NuGet.Common;
using NuGet.Packaging;

namespace Bamboo.ScriptEngine.CSharp
{
    internal static class ReferenceManager
    {
        private static readonly Microsoft.Extensions.Logging.ILogger _logger = new BambooLogger<CSharpScriptEngine>();

        private static string _currentAppName => AppSettingsConfigHelper.GetAppName();

        private static ConcurrentDictionary<string, List<MetadataReference>> _metadataReferences = new ConcurrentDictionary<string, List<MetadataReference>>();

        public static IDictionary<string, List<MetadataReference>> GetMetaDataReferences() => _metadataReferences;

        /// <summary>
        /// 初始化元数据引用
        /// </summary>
        public static void InitMetadataReferences()
        {
            var loadLocations = new HashSet<string>();

            //加载系统程序集地址
            LoadSystemAssembly(loadLocations);
            //加载自定义程序集地址
            LoadCustomAssembly(loadLocations);
            //下载并安装程序集
            LoadPackageAssembly(loadLocations);

            var metaDataReferenceList = new List<MetadataReference>(loadLocations.Count);

            //添加到引用
            foreach (var item in loadLocations)
            {
                metaDataReferenceList.Add(MetadataReference.CreateFromFile(item));
            }

            _metadataReferences.TryAdd(_currentAppName, metaDataReferenceList);

            _logger.LogInformation($"bamboo script engine csharp compnent -> dll load finished! load details：{JsonConvert.SerializeObject(loadLocations)}");
        }

        //加载系统程序集
        private static void LoadSystemAssembly(HashSet<string> loadLocations)
        {
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

            loadLocations.Add(Path.Combine(assemblyPath, "System.dll"));
            loadLocations.Add(Path.Combine(assemblyPath, "mscorlib.dll"));
            loadLocations.Add(Path.Combine(assemblyPath, "netstandard.dll"));
            loadLocations.Add(Path.Combine(assemblyPath, "System.Core.dll"));
            loadLocations.Add(Path.Combine(assemblyPath, "System.Runtime.dll"));
            loadLocations.Add(Path.Combine(assemblyPath, "Microsoft.CSharp.dll"));
            loadLocations.Add(Path.Combine(assemblyPath, "System.Collections.dll"));
            loadLocations.Add(Path.Combine(assemblyPath, "System.Linq.dll"));
            loadLocations.Add(typeof(object).Assembly.Location);
            loadLocations.Add(typeof(System.Xml.XmlReader).Assembly.Location);
            loadLocations.Add(typeof(System.Net.HttpWebRequest).Assembly.Location);
            loadLocations.Add(typeof(System.Net.Http.HttpClient).Assembly.Location);
            loadLocations.Add(typeof(Enumerable).Assembly.Location);
            loadLocations.Add(typeof(System.Runtime.Serialization.DataContractSerializer).Assembly.Location);
        }

        /// <summary>
        /// 加载自定义程序集
        /// </summary>
        private static void LoadCustomAssembly(HashSet<string> loadLocations)
        {
            //dll加载的目录
            var dllScanAndLoadPath = ScriptEngineCSharpConfigHelper.GetDllScanAndLoadPath();

            _logger.LogInformation($"bamboo script engine csharp compnent -> scan config dirs: [{string.Join(",", dllScanAndLoadPath)}]");

            var currentPath = AppDomain.CurrentDomain.BaseDirectory;

            foreach (var dllPath in dllScanAndLoadPath)
            {
                //如果配置的是文件，则按文件加载（绝对路径）
                if (File.Exists(dllPath))
                {
                    loadLocations.Add(dllPath);
                }
                //如果配置的是文件，则按文件加载（相对路径）
                else if (File.Exists(Path.Combine(currentPath, dllPath)))
                {
                    loadLocations.Add(Path.Combine(currentPath, dllPath));
                }
                //如果配置的是文件夹，那么将目录下的所有dll都加载
                else if (Directory.Exists(dllPath))
                {
                    //拿到目录下全部dll
                    var dlls = Directory.GetFiles(dllPath, "*.dll", SearchOption.AllDirectories) ?? new string[0];

                    foreach (var item in dlls)
                    {
                        loadLocations.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// 加载安装包程序集
        /// </summary>
        /// <param name="loadLocations"></param>
        private static void LoadPackageAssembly(HashSet<string> loadLocations)
        {
            //download
            var logger = NullLogger.Instance;
            CancellationToken cancellationToken = CancellationToken.None;

            //dll下载目录
            var downloadPath = Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, Consts.BambooScriptEnginePackageDownload)).FullName;

            ConcurrentBag<string> concurrentBag = new ConcurrentBag<string>();

            //并行下载
            ScriptEngineCSharpConfigHelper.GetInstallPackageSourceDic().AsParallel().ForAll(current =>
            //foreach (var current in ScriptEngineCSharpConfigHelper.GetInstallPackageSourceDic())//Debug
            {
                SourceCacheContext cache = new SourceCacheContext();
                var package = current.Key;

                var currentVersionPath = Path.Combine(downloadPath, package.PackageId.ToLower(), package.Version.ToLower());

                //先查询本地文件是否存在，如果存在，就无需再去下载
                if (Directory.Exists(currentVersionPath))
                {
                    var file = Directory.GetFiles(currentVersionPath, "*.dll", SearchOption.AllDirectories).FirstOrDefault(t => Path.GetFileName(t).Equals(string.Concat(package.PackageId, ".dll")));
                    if (file != null)
                    {
                        concurrentBag.Add(file);
                        return;
                    }
                }

                //标记是否当前包已经成功匹配，如果成功，则不会继续在后续的源中查找
                bool isCurrentPackageMatchSuccess = false;
                foreach (var source in current.Value)
                {
                    if (isCurrentPackageMatchSuccess)
                        break;

                    SourceRepository repository = Repository.Factory.GetCoreV3(source);
                    FindPackageByIdResource resource = repository.GetResourceAsync<FindPackageByIdResource>().Result;

                    //当前源没有找到这个包，换下一个源下载
                    if (!resource.DoesPackageExistAsync(package.PackageId, new NuGetVersion(package.Version), cache, logger, cancellationToken).Result)
                        continue;

                    using (MemoryStream packageStream = new MemoryStream())
                    {
                        //复制包
                        if (!resource.CopyNupkgToStreamAsync(package.PackageId, new NuGetVersion(package.Version), packageStream, cache, logger, cancellationToken).Result)
                        {
                            _logger.LogError($"copy package of {package.PackageId} {package.Version} from source [{source}] error");
                            break;
                        }

                        using (PackageArchiveReader packageReader = new PackageArchiveReader(packageStream))
                        {
                            _logger.LogDebug($"copy package of {package.PackageId} {package.Version} from source [{source}] success");

                            var files = packageReader.GetFiles().Where(t => Path.GetExtension(t).Equals(".dll")).ToArray();

                            foreach (var file in files)
                            {
                                if (isCurrentPackageMatchSuccess)
                                    break;

                                //按顺序匹配对应版本的dll
                                foreach (var matchItem in Consts.MatchDllVersionFromNugetPackageSequnce)
                                {
                                    if (file.Contains(matchItem))
                                    {
                                        var fileFullPath = Path.Combine(currentVersionPath, file);

                                        if (!File.Exists(fileFullPath))
                                            packageReader.ExtractFile(file, fileFullPath, logger);

                                        concurrentBag.Add(fileFullPath);
                                        isCurrentPackageMatchSuccess = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                //}//DEBUG
            });

            //添加到dll引用目录
            loadLocations.AddRange(concurrentBag);
        }
    }
}
