using Bamboo.Configuration.Core.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace Bamboo.Configuration
{
    internal class DirectoryWatcher
    {
        private SafeReaderWriterLock filesLock;
        private ConcurrentDictionary<string, EventHandler> files;
        private List<string> pendingFileReloads;
        private string directory;
        private static readonly SafeReaderWriterLock fileLoadResourceLock = new SafeReaderWriterLock();

        public DirectoryWatcher(string directory)
        {
            filesLock = new SafeReaderWriterLock();
            files = new ConcurrentDictionary<string, EventHandler>();
            this.directory = directory;
            pendingFileReloads = new List<string>();
            InitWatcher(directory);
        }

        /// <summary>
        /// call only once when DirectoryWatcher created
        /// </summary>
        /// <param name="directory"></param>
        void InitWatcher(string directory)
        {
            FileSystemWatcher scareCrow = new FileSystemWatcher();
            scareCrow.Path = directory;
            scareCrow.IncludeSubdirectories = false;

            scareCrow.Changed += scareCrow_Changed;
            scareCrow.Renamed += scareCrow_Changed;
            scareCrow.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="delegateMethod">delegateMethod(string filePath,Empty), filePath is in lower case</param>
        public void AddFile(string fileName, EventHandler delegateMethod)
        {
            using (filesLock.AcquireWriterLock())
            {
                if (!files.ContainsKey(fileName))
                    files.AddOrUpdate(fileName, delegateMethod);
            }
        }

        EventHandler GetEventHandler(string fileName)
        {
            EventHandler handler;
            using (filesLock.AcquireReaderLock())
            {
                files.TryGetValue(fileName, out handler);
            }
            return handler;
        }

        bool ContainsFile(string fileName)
        {
            bool contains;
            using (filesLock.AcquireReaderLock())
            {
                contains = files.ContainsKey(fileName);
            }
            return contains;
        }

        private void scareCrow_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                string fileName = e.Name;
                using (fileLoadResourceLock.AcquireWriterLock())
                {
                    if (pendingFileReloads.Contains(fileName) || !ContainsFile(fileName))
                        return;

                    pendingFileReloads.Add(fileName);
                }

                CountdownTimer timer = new CountdownTimer();
                timer.BeginCountdown(ConfigurationConst.CHANGE_FILE_DELAY, DelayedProcessFileChanged, fileName);
            }
            catch (Exception)
            {
                //logger.HandleException(ex, "FileWatcher");
            }
        }

        internal void ProcessFileChanged(string fileName)
        {
            EventHandler delegateMethod = GetEventHandler(fileName);
            if (delegateMethod != null)
            {
                try
                {
                    string filePath = Path.Combine(directory, fileName);
                    delegateMethod(filePath, EventArgs.Empty);
                }
                catch (Exception)
                {
                    //logger.HandleException(ex, "FileWatcher");
                }
            }
        }

        private void DelayedProcessFileChanged(IAsyncResult ar)
        {
            try
            {
                string fileName = (string)ar.AsyncState;

                using (fileLoadResourceLock.AcquireWriterLock())
                {
                    pendingFileReloads.Remove(fileName);
                }

                //only one Handler for one file!!
                ProcessFileChanged(fileName);
            }
            catch (Exception)
            {
                //logger.HandleException(ex, "FileWatcher");
            }
        }
    }

    internal class FileWatcher
    {
        object dirsLock;
        ConcurrentDictionary<string, DirectoryWatcher> directories;

        static FileWatcher instance = new FileWatcher();
        public static FileWatcher Instance
        {
            get
            {
                return instance;
            }
        }

        protected FileWatcher()
        {
            dirsLock = new object();
            directories = new ConcurrentDictionary<string, DirectoryWatcher>();
        }

        public void AddFile(string filePath, EventHandler handler)
        {
            DirectoryWatcher watcher;

            lock (dirsLock)
            {
                string dir = Path.GetDirectoryName(filePath);

                if (!directories.TryGetValue(dir, out watcher))
                {
                    watcher = new DirectoryWatcher(dir);

                    directories.AddOrUpdate(dir, watcher);
                }
            }
            watcher.AddFile(Path.GetFileName(filePath), handler);
        }
    }
}
