using System;

namespace Bamboo.Configuration
{
    internal class ConfigEntry
    {
        public delegate object CreateObjectDelegate(string configName, Type type);

        private bool _isSet;
        private object locker;
        private CreateObjectDelegate OnCreate;

        public string ConfigName { get; private set; }

        public Type Type { get; private set; }

        public ConfigEntry(string configName, CreateObjectDelegate creater, Type type)
        {
            this.ConfigName = configName;

            _isSet = false;
            locker = new object();
            Type = type;
            OnCreate = creater;
        }

        private object val;
        public object Value
        {
            get
            {
                if (!_isSet)
                {
                    lock (locker)
                    {
                        if (!_isSet)
                        {
                            val = OnCreate(this.ConfigName, Type);
                            _isSet = true;
                        }
                    }
                }
                return val;
            }
            set
            {
                val = value;
                _isSet = true;
            }
        }
    }
}
