using Bamboo.ScriptEngine.Core;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Bamboo.ScriptEngine.CSharp.SandBox
{
    /// <summary>
    /// Modern sandbox launcher. Executes a compiled assembly in an external runner process.
    ///
    /// Behavior:
    /// - Attempts to locate a runner DLL via environment variable `BAMBOO_SANDBOX_RUNNER_PATH` or
    ///   next to the current base directory as `Bamboo.ScriptEngine.SandboxRunner.dll`.
    /// - If runner exists, launches `dotnet <runner.dll>` and passes arguments. Captures stdout/stderr.
    /// - On timeout will kill the process and throw ScriptEngineException.
    /// - If runner is not found, falls back to in-process RunContainer (less secure, not recommended).
    ///
    /// Note: To enable reliable sandboxing you must build and deploy the runner (see project `Bamboo.ScriptEngine.SandboxRunner`).
    /// </summary>
    public static class SandBoxer
    {
        private const string RunnerEnvKey = "BAMBOO_SANDBOX_RUNNER_PATH";

        public static ExecutionResult<T> ExecuteUntrustedCodeFromAssemblyFile<T>(string assemblyFilePath, string typeFullName, string methodName, int millisecondsTimeout, object[] parameters)
        {
            var runnerPath = Environment.GetEnvironmentVariable(RunnerEnvKey);

            if (string.IsNullOrWhiteSpace(runnerPath))
            {
                var baseDir = AppContext.BaseDirectory;
                var guess = Path.Combine(baseDir, "Bamboo.ScriptEngine.SandboxRunner.dll");
                if (File.Exists(guess)) runnerPath = guess;
            }

            if (string.IsNullOrWhiteSpace(runnerPath) || !File.Exists(runnerPath))
            {
                throw new FileNotFoundException("Bamboo script engine sandbox runner not found");
            }

            var proc = CreateRunnerProcess(runnerPath, assemblyFilePath, typeFullName, methodName, parameters);

            try
            {
                proc.Start();

                var exited = proc.WaitForExit(millisecondsTimeout);
                if (!exited)
                {
                    try { proc.Kill(true); } catch { }
                    throw new ScriptEngineException($"[Assembly:{assemblyFilePath},Method:{methodName},Timeout:{millisecondsTimeout}, execution timed out.");
                }

                var stdout = proc.StandardOutput.ReadToEnd();
                var stderr = proc.StandardError.ReadToEnd();

                if (proc.ExitCode != 0)
                {
                    var msg = !string.IsNullOrEmpty(stderr) ? stderr : stdout;
                    throw new ScriptEngineException(msg);
                }

                var wrapper = JsonConvert.DeserializeObject<RunnerResult<T>>(stdout);
                return new ExecutionResult<T> { Data = wrapper.Data, Message = wrapper.Message };
            }
            finally
            {
                proc?.Dispose();
            }
        }

        private static Process CreateRunnerProcess(string runnerDllPath, string assemblyFile, string typeFullName, string methodName, object[] parameters)
        {
            var paramsJson = JsonConvert.SerializeObject(parameters ?? new object[0]);
            var paramsBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(paramsJson));

            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{runnerDllPath}\" \"{assemblyFile}\" \"{typeFullName}\" \"{methodName}\" {paramsBase64}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            return new Process { StartInfo = startInfo };
        }

        private class RunnerResult<T>
        {
            public T Data { get; set; }
            public string Message { get; set; }
        }
    }
}
