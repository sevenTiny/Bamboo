using System;

namespace Bamboo.Configuration.Helpers
{
    /// <summary>
    /// global copy locker
    /// </summary>
    internal static class CopyLocker
    {
        private static readonly object locker = new object();

        public static void Copy(Action action)
        {
            lock (locker)
            {
                action();
            }
        }
    }
}
