using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Bamboo.ScriptEngine.CSharp.Helpers
{
    internal class NugetCommonLogger(ILogger logger) : NuGet.Common.ILogger
    {
        private LogLevel ConvertMsLogLevel(NuGet.Common.LogLevel logLevel) => logLevel switch
        {
            NuGet.Common.LogLevel.Debug => LogLevel.Debug,
            NuGet.Common.LogLevel.Verbose => LogLevel.Trace,
            NuGet.Common.LogLevel.Information => LogLevel.Information,
            NuGet.Common.LogLevel.Warning => LogLevel.Warning,
            NuGet.Common.LogLevel.Error => LogLevel.Error,
            NuGet.Common.LogLevel.Minimal => LogLevel.Information,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };

        public void Log(NuGet.Common.LogLevel level, string data)
        {
            logger.Log(ConvertMsLogLevel(level), data);
        }

        public void Log(NuGet.Common.ILogMessage message)
        {
            logger.Log(ConvertMsLogLevel(message.Level), message.Message);
        }

        public Task LogAsync(NuGet.Common.LogLevel level, string data)
        {
            logger.Log(ConvertMsLogLevel(level), data);
            return Task.CompletedTask;
        }

        public Task LogAsync(NuGet.Common.ILogMessage message)
        {
            logger.Log(ConvertMsLogLevel(message.Level), message.Message);
            return Task.CompletedTask;
        }

        public void LogDebug(string data)
        {
            logger.LogDebug(data);
        }

        public void LogError(string data)
        {
            logger.LogError(data);
        }

        public void LogInformation(string data)
        {
            logger.LogInformation(data);
        }

        public void LogInformationSummary(string data)
        {
            logger.LogInformation(data);
        }

        public void LogMinimal(string data)
        {
            logger.LogInformation(data);
        }

        public void LogVerbose(string data)
        {
            logger.LogTrace(data);
        }

        public void LogWarning(string data)
        {
            logger.LogWarning(data);
        }
    }
}
