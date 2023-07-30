using System;
using System.Collections.Generic;

namespace Bamboo.Configuration
{
    /// <summary>
    /// 以数据表的值做映射，结果为表中多行数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MySqlRowConfigBase<T> : ConfigBase<List<T>> where T : class, new()
    {
        private static string _ConfigName = ConfigNameAttribute.GetName(typeof(T));
        public static List<T> Instance => GetConfig(_ConfigName);

        static MySqlRowConfigBase()
        {
            RegisterGetRemoteFunction(_ConfigName, typeof(List<T>), () =>
             {
                 string _ConnectionString = ConnectionStringManager.GetConnectionStringFromAppSettings<T>(_ConfigName);

                 if (string.IsNullOrEmpty(_ConnectionString))
                     throw new ArgumentNullException(nameof(_ConnectionString), $"{nameof(_ConnectionString)} must be provide.");

                 using (var db = new ConfigDbContext(_ConnectionString))
                 {
                     return db.Queryable<T>($"SELECT * FROM {_ConfigName}").ToList();
                 }
             });
        }
    }
}
