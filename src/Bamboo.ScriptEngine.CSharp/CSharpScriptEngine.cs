using Bamboo.ScriptEngine.Core;
using Bamboo.ScriptEngine.CSharp.Configs;
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
using System.Threading;
using System.Threading.Tasks;

namespace Bamboo.ScriptEngine.CSharp
{
    public interface ICSharpScriptEngine : IScriptEngine { }

    /// <summary>
    ///  .NET Compiler Platform ("Roslyn")
    /// </summary>
    public class CSharpScriptEngine(IOptions<ScriptEngineCSharpConfig> options, ILogger<CSharpScriptEngine> logger, ReferenceManager referenceManager) : ICSharpScriptEngine
    {
        private static readonly object _lock = new();
        private static readonly IDictionary<string, Type> _scriptTypeDict = new ConcurrentDictionary<string, Type>();
        private readonly ScriptEngineCSharpConfig _scriptEngineCSharpConfig = options?.Value ?? throw new ArgumentNullException(nameof(_scriptEngineCSharpConfig), "ScriptEngineCSharpConfig is not registered at startup. Please register part of appsetting as ScriptEngineCSharpConfig configuration instance.");

        public void CheckScript(DynamicScript dynamicScript)
        {
            ArgumentsCheck(dynamicScript);
            PreProcessing(dynamicScript);
            BuildDynamicScript(dynamicScript);
        }

        public ExecutionResult<T> Execute<T>(DynamicScript dynamicScript)
        {
            ArgumentsCheck(dynamicScript);
            PreProcessing(dynamicScript);
            return RunningDynamicScript<T>(dynamicScript);
        }

        private void ArgumentsCheck(DynamicScript dynamicScript)
        {
            dynamicScript.Script.CheckNullOrEmpty("script can not be null.");
            dynamicScript.ClassFullName.CheckNullOrEmpty("classFullName cannot be null.");
            dynamicScript.FunctionName.CheckNullOrEmpty("FunctionName can not be null.");

            if (dynamicScript.Language != DynamicScriptLanguage.CSharp)
                throw new ArgumentOutOfRangeException("dynamicScript language is not csharp, please check code or language argument.");

            if (dynamicScript.IsExecutionInSandbox && dynamicScript.ExecutionInSandboxMillisecondsTimeout <= 0)
                throw new ArgumentException("if execute untrusted code,please setting the milliseconds timeout!", "dynamicScript.MillisecondsTimeout");
        }

        private void PreProcessing(DynamicScript dynamicScript)
        {
            //生成唯一hash值
            GetScriptKeyHash(dynamicScript.Script);
        }

        private ExecutionResult<T> RunningDynamicScript<T>(DynamicScript dynamicScript)
        {
            //检查编译
            var scriptHash = BuildDynamicScript(dynamicScript);

            try
            {
                //是否开启执行分析,统计非常耗时且会带来更多GC开销，正常运行过程请关闭！
                if (dynamicScript.IsExecutionInformationCollected)
                {
                    Stopwatch stopwatch = new Stopwatch();  //程序执行时间
                    var startMemory = GC.GetTotalMemory(true);  //方法调用内存占用
                    stopwatch.Start();

                    var result = CallFunction<T>(dynamicScript);

                    stopwatch.Stop();
                    result.TotalMemoryAllocated = GC.GetTotalMemory(true) - startMemory;
                    result.ProcessorTime = stopwatch.ElapsedMilliseconds;
                    return result;
                }

                return CallFunction<T>(dynamicScript);
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

        #region Build and Create Assembly
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
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithOptimizationLevel(_scriptEngineCSharpConfig.IsDebugModeCompile ? OptimizationLevel.Debug : OptimizationLevel.Release));

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
        private string EnsureOutputPath()
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

        #region Call script function
        private ExecutionResult<T> CallFunction<T>(DynamicScript dynamicScript)
        {
            if (dynamicScript.FunctionName.IsNullOrEmpty())
                throw new ScriptEngineException($"function name can not be null.");

            var scriptHash = GetScriptKeyHash(dynamicScript.Script);

            if (scriptHash.IsNullOrEmpty() || !_scriptTypeDict.TryGetValue(scriptHash, out Type type))
                throw new ScriptEngineException($"type not found.");

            var methodInfo = type.Method(dynamicScript.FunctionName);

            if (methodInfo == null)
                throw new ScriptEngineException($"function name can not be null.");

            if (!dynamicScript.IsExecutionInSandbox)
            {
                return ExecuteTrustedCode<T>(type, methodInfo, dynamicScript.Parameters);
            }
            else
            {
                if (dynamicScript.ExecutionInSandboxMillisecondsTimeout <= 0)
                    throw new ScriptEngineException("if execute untrusted code,please setting the milliseconds timeout!");

                return ExecuteUntrustedCode<T>(type, methodInfo, dynamicScript.ExecutionInSandboxMillisecondsTimeout, dynamicScript.Parameters);
            }
        }
        private ExecutionResult<T> ExecuteTrustedCode<T>(Type type, MethodInfo methodInfo, params object[] parameters)
        {
            object result = null;
            var parms = methodInfo.GetParameters();
            var safeParameters = SafeTypeConvertParameters(methodInfo.Name, parms, parameters);

            if (methodInfo.IsStatic)
                result = type.TryCallMethod(methodInfo.Name, true, parms.Select(t => t.Name).ToArray(), parms.Select(t => t.ParameterType).ToArray(), safeParameters);
            else
                result = Activator.CreateInstance(type).TryCallMethod(methodInfo.Name, true, parms.Select(t => t.Name).ToArray(), parms.Select(t => t.ParameterType).ToArray(), safeParameters);

            return ExecutionResult<T>.Success((T)result);
        }

        private ExecutionResult<T> ExecuteUntrustedCode<T>(Type type, MethodInfo methodInfo, int millisecondsTimeout, params object[] parameters)
        {
            string errorMessage = string.Format("[Assembly:{0},Method:{1},Timeout:{2}, execution timed out.", type.Assembly.FullName, methodInfo.Name, millisecondsTimeout);
            ExecutionResult<T> result = new ExecutionResult<T>();

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var t = Task.Factory.StartNew(() =>
            {
                result = ExecuteTrustedCode<T>(type, methodInfo, parameters);
            }, token);

            if (!t.Wait(millisecondsTimeout, token))
            {
                tokenSource.Cancel();

                logger.LogError(errorMessage);

                throw new ScriptEngineException(errorMessage);
            }

            return result;

            //这里用不同的应用程序域重构，增强沙箱支持
            //Note:.NET Core 3.0 Preview 5 start support
            //暂时不支持沙箱环境
            //if (SettingsConfig.Instance.SandboxEnable)
            //{
            //    object obj = null;
            //    var sandBoxer = new SandBoxer();
            //    obj = sandBoxer.ExecuteUntrustedCode(type, functionName, 0, parameters);
            //    sandBoxer.UnloadSandBoxer();
            //    return (T)obj;
            //}
        }

        private object[] SafeTypeConvertParameters(string method, ParameterInfo[] parameterInfos, object[] parameters)
        {
            if (!parameterInfos.Any())
                return null;

            Ensure.ArgumentNotNullOrEmpty(parameters, nameof(parameters));

            if (parameterInfos.Length != parameters.Length)
                throw new ArgumentException(nameof(parameters), $"The number of parameters of {method} a does not match.");

            object[] result = new object[parameters.Length];

            for (int i = 0; i < parameterInfos.Length; i++)
            {
                //这里如果有参数没有实现IConvert接口，则会抛出异常
                if (typeof(IConvertible).IsAssignableFrom(parameterInfos[i].ParameterType))
                    result[i] = Convert.ChangeType(parameters[i], parameterInfos[i].ParameterType);
                else
                    result[i] = parameters[i];
            }

            return result;
        }
        #endregion
    }
}
