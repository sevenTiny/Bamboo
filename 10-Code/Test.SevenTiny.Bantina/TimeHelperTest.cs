using SevenTiny.Bantina;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Test.SevenTiny.Bantina
{
    public class TimeHelperTest
    {
        [Fact]
        public void TimeStamp()
        {
            DateTime time = new DateTime(2000, 12, 30, 0, 0, 0);
            long timestamp = TimeHelper.GetTimeStamp(time);
            Assert.Equal(time, TimeHelper.GetDateTime(timestamp));
        }
    }
}
