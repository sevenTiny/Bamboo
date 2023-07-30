using Microsoft.VisualStudio.TestTools.UnitTesting;
using SevenTiny.Bantina.Spring;
using SevenTiny.Bantina.Spring.Aop;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.SevenTiny.Bantina.Spring
{
    [TestClass]
    public class SpringTest
    {
        public SpringTest()
        {
            new StartUp().Start();
        }

        [TestMethod]
        public void Test()
        {
            var instance = SpringContext.RequestServices.GetService<IAService>();

            instance.ServiceTest();
            instance.Test();
            Assert.AreEqual(123, instance.GetInt(123));
            Assert.AreEqual(1, instance.NoArgument());
            //Assert.Throws<ArgumentException>(() => { try { instance.ThrowException(); } catch (Exception ex) { throw ex.InnerException; } });
            instance.ArgumentVoid(123, "123");
            Assert.IsFalse(instance.GetBool(false));
            Assert.AreEqual("123", instance.GetString("123"));
            Assert.AreEqual(123f, instance.GetFloat(123f));
            Assert.AreEqual(123.123, instance.GetDouble(123.123));
            Assert.IsNull(instance.GetObject(null));
            Assert.IsNotNull(instance.GetOperateResult(123, "123"));
            Assert.IsNotNull(instance.GetOperateResults(new List<OperateResult>()));
            Assert.AreEqual(123.123m, instance.GetDecimal(123.123m));
            Assert.AreEqual(new DateTime(1970, 1, 1, 0, 0, 0), instance.GetDateTime(new DateTime(1970, 1, 1, 0, 0, 0)));
        }

        [TestMethod]
        public void Test2()
        {
            var instance = DynamicProxy.CreateProxyOfRealize<IAService, AService>();

            instance.ServiceTest();
        }
    }
}
