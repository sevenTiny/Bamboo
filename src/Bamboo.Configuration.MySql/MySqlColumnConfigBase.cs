using System;

namespace Bamboo.Configuration
{
    /// <summary>
    /// 以数据表的列映射，结果为表中一行数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MySqlColumnConfigBase<T> : ConfigBase<T> where T : class, new()
    {
        private static string _ConfigName = ConfigNameAttribute.GetName(typeof(T));
        public static T Instance => GetConfig(_ConfigName);

        static MySqlColumnConfigBase()
        {
            RegisterGetRemoteFunction(_ConfigName, typeof(T), () =>
            {
                string _ConnectionString = ConnectionStringManager.GetConnectionStringFromAppSettings<T>(_ConfigName);

                if (string.IsNullOrEmpty(_ConnectionString))
                    throw new ArgumentNullException(nameof(_ConnectionString), $"{nameof(_ConnectionString)} must be provide.");

                using (var db = new ConfigDbContext(_ConnectionString))
                {
                    return db.Queryable<T>($"SELECT * FROM {_ConfigName} LIMIT 1").FirstOrDefault();
                }
            });
        }
    }
}
