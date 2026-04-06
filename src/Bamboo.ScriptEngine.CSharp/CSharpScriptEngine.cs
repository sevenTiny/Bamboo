using Bamboo.ScriptEngine.Core;
using Bamboo.ScriptEngine.CSharp.Configs;
using Bamboo.ScriptEngine.CSharp.SandBox;
using Fasterflect;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SevenTiny.Bantina.Security;
using SevenTiny.Bantina.Validation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bamboo.ScriptEngine.CSharp
{
    public interface ICSharpScriptEngine : IScriptEngine { }

    /// <summary>
    ///  .NET Compiler Platform ("Roslyn")
    /// </summary>
    public class CSharpScriptEngine(IOptions<ScriptEngineCSharpConfig> options, ILogger<CSharpScriptEngine> logger, ReferenceManager referenceManager) : ICSharpScriptEngine
    {
        private static readonly Flags _searchedFlags = Flags.InstancePublic | Flags.StaticPublic;
        private static readonly object _lock = new();
        private static readonly IDictionary<string, Type> _scriptTypeDict = new ConcurrentDictionary<string, Type>();
        private readonly ScriptEngineCSharpConfig _scriptEngineCSharpConfig = options?.Value ?? throw new ArgumentNullException(nameof(_scriptEngineCSharpConfig), "ScriptEngineCSharpConfig is not registered at startup. Please register part of appsetting as ScriptEngineCSharpConfig configuration instance.");

        public void CheckScript(DynamicScript dynamicScript)
        {
            ArgumentsCheck(dynamicScript);
            BuildDynamicScript(dynamicScript);
        }

        public ExecutionResult<T> Execute<T>(DynamicScript dynamicScript)
        {
            ArgumentsCheck(dynamicScript);
            return RunningDynamicScript<T>(dynamicScript);
        }

        public async Task<ExecutionResult<T>> ExecuteAsync<T>(DynamicScript dynamicScript)
        {
            ArgumentsCheck(dynamicScript);
            return await RunningDynamicScriptAsync<T>(dynamicScript).ConfigureAwait(false);
        }

        #region Call function
        private ExecutionResult<T> RunningDynamicScript<T>(DynamicScript dynamicScript)
        {
            //检查编译
            var scriptHash = BuildDynamicScript(dynamicScript);

            try
            {
                //是否开启执行分析,统计非常耗时且会带来更多GC开销，正常运行过程请关闭！
                if (dynamicScript.IsExecutionInformationCollected)
                {
                    Stopwatch stopwatch = new();  //程序执行时间
                    var startMemory = GC.GetTotalMemory(true);  //方法调用内存占用
                    stopwatch.Start();

                    var result = CallFunction<T>(dynamicScript, scriptHash);

                    stopwatch.Stop();
                    result.TotalMemoryAllocated = GC.GetTotalMemory(true) - startMemory;
                    result.ProcessorTime = stopwatch.ElapsedMilliseconds;
                    return result;
                }

                return CallFunction<T>(dynamicScript, scriptHash);
            }
            catch (MissingMethodException missingMethod)
            {
                logger.LogError(missingMethod, string.Format("ClassName:{0},FunctionName:{1},Language:{2},AppName:{3},ScriptHash:{4},ParameterCount:{5},ErrorMsg: {6}", dynamicScript.ClassFullName, dynamicScript.FunctionName, "CSharp", _scriptEngineCSharpConfig.AppName, scriptHash, dynamicScript.Parameters?.Length, missingMethod.Message));
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, string.Format("Script objectId:{0},appName:{1},functionName:{2},errorMsg:{3}", null, _scriptEngineCSharpConfig.AppName, dynamicScript.FunctionName, ex.Message));
                throw;
            }
        }
        private static ExecutionResult<T> CallFunction<T>(DynamicScript dynamicScript, string scriptHash)
        {
            if (string.IsNullOrEmpty(dynamicScript.FunctionName))
                throw new ScriptEngineException($"function name can not be null.");

            if (string.IsNullOrEmpty(scriptHash) || !_scriptTypeDict.TryGetValue(scriptHash, out Type type))
                throw new ScriptEngineException($"type not found.");

            var methodInfo = type.Method(dynamicScript.FunctionName, _searchedFlags) ?? throw new ScriptEngineException($"can not found the function in the type.");

            if (!dynamicScript.IsExecutionInSandbox)
            {
                return RunContainer.ExecuteTrustedCode<T>(type, methodInfo, dynamicScript.Parameters);
            }
            else
            {
                if (dynamicScript.ExecutionInSandboxMillisecondsTimeout <= 0)
                    throw new ScriptEngineException("if execute untrusted code,please setting the milliseconds timeout!");

                return RunContainer.ExecuteUntrustedCode<T>(type, methodInfo, dynamicScript.ExecutionInSandboxMillisecondsTimeout, dynamicScript.Parameters);
            }
        }

        #endregion

        #region Call function async
        private async Task<ExecutionResult<T>> RunningDynamicScriptAsync<T>(DynamicScript dynamicScript)
        {
            //检查编译
            var scriptHash = BuildDynamicScript(dynamicScript);

            try
            {
                if (dynamicScript.IsExecutionInformationCollected)
                {
                    Stopwatch stopwatch = new();  //程序执行时间
                    var startMemory = GC.GetTotalMemory(true);  //方法调用内存占用
                    stopwatch.Start();

                    var result = await CallFunctionAsync<T>(dynamicScript, scriptHash).ConfigureAwait(false);

                    stopwatch.Stop();
                    result.TotalMemoryAllocated = GC.GetTotalMemory(true) - startMemory;
                    result.ProcessorTime = stopwatch.ElapsedMilliseconds;
                    return result;
                }

                return await CallFunctionAsync<T>(dynamicScript, scriptHash).ConfigureAwait(false);
            }
            catch (MissingMethodException missingMethod)
            {
                logger.LogError(missingMethod, string.Format("ClassName:{0},FunctionName:{1},Language:{2},AppName:{3},ScriptHash:{4},ParameterCount:{5},ErrorMsg: {6}", dynamicScript.ClassFullName, dynamicScript.FunctionName, "CSharp", _scriptEngineCSharpConfig.AppName, scriptHash, dynamicScript.Parameters?.Length, missingMethod.Message));
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, string.Format("Script objectId:{0},appName:{1},functionName:{2},errorMsg:{3}", null, _scriptEngineCSharpConfig.AppName, dynamicScript.FunctionName, ex.Message));
                throw;
            }
        }
        private static async Task<ExecutionResult<T>> CallFunctionAsync<T>(DynamicScript dynamicScript, string scriptHash)
        {
            if (string.IsNullOrEmpty(dynamicScript.FunctionName))
                throw new ScriptEngineException($"function name can not be null.");

            if (string.IsNullOrEmpty(scriptHash) || !_scriptTypeDict.TryGetValue(scriptHash, out Type type))
                throw new ScriptEngineException($"type not found.");

            var methodInfo = type.Method(dynamicScript.FunctionName, _searchedFlags) ?? throw new ScriptEngineException($"can not found the function in the type.");

            if (!dynamicScript.IsExecutionInSandbox)
            {
                return await RunContainer.ExecuteTrustedCodeAsync<T>(type, methodInfo, dynamicScript.Parameters).ConfigureAwait(false);
            }
            else
            {
                if (dynamicScript.ExecutionInSandboxMillisecondsTimeout <= 0)
                    throw new ScriptEngineException("if execute untrusted code,please setting the milliseconds timeout!");

                return await RunContainer.ExecuteUntrustedCodeAsync<T>(type, methodInfo, dynamicScript.ExecutionInSandboxMillisecondsTimeout, dynamicScript.Parameters).ConfigureAwait(false);
            }
        }

        #endregion

        #region Build and Create Assembly
        private static void ArgumentsCheck(DynamicScript dynamicScript)
        {
            Ensure.ArgumentNotNullOrWhiteSpace(dynamicScript.Script, nameof(dynamicScript.Script), "Script can not be null.");
            Ensure.ArgumentNotNullOrWhiteSpace(dynamicScript.ClassFullName, nameof(dynamicScript.ClassFullName), "ClassFullName cannot be null.");
            Ensure.ArgumentNotNullOrWhiteSpace(dynamicScript.FunctionName, nameof(dynamicScript.FunctionName), "FunctionName can not be null.");

            if (dynamicScript.Language != DynamicScriptLanguage.CSharp)
                throw new ArgumentException("dynamicScript language is not csharp, please check code or language argument.");

            if (dynamicScript.IsExecutionInSandbox && dynamicScript.ExecutionInSandboxMillisecondsTimeout <= 0)
                throw new ArgumentException("if execute untrusted code,please setting the milliseconds timeout!", "dynamicScript.MillisecondsTimeout");
        }

        private string BuildDynamicScript(DynamicScript dynamicScript)
        {
            var errorMessage = string.Empty;
            var scriptHash = GetScriptKeyHash(dynamicScript.Script);

            try
            {
                if (_scriptTypeDict.ContainsKey(scriptHash))
                    return scriptHash;

                lock (_lock)
                {
                    if (_scriptTypeDict.ContainsKey(scriptHash))
                        return scriptHash;

                    var asm = CreateAsmExecutor(dynamicScript.Script, out errorMessage);
                    if (asm != null)
                    {
                        var type = asm.GetType(dynamicScript.ClassFullName);
                        if (type == null)
                        {
                            errorMessage = $"type [{dynamicScript.ClassFullName}] not found in the assembly [{asm.FullName}].";
                            logger.LogError($"Build Script Error ! Script Info:{JsonConvert.SerializeObject(dynamicScript)}");
                            throw new ScriptEngineException(errorMessage);
                        }

                        _scriptTypeDict.Add(scriptHash, type);

                        return scriptHash;
                    }
                }

                logger.LogError($"Build Script Error ! Script Info:{JsonConvert.SerializeObject(dynamicScript)}");
                throw new ScriptEngineException(errorMessage);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "BuildDynamicScript Error");
                throw;
            }
        }
        private string GetScriptKeyHash(string script)
        {
            return string.Format(Consts.AssemblyScriptKey, DynamicScriptLanguage.CSharp, _scriptEngineCSharpConfig.AppName, MD5Helper.GetMd5Hash(script));
        }
        /// <summary>
        /// Create Assembly whick will run
        /// </summary>
        /// <param name="script"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private Assembly CreateAsmExecutor(string script, out string errorMsg)
        {
            errorMsg = null;

            var scriptHash = GetScriptKeyHash(script);

            var assemblyName = scriptHash;

            var references = referenceManager.GetMetaDataReferences()[_scriptEngineCSharpConfig.AppName];

            var compilation = CSharpCompilation.Create(assemblyName,
                [GetSyntaxTree(script)], references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOptimizationLevel(_scriptEngineCSharpConfig.IsDebugModeCompile ? OptimizationLevel.Debug : OptimizationLevel.Release));

            Assembly assembly = null;
            using (var assemblyStream = new MemoryStream())
            {
                using var pdbStream = new MemoryStream();

                var emitExecutionResult = compilation.Emit(assemblyStream, pdbStream);

                if (emitExecutionResult.Success)
                {
                    var assemblyBytes = assemblyStream.GetBuffer();
                    var pdbBytes = pdbStream.GetBuffer();
                    assembly = Assembly.Load(assemblyBytes, pdbBytes);
                    //output files
                    if (_scriptEngineCSharpConfig.IsOutPutCompileFiles)
                        OutputDynamicScriptAllFile(script, assemblyName, assemblyBytes, pdbBytes);
                }
                else
                {
                    var msgs = new StringBuilder();
                    foreach (var msg in emitExecutionResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => string.Format("[{0}]:{1}({2})", d.Id, d.GetMessage(), d.Location.GetLineSpan().StartLinePosition)))
                        msgs.AppendLine(msg);

                    if (_scriptEngineCSharpConfig.IsOutPutCompileFiles)
                        WriteDynamicScriptCs(Path.Combine(EnsureOutputPath(), assemblyName + ".cs"), script);

                    errorMsg = msgs.ToString();
                    logger.LogError(string.Format("{0}：{1}：{2}：{3}", "CSharp", _scriptEngineCSharpConfig.AppName, errorMsg, scriptHash));
                }
            }

            logger.LogInformation($"CreateAsmExecutor -> _context: CSharp, {_scriptEngineCSharpConfig.AppName},{scriptHash} _scriptTypeDict:{_scriptTypeDict?.Count} _metadataReferences:{referenceManager.GetMetaDataReferences()[_scriptEngineCSharpConfig.AppName]?.Count}");

            return assembly;
        }
        private SyntaxTree GetSyntaxTree(string script)
        {
            var scriptHash = GetScriptKeyHash(script);

            return CSharpSyntaxTree.ParseText(script, path: scriptHash + ".cs", encoding: Encoding.UTF8);
        }
        private void OutputDynamicScriptAllFile(string script, string assemblyName, byte[] assemblyBytes, byte[] pdbBytes)
        {
            string path = EnsureOutputPath();
            WriteDynamicScriptFile(Path.Combine(path, assemblyName + ".dll"), assemblyBytes);
            WriteDynamicScriptFile(Path.Combine(path, assemblyName + ".pdb"), pdbBytes);
            WriteDynamicScriptCs(Path.Combine(path, assemblyName + ".cs"), script);
        }
        private static string EnsureOutputPath()
        {
            var path = Path.Combine(AppContext.BaseDirectory, Consts.BambooScriptEngineCompileOutPut);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
        private void WriteDynamicScriptFile(string filePathName, byte[] bytes)
        {
            try
            {
                if (!File.Exists(filePathName))
                    File.WriteAllBytes(filePathName, bytes);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "WriteDynamicScriptFile Error");
            }
        }
        private void WriteDynamicScriptCs(string filePathName, string script)
        {
            try
            {
                if (!File.Exists(filePathName))
                    File.WriteAllText(filePathName, script, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "WriteDynamicScriptCs Error");
            }
        }
        #endregion
    }
}
