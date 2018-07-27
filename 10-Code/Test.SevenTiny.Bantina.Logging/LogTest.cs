using SevenTiny.Bantina.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.ConsoleApp
{
    public class LogTest
    {
        public static void Test()
        {
            ILog logger = new LogManager();
            logger.Error("");
        }
    }
}
