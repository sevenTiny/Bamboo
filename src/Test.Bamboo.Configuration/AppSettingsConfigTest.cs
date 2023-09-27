using Bamboo.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Bamboo.Configuration
{
    [TestClass]
    public class AppSettingsConfigTest
    {

        [TestMethod]
        public void GetKey1()
        {
            Assert.AreEqual("123", AppSettingsConfig.GetValue<string>("Key1"));
        }

        [TestMethod]
        public void GetConnectionString()
        {
            Assert.AreNotEqual(string.Empty, AppSettingsConfig.GetConnectionString("BambooConfig"));
        }
    }
}
