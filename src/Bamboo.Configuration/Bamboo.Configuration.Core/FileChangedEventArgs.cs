using System;

namespace Bamboo.Configuration
{
    internal class FileChangedEventArgs : EventArgs
    {
        public string FileFullPath { get; private set; }
        public object ConfigInstance { get; private set; }
        public Type ConfigType { get; private set; }

        public FileChangedEventArgs(string fileFullPath, object configInstance, Type configType)
        {
            this.FileFullPath = fileFullPath;
            this.ConfigInstance = configInstance;
            this.ConfigType = configType;
        }
    }
}