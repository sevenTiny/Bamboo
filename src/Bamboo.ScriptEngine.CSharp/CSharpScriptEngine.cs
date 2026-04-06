using Bamboo.ScriptEngine.Core;
using Bamboo.ScriptEngine.CSharp.Configs;
using Bamboo.ScriptEngine.CSharp.SandBox;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SevenTiny.Bantina.Validation;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Bamboo.ScriptEngine.CSharp
{
    public interface ICSharpScriptEngine : IScriptEngine { }

    /// <summary>
    ///  .NET Compiler Platform ("Roslyn")
    /// </summary>
    public class CSharpScriptEngine : ICSharpScriptEngine
    {
        private readonly ScriptEngineCSharpConfig _scriptEngineCSharpConfig;
        private readonly ILogger<CSharpScriptEngine> _logger;
        private CSharpScriptBuilder _builder;

        public CSharpScriptEngine(IOptions<ScriptEngineCSharpConfig> options, ILogger<CSharpScriptEngine> logger, ReferenceManager referenceManager)
        {
            _logger = logger;
            _scriptEngineCSharpConfig = options?.Value ?? throw new ArgumentNullException(nameof(_scriptEngineCSharpConfig), "ScriptEngineCSharpConfig is not registered at startup. Please register part of appsetting as ScriptEngineCSharpConfig configuration instance.");
            _builder = new(logger, _scriptEngineCSharpConfig, referenceManager);
        }

        public void CheckScript(DynamicScript dynamicScript)
        {
            ArgumentsCheck(dynamicScript);
            _builder.BuildDynamicScript(dynamicScript);
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
            var scriptHash = _builder.BuildDynamicScript(dynamicScript);

            try
            {
                // 开启执行分析, 统计非常耗时且会带来更多GC开销，正常运行过程请关闭！
                if (dynamicScript.CollectExecutionStatistics)
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
                _logger.LogError(missingMethod, string.Format("ClassName:{0},FunctionName:{1},Language:{2},AppName:{3},ScriptHash:{4},ParameterCount:{5},ErrorMsg: {6}", dynamicScript.ClassFullName, dynamicScript.FunctionName, "CSharp", _scriptEngineCSharpConfig.AppName, scriptHash, dynamicScript.Parameters?.Length, missingMethod.Message));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("Script objectId:{0},appName:{1},functionName:{2},errorMsg:{3}", null, _scriptEngineCSharpConfig.AppName, dynamicScript.FunctionName, ex.Message));
                throw;
            }
        }

        private ExecutionResult<T> CallFunction<T>(DynamicScript dynamicScript, string scriptHash)
        {
            var (type, methodInfo) = CSharpScriptBuilder.GetGeneratedTypeAndMethod(scriptHash, dynamicScript.FunctionName);

            if (!dynamicScript.IsExecutionInSandbox)
            {
                return RunContainer.ExecuteTrustedCode<T>(type, methodInfo, dynamicScript.Parameters);
            }
            else
            {
                return ExecuteUnTrustedCode<T>(dynamicScript);
            }
        }

        #endregion

        #region Call function async
        private async Task<ExecutionResult<T>> RunningDynamicScriptAsync<T>(DynamicScript dynamicScript)
        {
            //检查编译
            var scriptHash = _builder.BuildDynamicScript(dynamicScript);

            try
            {
                // 开启执行分析, 统计非常耗时且会带来更多GC开销，正常运行过程请关闭！
                if (dynamicScript.CollectExecutionStatistics)
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
                _logger.LogError(missingMethod, string.Format("ClassName:{0},FunctionName:{1},Language:{2},AppName:{3},ScriptHash:{4},ParameterCount:{5},ErrorMsg: {6}", dynamicScript.ClassFullName, dynamicScript.FunctionName, "CSharp", _scriptEngineCSharpConfig.AppName, scriptHash, dynamicScript.Parameters?.Length, missingMethod.Message));
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Format("Script objectId:{0},appName:{1},functionName:{2},errorMsg:{3}", null, _scriptEngineCSharpConfig.AppName, dynamicScript.FunctionName, ex.Message));
                throw;
            }
        }
        private async Task<ExecutionResult<T>> CallFunctionAsync<T>(DynamicScript dynamicScript, string scriptHash)
        {
            var (type, methodInfo) = CSharpScriptBuilder.GetGeneratedTypeAndMethod(scriptHash, dynamicScript.FunctionName);

            if (!dynamicScript.IsExecutionInSandbox)
            {
                return await RunContainer.ExecuteTrustedCodeAsync<T>(type, methodInfo, dynamicScript.Parameters).ConfigureAwait(false);
            }
            else
            {
                return ExecuteUnTrustedCode<T>(dynamicScript);
            }
        }

        #endregion

        private static void ArgumentsCheck(DynamicScript dynamicScript)
        {
            Ensure.ArgumentNotNullOrWhiteSpace(dynamicScript.Script, nameof(dynamicScript.Script), "Script can not be null.");
            Ensure.ArgumentNotNullOrWhiteSpace(dynamicScript.ClassFullName, nameof(dynamicScript.ClassFullName), "ClassFullName cannot be null.");
            Ensure.ArgumentNotNullOrWhiteSpace(dynamicScript.FunctionName, nameof(dynamicScript.FunctionName), "FunctionName can not be null.");

            if (dynamicScript.Language != DynamicScriptLanguage.CSharp)
                throw new ArgumentException("dynamicScript language is not csharp, please check code or language argument.");

            if (dynamicScript.IsExecutionInSandbox && dynamicScript.SandboxExecutionTimeoutMilliseconds <= 0)
                throw new ArgumentException("if execute untrusted code,please setting the milliseconds timeout!", "dynamicScript.MillisecondsTimeout");
        }

        private ExecutionResult<T> ExecuteUnTrustedCode<T>(DynamicScript dynamicScript)
        {
            // emit assembly to a temporary file and run in external sandbox runner (process isolation)
            var asmFile = _builder.EmitAssemblyToTempFile(dynamicScript.Script, out var errorMsg);

            if (string.IsNullOrEmpty(asmFile))
            {
                throw new ScriptEngineException(errorMsg ?? "emit assembly to file failed");
            }

            try
            {
                return SandBoxer.ExecuteUntrustedCodeFromAssemblyFile<T>(asmFile, dynamicScript.ClassFullName, dynamicScript.FunctionName, dynamicScript.SandboxExecutionTimeoutMilliseconds, dynamicScript.Parameters);
            }
            finally
            {
                try { if (File.Exists(asmFile)) File.Delete(asmFile); } catch { }
            }
        }
    }
}
