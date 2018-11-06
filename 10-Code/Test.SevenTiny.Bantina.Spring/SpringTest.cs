using SevenTiny.Bantina.Spring.Aop;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Test.SevenTiny.Bantina.Spring
{
    public class SpringTest
    {
        public SpringTest()
        {
            new StartUp().Start();
        }

        [Fact]
        public void Test()
        {
            var instance = AService.Instance;

            instance.ServiceTest();
        }

        [Fact]
        public void Test2()
        {
            var instance = DynamicProxy.CreateProxyOfRealize<IAService, AService>();

            instance.ServiceTest();
        }
    }
}
