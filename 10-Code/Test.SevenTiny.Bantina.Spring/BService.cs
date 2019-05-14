using SevenTiny.Bantina.Spring.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.Spring
{
    public interface IBService
    {
        void Test();
    }

    public class BService : IBService
    {
        [Service]
        private ICService cService;

        public void Test()
        {
            cService.WriteLog();
        }
    }
}
