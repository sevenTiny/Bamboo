//using Bamboo.Configuration.Core.Helpers;
//using System.IO;

//namespace Bamboo.Configuration.Core
//{
//    [ConfigName("BambooConfigSetting")]
//    internal class BambooConfigSetting : ConfigBase<BambooConfigSetting>
//    {
//        private static string _ConfigName = ConfigNameAttribute.GetName(typeof(BambooConfigSetting));
//        public static BambooConfigSetting Instance
//        {
//            get
//            {
//                var instance = LocalConfigurationManager.Instance.GetSection<BambooConfigSetting>(_ConfigName);

//                //if not found ,retry once time
//                if (instance == null)
//                {
//                    GenerateConfigSetting();

//                    instance = LocalConfigurationManager.Instance.GetSection<BambooConfigSetting>(_ConfigName);
//                }

//                return instance;
//            }
//        }

//        static BambooConfigSetting()
//        {
//            GenerateConfigSetting();
//        }

//        private static void GenerateConfigSetting()
//        {
//            string jsonContent = "{\"LocalMode\":false}";

//            string fullPath = ConfigPathHelper.GetConfigFileFullPath(_ConfigName);

//            if (!Directory.Exists(ConfigPathHelper.BaseConfigDir))
//                Directory.CreateDirectory(ConfigPathHelper.BaseConfigDir);

//            if (!File.Exists(fullPath))
//                File.WriteAllText(fullPath, jsonContent);
//        }

//        /// <summary>
//        /// local mode,donnot pull another remote configs nolonger
//        /// </summary>
//        public bool LocalMode { get; set; }
//    }
//}
