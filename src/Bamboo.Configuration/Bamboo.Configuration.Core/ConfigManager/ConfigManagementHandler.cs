using Bamboo.Configuration.Core.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Bamboo.Configuration
{
    internal class ConfigManagementHandler
    {
        private static readonly ConcurrentDictionary<string, EventHandler> _reloadFileEvents = new ConcurrentDictionary<string, EventHandler>();

        public static object CreateAndSetupWatcher(object configInstance, string fileFullPath, EventHandler OnConfigFileChangedByFile)
        {
            SetupWatcher(fileFullPath);

            if (OnConfigFileChangedByFile != null)
                RegisterReloadNotification(fileFullPath, OnConfigFileChangedByFile);

            return configInstance;
        }

        public static void SetupWatcher(string fileFullPath)
        {
            FileWatcher.Instance.AddFile(fileFullPath, DelayedProcessConfigChange);
        }

        private static void DelayedProcessConfigChange(object sender, EventArgs args)
        {
            string filePath = ((string)sender);
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            //refresh the section in case anyone else uses it
            if (ConfigEntryBag.ConfigNameAndEntriesKeysMapping.TryGetValue(fileName, out List<string> keys))
            {
                foreach (var key in keys)
                {
                    var configType = ConfigEntryBag.ConfigEntries[key].Type;

                    object configInstance = LocalConfigurationManager.Instance.GetConfigInstanceNoListen(filePath, configType);

                    FileChangedEventArgs eventArgs = new FileChangedEventArgs(filePath, configInstance, configType);

                    if (_reloadFileEvents.TryGetValue(filePath, out EventHandler delegateMethod))
                        ThreadPool.QueueUserWorkItem(CallEventHandler, new EventObject(delegateMethod, configInstance, eventArgs));
                }
            }
        }

        private static void CallEventHandler(object obj)
        {
            ((EventObject)obj).Execute();
        }

        private static void RegisterReloadNotification(string filePath, EventHandler delegateMethod)
        {
            _reloadFileEvents.AddOrUpdate(filePath, delegateMethod);
        }
    }
}
