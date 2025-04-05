using Bamboo.ScriptEngine.CSharp.Configs;
using Bamboo.ScriptEngine.CSharp.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using SevenTiny.Bantina.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Bamboo.ScriptEngine.CSharp
{
    public class ReferenceManager
    {
        private readonly ILogger _logger;
        private readonly ScriptEngineCSharpConfig _scriptEngineCSharpConfig;
        private static readonly ConcurrentDictionary<string, List<MetadataReference>> _metadataReferences = new();
        /// <summary>
        /// 手动注册dll加载地址（有些配置文件不好描述的，可以根据程序运行时动态获取程序集地址）
        /// </summary>
        private static HashSet<string> _registerAssemblyLocations { get; set; } = [];
        // init complete?
        private static bool _isInitFinished = false;
        private static readonly object _locker = new();

        public ReferenceManager(IServiceProvider serviceProvider, ReferenceConfig config = null)
        {
            _logger = serviceProvider?.GetService<ILogger<ReferenceManager>>();
            _scriptEngineCSharpConfig = serviceProvider?.GetService<IOptions<ScriptEngineCSharpConfig>>()?.Value;

            if (_scriptEngineCSharpConfig == null)
            {
                throw new ArgumentNullException(nameof(_scriptEngineCSharpConfig), "ScriptEngineCSharpConfig is not registered at startup. Please register part of appsetting as ScriptEngineCSharpConfig configuration instance.");
            }

            // register dependency assembly
            if (config != null)
            {
                foreach (var assembly in config.RuntimeDependentAssemblies ?? [])
                {
                    if (assembly == null)
                        continue;

                    _registerAssemblyLocations.Add(assembly.Location);
                }
            }

            if (_isInitFinished)
                return;

            lock (_locker)
            {
                if (_isInitFinished)
                    return;

                InitMetadataReferences();

                _isInitFinished = true;
            }
        }

        public IDictionary<string, List<MetadataReference>> GetMetaDataReferences()
        {
            return _metadataReferences;
        }

        /// <summary>
        /// 初始化元数据引用
        /// </summary>
        private void InitMetadataReferences()
        {
            var loadLocations = new HashSet<string>();

            //加载系统程序集地址
            LoadSystemAssembly(loadLocations);
            //加载自定义程序集地址
            LoadCustomAssembly(loadLocations);
            //加载手动注册的程序集地址
            LoadRegisterLocations(loadLocations);
            //下载并安装程序集
            LoadPackageAssembly(loadLocations);

            //将路径不同，但是dll名称相同的，视为同一个dll，取最后一个路径的dll
            var dllNameLocationsDic = loadLocations
                .SafeToDictionary(
                    k => Path.GetFileName(k),
                    v => v,
                    SevenTiny.Bantina.RepeatActionEnum.Replace
                );

            //添加到引用
            var metaDataReferenceList = dllNameLocationsDic.Select(item => (MetadataReference)MetadataReference.CreateFromFile(item.Value)).ToList();

            _metadataReferences.TryAdd(_scriptEngineCSharpConfig.AppName, metaDataReferenceList);

            _logger.LogInformation($"bamboo script engine csharp compnent -> dll load finished! load details：{JsonConvert.SerializeObject(loadLocations)}");
        }

        //加载系统程序集
        private void LoadSystemAssembly(HashSet<string> loadLocations)
        {
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

            //这里不能把所有dll都加载上，因为有些dll是无法加载的，执行会抛出异常，只能遇到需要的再加
            loadLocations.Add(Path.Combine(assemblyPath, "System.dll"));
            loadLocations.Add(Path.Combine(assemblyPath, "mscorlib.dll"));
            loadLocations.Add(Path.Combine(assemblyPath, "netstandard.dll"));
            loadLocations.Add(Path.Combine(assemblyPath, "System.Core.dll"));
            loadLocations.Add(Path.Combine(assemblyPath, "System.Runtime.dll"));
            loadLocations.Add(Path.Combine(assemblyPath, "Microsoft.CSharp.dll"));
            loadLocations.Add(Path.Combine(assemblyPath, "System.Collections.dll"));
            loadLocations.Add(Path.Combine(assemblyPath, "System.Linq.dll"));
            loadLocations.Add(Path.Combine(assemblyPath, "System.Linq.Expressions.dll"));
            loadLocations.Add(typeof(object).Assembly.Location);
            loadLocations.Add(typeof(System.Xml.XmlReader).Assembly.Location);
            loadLocations.Add(typeof(System.Net.HttpWebRequest).Assembly.Location);
            loadLocations.Add(typeof(System.Net.Http.HttpClient).Assembly.Location);
            loadLocations.Add(typeof(Enumerable).Assembly.Location);
            loadLocations.Add(typeof(System.Runtime.Serialization.DataContractSerializer).Assembly.Location);

            //系统dll加载的目录
            var dllScanAndLoadPath = _scriptEngineCSharpConfig.GetSystemDllScanAndLoadPath();

            ParseAndLoadLocations(loadLocations, assemblyPath, dllScanAndLoadPath);
        }

        /// <summary>
        /// 加载自定义程序集
        /// </summary>
        private void LoadCustomAssembly(HashSet<string> loadLocations)
        {
            //dll加载的目录
            var dllScanAndLoadPath = _scriptEngineCSharpConfig.GetDllScanAndLoadPath();

            _logger.LogInformation($"bamboo script engine csharp compnent -> scan config dirs: [{string.Join(",", dllScanAndLoadPath)}]");

            var currentPath = AppDomain.CurrentDomain.BaseDirectory;

            ParseAndLoadLocations(loadLocations, currentPath, dllScanAndLoadPath);
        }

        private void LoadRegisterLocations(HashSet<string> loadLocations)
        {
            if (_registerAssemblyLocations == null || _registerAssemblyLocations.Count == 0)
                return;

            var currentPath = AppDomain.CurrentDomain.BaseDirectory;

            ParseAndLoadLocations(loadLocations, currentPath, _registerAssemblyLocations);
        }

        private void ParseAndLoadLocations(HashSet<string> loadLocations, string currentPath, IEnumerable<string> preLoadLocations)
        {
            foreach (var dllPath in preLoadLocations)
            {
                //如果配置的是文件，则按文件加载（绝对路径）
                if (Path.IsPathRooted(dllPath) && File.Exists(dllPath))
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
        private void LoadPackageAssembly(HashSet<string> loadLocations)
        {
            //download
            var logger = new NugetCommonLogger(_logger);

            CancellationToken cancellationToken = CancellationToken.None;

            //dll下载目录
            var downloadPath = Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, Consts.BambooScriptEnginePackageDownload)).FullName;

            ConcurrentBag<string> concurrentBag = [];

            var nugetSoursces = _scriptEngineCSharpConfig.GetNugetSources();

#if DEBUG
            foreach (var current in _scriptEngineCSharpConfig.GetInstallPackages())
#else
            //并行下载
            ScriptEngineCSharpConfigHelper.GetInstallPackages().AsParallel().ForAll(current =>
#endif
            {
                SourceCacheContext cache = new();

                var currentVersionPath = Path.Combine(downloadPath, current.PackageId.ToLower(), current.Version.ToLower());

                //先查询本地文件是否存在，如果存在，就无需再去下载
                if (Directory.Exists(currentVersionPath))
                {
                    var file = Directory.GetFiles(currentVersionPath, string.Concat(current.PackageId, ".dll"), SearchOption.AllDirectories).FirstOrDefault();
                    if (file != null)
                    {
                        concurrentBag.Add(file);
                        return;
                    }
                }

                //标记是否当前包已经成功匹配，如果成功，则不会继续在后续的源中查找
                bool isCurrentPackageMatchSuccess = false;

                foreach (var source in nugetSoursces)
                {
                    if (isCurrentPackageMatchSuccess)
                        break;

                    SourceRepository repository = Repository.Factory.GetCoreV3(source);
                    FindPackageByIdResource resource = repository.GetResourceAsync<FindPackageByIdResource>().Result;

                    //当前源没有找到这个包，换下一个源下载
                    if (!resource.DoesPackageExistAsync(current.PackageId, new NuGetVersion(current.Version), cache, logger, cancellationToken).Result)
                        continue;

                    using MemoryStream packageStream = new();

                    //复制包
                    if (!resource.CopyNupkgToStreamAsync(current.PackageId, new NuGetVersion(current.Version), packageStream, cache, logger, cancellationToken).Result)
                    {
                        _logger.LogError($"copy package of {current.PackageId} {current.Version} from source [{source}] error");
                        break;
                    }

                    using PackageArchiveReader packageReader = new(packageStream);

                    _logger.LogDebug($"copy package of {current.PackageId} {current.Version} from source [{source}] success");

                    var fs = packageReader.GetFiles();

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
#if DEBUG
            }
#else
            });
#endif
            //添加到dll引用目录
            loadLocations.AddRange(concurrentBag);
        }
    }
}
