using Bamboo.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Bamboo.Configuration
{
    [ConfigFile("RuntimeOutPut.json")]
    public class RuntimeOutPutConfig : ConfigBase<RuntimeOutPutConfig>
    {
        public string Value1 { get; set; }
        public string Value2 { get; set; }
    }

    [ConfigFile(@"Sub\RuntimeOutPutWithFolder.json")]
    public class RuntimeOutPutWithFolderConfig : ConfigBase<RuntimeOutPutWithFolderConfig>
    {
        public string Value1 { get; set; }
        public string Value2 { get; set; }
    }

    [ConfigFile("NotExistConfigFile.json")]
    public class NotExistConfigFileConfig : ConfigBase<NotExistConfigFileConfig>
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    [TestClass]
    public class ConfigurationFileTest
    {
        [TestMethod]
        public void FilePath()
        {
            var path = RuntimeOutPutConfig.GetFilePath();

            Assert.AreEqual("RuntimeOutPut.json", path);
        }

        [TestMethod]
        public void FileFullPath()
        {
            var path = RuntimeOutPutConfig.GetFileFullPath();

            Assert.AreEqual(Path.Combine(AppContext.BaseDirectory, "BambooConfig", "RuntimeOutPut.json"), path);
        }

        [TestMethod]
        public void FilePathWithFolder()
        {
            var path = RuntimeOutPutWithFolderConfig.GetFilePath();

            Assert.AreEqual("Sub\\RuntimeOutPutWithFolder.json", path);
        }

        [TestMethod]
        public void FileFullPathWithFolder()
        {
            var path = RuntimeOutPutWithFolderConfig.GetFileFullPath();

            Assert.AreEqual(Path.Combine(AppContext.BaseDirectory, "BambooConfig", "Sub", "RuntimeOutPutWithFolder.json"), path);
        }

        [TestMethod]
        public void StoreConfigTest()
        {
            // If file exists, delete firstly
            if (RuntimeOutPutConfig.FileExists())
                File.Delete(RuntimeOutPutConfig.GetFileFullPath());

            var instance = new RuntimeOutPutConfig
            {
                Value1 = "222"
            };

            //write the configuration instance to file
            instance.WriteToFile();

            instance.Value1 = "333";

            // the instance value is 333, but configuration file value is 222
            // so if we get from configuration, it will return 222
            Assert.AreEqual("333", instance.Value1);
            Assert.AreEqual("222", RuntimeOutPutConfig.Get().Value1);
            Assert.AreEqual("222", RuntimeOutPutConfig.GetValue<string>("Value1"));

            // it we bind the configuration entry to instance, the value will be 222
            // because it will set the configuration value to instance
            instance.Bind();

            Assert.AreEqual("222", instance.Value1);
        }

        [TestMethod]
        public void NotExistFileTest()
        {
            var exist = NotExistConfigFileConfig.FileExists();

            Assert.AreEqual(false, exist);
        }

        [TestMethod]
        public void GetConfigurationFileFullPathTest()
        {
            var path = NotExistConfigFileConfig.GetFileFullPath();

            Assert.AreNotEqual(string.Empty, path);
            Assert.AreEqual(Path.Combine(AppContext.BaseDirectory, "BambooConfig", "NotExistConfigFile.json"), path);
        }
    }
}
