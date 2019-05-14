using SevenTiny.Bantina.Spring;
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
            var instance = SpringContext.RequestServices.GetService<IAService>();

            instance.ServiceTest();
            instance.Test();
            Assert.Equal(123, instance.GetInt(123));
            Assert.Equal(1, instance.NoArgument());
            Assert.Throws<ArgumentException>(() => { try { instance.ThrowException(); } catch (Exception ex) { throw ex.InnerException; } });
            instance.ArgumentVoid(123, "123");
            Assert.False(instance.GetBool(false));
            Assert.Equal("123", instance.GetString("123"));
            Assert.Equal(123f, instance.GetFloat(123f));
            Assert.Equal(123.123, instance.GetDouble(123.123));
            Assert.Null(instance.GetObject(null));
            Assert.NotNull(instance.GetOperateResult(123, "123"));
            Assert.NotNull(instance.GetOperateResults(new List<OperateResult>()));
            Assert.Equal(123.123m, instance.GetDecimal(123.123m));
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0), instance.GetDateTime(new DateTime(1970, 1, 1, 0, 0, 0)));
        }

        [Fact]
        public void Test2()
        {
            var instance = DynamicProxy.CreateProxyOfRealize<IAService, AService>();

            instance.ServiceTest();
        }
    }
}
