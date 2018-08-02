using SevenTiny.Bantina.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Test.SevenTiny.Bantina.Logging
{
    public class LogTest
    {
        [Fact]
        public void Test()
        {
            ILog logger = new LogManager();
            logger.Error("this is a test");
        }
    }
}
