using SevenTiny.Bantina;
using System;
using System.Threading;
using Xunit;

namespace Test.SevenTiny.Bantina
{
    public class StopwatchHelperTest
    {
        [Theory]
        [InlineData(100)]
        [InlineData(200)]
        [InlineData(500)]
        [InlineData(1000)]
        [InlineData(2000)]
        public void Caculate(int millisecondsTimeout)
        {
            var timespan = StopwatchHelper.Caculate(() =>
            {
                Thread.Sleep(millisecondsTimeout);
            });
            Assert.True(timespan > TimeSpan.FromMilliseconds(millisecondsTimeout));
        }

        [Theory]
        [InlineData(3, 100)]
        [InlineData(3, 200)]
        [InlineData(3, 300)]
        [InlineData(3, 400)]
        [InlineData(3, 500)]
        public void CaculateTimes(int times, int millisecondsTimeout)
        {
            var timespan = StopwatchHelper.Caculate(times, () =>
             {
                 Thread.Sleep(millisecondsTimeout);
             });
            Assert.True(timespan > TimeSpan.FromMilliseconds(times * millisecondsTimeout));
        }
    }
}
