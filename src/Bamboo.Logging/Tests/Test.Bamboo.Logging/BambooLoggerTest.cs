using Bamboo.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using Xunit;

namespace Test.Bamboo.Logging
{
    public class BambooLoggerTest
    {
        public static ILogger Logger => new BambooLogger<BambooLoggerTest>();

        [Fact]
        public void LogError()
        {
            Logger.LogError("this is a error message");
        }

        [Fact]
        public void LogException()
        {
            Logger.LogError(new Exception("7tiny exception"), "execute code error");
        }

        [Fact]
        public void LogDebug()
        {
            Logger.LogDebug("this is a debug debugdebugdebug");
        }

        [Fact]
        public void LogCritical()
        {
            Logger.LogCritical("this is a debug LogCritical");
        }

        [Theory]
        [InlineData(100)]
        public void Performance(int count)
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < count; i++)
            {
                Logger.LogCritical("this is a critical message");
            }

            stopwatch.Stop();

            Trace.WriteLine($"ElapsedMilliseconds: {stopwatch.ElapsedMilliseconds}");
        }
    }
}
