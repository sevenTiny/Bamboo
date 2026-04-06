using Bamboo.ScriptEngine.Core;
using Bamboo.ScriptEngine.CSharp.Configs;
using Fasterflect;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SevenTiny.Bantina.Security;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Bamboo.ScriptEngine.CSharp
{
    internal class CSharpScriptBuilder(ILogger<CSharpScriptEngine> logger, ScriptEngineCSharpConfig config, ReferenceManager referenceManager)
    {
        private static readonly Flags _searchedFlags = Flags.InstancePublic | Flags.StaticPublic;
        private static readonly object _lock = new();
        private static readonly IDictionary<string, Type> _scriptTypeDict = new ConcurrentDictionary<string, Type>();

        public string BuildDynamicScript(DynamicScript dynamicScript)
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

        public static (Type, MethodInfo) GetGeneratedTypeAndMethod(string scriptHash, string functionName)
        {
            if (string.IsNullOrEmpty(functionName))
                throw new ScriptEngineException($"function name can not be null.");

            if (string.IsNullOrEmpty(scriptHash) || !_scriptTypeDict.TryGetValue(scriptHash, out Type type))
                throw new ScriptEngineException($"generated type not found.");

            var methodInfo = type.Method(functionName, _searchedFlags) ?? throw new ScriptEngineException($"can not found the function in the type.");

            return (type, methodInfo);
        }

        private string GetScriptKeyHash(string script)
        {
            return string.Format(Consts.AssemblyScriptKey, DynamicScriptLanguage.CSharp, MD5Helper.GetMd5Hash(script));
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

            var references = referenceManager.GetMetaDataReferences();

            var compilation = CSharpCompilation.Create(assemblyName,
                [GetSyntaxTree(script)], references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOptimizationLevel(config.IsDebugModeCompile ? OptimizationLevel.Debug : OptimizationLevel.Release));

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
                    if (config.IsOutPutCompileFiles)
                        OutputDynamicScriptAllFile(script, assemblyName, assemblyBytes, pdbBytes);
                }
                else
                {
                    var msgs = new StringBuilder();
                    foreach (var msg in emitExecutionResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => string.Format("[{0}]:{1}({2})", d.Id, d.GetMessage(), d.Location.GetLineSpan().StartLinePosition)))
                        msgs.AppendLine(msg);

                    if (config.IsOutPutCompileFiles)
                        WriteDynamicScriptCs(Path.Combine(EnsureOutputPath(), assemblyName + ".cs"), script);

                    errorMsg = msgs.ToString();
                    logger.LogError(string.Format("{0}：{1}：{2}：{3}", "CSharp", config.AppName, errorMsg, scriptHash));
                }
            }

            logger.LogInformation($"CreateAsmExecutor -> _context: CSharp, {config.AppName},{scriptHash} _scriptTypeDict:{_scriptTypeDict?.Count} _metadataReferences:{referenceManager.GetMetaDataReferences().Count}");

            return assembly;
        }

        private SyntaxTree GetSyntaxTree(string script)
        {
            var scriptHash = GetScriptKeyHash(script);

            return CSharpSyntaxTree.ParseText(script, path: scriptHash + ".cs", encoding: Encoding.UTF8);
        }

        public string EmitAssemblyToTempFile(string script, out string errorMsg)
        {
            errorMsg = null;

            var assemblyFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".dll");

            var references = referenceManager.GetMetaDataReferences();

            var compilation = CSharpCompilation.Create(Path.GetFileNameWithoutExtension(assemblyFilePath),
                new[] { GetSyntaxTree(script) }, references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithOptimizationLevel(config.IsDebugModeCompile ? OptimizationLevel.Debug : OptimizationLevel.Release));

            using var assemblyStream = new MemoryStream();
            using var pdbStream = new MemoryStream();

            var emitResult = compilation.Emit(assemblyStream, pdbStream);

            if (!emitResult.Success)
            {
                var msgs = new StringBuilder();
                foreach (var msg in emitResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(d => string.Format("[{0}]:{1}({2})", d.Id, d.GetMessage(), d.Location.GetLineSpan().StartLinePosition)))
                    msgs.AppendLine(msg);

                errorMsg = msgs.ToString();
                return null;
            }

            try
            {
                var assemblyBytes = assemblyStream.ToArray();
                File.WriteAllBytes(assemblyFilePath, assemblyBytes);
                return assemblyFilePath;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return null;
            }
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
    }
}
