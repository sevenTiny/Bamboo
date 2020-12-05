/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-13 22:32:44
 * Modify: 2018-05-06 22:00:27
 * Modify: 2019年6月19日 23点14分 基本完成了自动同步远程模式
 * E-mail: dong@7tiny.com | Bamboo@foxmail.com 
 * GitHub: https://github.com/Bamboo 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using Bamboo.Configuration.Core;
using Bamboo.Configuration.Core.Helpers;
using System;
using System.IO;

namespace Bamboo.Configuration
{
    public abstract class ConfigBase<T> where T : class, new()
    {
        /// <summary>
        /// local mode signal（set true，nolonger pull remote）
        /// </summary>
        public bool LocalMode { get; set; }

        static ConfigBase()
        {
            if (!Directory.Exists(ConfigPathHelper.BaseConfigDir))
                Directory.CreateDirectory(ConfigPathHelper.BaseConfigDir);
        }

        protected static T GetConfig(string configName)
        {
            return ConfigManager.GetSection<T>(configName);
        }

        /// <summary>
        /// register how to get remote function
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="type">which use in ConfigBase<'type'></param>
        /// <param name="getRemoteConfigFunction">how to get remote</param>
        protected static void RegisterGetRemoteFunction(string configName, Type type, Func<object> getRemoteConfigFunction)
        {
            if (string.IsNullOrEmpty(configName))
                throw new ArgumentNullException(nameof(configName), $"{nameof(configName)} must provider!");

            RemoteConfigurationManager.Instance.RegisterRemoteFunction(configName, type, getRemoteConfigFunction);
        }
    }
}