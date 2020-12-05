using Newtonsoft.Json;
using Bamboo.Configuration.Core.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Bamboo.Configuration
{
    internal class RemoteConfigurationManager : ConfigurationManagerBase
    {
        public static RemoteConfigurationManager Instance { get; set; } = new RemoteConfigurationManager();

        /// <summary>
        /// get remote method conllection
        /// </summary>
        private ConcurrentDictionary<string, Func<object>> _RemoteFunctions;

        private RemoteConfigurationManager() : base()
        {
            _RemoteFunctions = new ConcurrentDictionary<string, Func<object>>();

            //start timer
            System.Timers.Timer timer = new System.Timers.Timer(ConfigurationConst.TIMER_INTERVAL);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerCallback);
            timer.Start();
        }

        /// <summary>
        /// register method to get remote
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="type">which use in ConfigBase<'type'></param>
        /// <param name="getRemoteConfigFunction"></param>
        public void RegisterRemoteFunction(string configName, Type type, Func<object> getRemoteConfigFunction)
        {
            _RemoteFunctions.AddOrUpdate(GenerateUniqueConfigName(configName, type), getRemoteConfigFunction);
        }

        void TimerCallback(object sender, System.Timers.ElapsedEventArgs args)
        {
            foreach (var remote in _RemoteFunctions)
            {
                if (!CheckNeedToPullRemote(remote.Key))
                    continue;

                ThreadPool.QueueUserWorkItem(m =>
                {
                    DownloadRemoteConfig(remote.Key);
                });
            }
        }

        private bool CheckNeedToPullRemote(string configUniqueKey)
        {
            if (ConfigEntryBag.ConfigEntries.TryGetValue(configUniqueKey, out ConfigEntry entry) && entry != null && entry.Value != null)
            {
                var localMode = entry.Type.GetProperty("LocalMode")?.GetValue(entry.Value);

                if (localMode == null || !(localMode is bool))
                    return true;

                return !(bool)localMode;
            }

            return true;
        }

        private void DownloadRemoteConfig(string configName_Type_Key)
        {
            //get remote config download func
            var func = _RemoteFunctions.SafeGet<string, Func<object>>(configName_Type_Key) ?? throw new KeyNotFoundException($"The Configuration key[{configName_Type_Key}] is not registered!");
            
            string fileFullPath = ConfigPathHelper.GetConfigFileFullPath(configName_Type_Key.Substring(0, configName_Type_Key.IndexOf(ConfigurationConst.SPLITE_SYMBOL)));
            
            string tmpFile = fileFullPath + "." + Guid.NewGuid().ToString("N");
            
            try
            {
                //get remote config
                var config = func.Invoke();
                SaveToFile(tmpFile, config);
                //backup
                //BackUpConfig(fileFullPath);

                //this sucks, but this is to reduce the confliction of writing and reading 
                // because of sucks of Windows, the copyfile is non-transaction. 
                // we must remove the file before change its name!!!
                if (File.Exists(fileFullPath))
                    File.Delete(fileFullPath);

                File.Move(tmpFile, fileFullPath);
            }
            catch (Exception)
            {
                //log
            }
            finally
            {
                if (File.Exists(tmpFile))
                    File.Delete(tmpFile);
            }
        }

        private void SaveToFile(string fileFullPath, object config)
        {
            if (!Directory.Exists(ConfigPathHelper.BaseConfigDir))
                Directory.CreateDirectory(ConfigPathHelper.BaseConfigDir);

            //false means overwrite the file
            using (StreamWriter writer = new StreamWriter(fileFullPath, false))
            {
                writer.WriteLine(JsonConvert.SerializeObject(config));
                writer.Flush();
                writer.Close();
            }
        }

        private void BackUpConfig(string fileFullPath)
        {
            if (!File.Exists(fileFullPath))
                return;

            //backUp
            string backUpFileFullPath = $"{fileFullPath}.back";
            if (File.Exists(backUpFileFullPath))
                File.Delete(backUpFileFullPath);

            File.Copy(fileFullPath, backUpFileFullPath);
            File.Delete(fileFullPath);
        }

        protected override object OnCreate(string configName, Type type)
        {
            string fileFullPath = ConfigPathHelper.GetConfigFileFullPath(configName);

            object obj = LocalConfigurationManager.Instance.GetConfigInstance(fileFullPath, type);

            if (obj != null)
                return obj;

            //download from remote!
            DownloadRemoteConfig(GenerateUniqueConfigName(configName, type));

            return LocalConfigurationManager.Instance.GetConfigInstance(fileFullPath, type);
        }
    }
}
