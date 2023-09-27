using System.Collections.Generic;
using System.Timers;

namespace Bamboo.Configuration.Remote
{
    internal static class ConfigDownloadManager
    {
        private static object _lock = new object();
        private static HashSet<string> _existTimerKey = new HashSet<string>();

        internal static void StartTimerIfNotExist(string key, double interval, ElapsedEventHandler eventHandler)
        {
            lock (_lock)
            {
                if (!_existTimerKey.Add(key))
                    return;

                var timer = new Timer(interval);
                timer.Elapsed += eventHandler;
                timer.AutoReset = true;
                timer.Enabled = true;
            }
        }
    }
}
