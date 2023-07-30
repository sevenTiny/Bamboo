/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-04-19 21:34:01
 * Modify: 2018-04-19 21:34:01
 * Modify: 2019年1月7日 15点41分 -- 将原来的单文件进行拆分，提高代码整洁性
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using SevenTiny.Bantina.Bankinate.DbContexts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Bankinate.SqlDataAccess
{
    /// <summary>
    /// 为了统一控制，这里仅仅存在执行语句，初始化转移到了上下文中进行管理
    /// </summary>
    internal class QueryExecutor
    {
        public QueryExecutor(SqlDbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        private SqlDbContext DbContext;

        /// <summary>
        /// ExcuteNonQuery 执行sql语句或者存储过程,返回影响的行数---ExcuteNonQuery
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public int ExecuteNonQuery()
        {
            if (DbContext.RealExecutionSaveToDb)
            {
                DbContext.ParameterInitializes();
                DbContext.ConnectionStatusCheck();
                return DbContext.DbCommand.ExecuteNonQuery();
            }
            return default(int);
        }
        public Task<int> ExecuteNonQueryAsync()
        {
            if (DbContext.RealExecutionSaveToDb)
            {
                DbContext.ParameterInitializes();
                DbContext.ConnectionStatusCheck();
                return DbContext.DbCommand.ExecuteNonQueryAsync();
            }
            return default(Task<int>);
        }

        /// <summary>
        /// ExecuteScalar 执行sql语句或者存储过程,执行单条语句，返回单个结果---ScalarExecuteScalar
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public object ExecuteScalar()
        {
            if (DbContext.RealExecutionSaveToDb)
            {
                DbContext.ParameterInitializes();
                DbContext.ConnectionStatusCheck();
                return DbContext.DbCommand.ExecuteScalar();
            }
            return default(object);
        }
        public Task<object> ExecuteScalarAsync()
        {
            if (DbContext.RealExecutionSaveToDb)
            {
                DbContext.ParameterInitializes();
                DbContext.ConnectionStatusCheck();
                return DbContext.DbCommand.ExecuteScalarAsync();
            }
            return default(Task<object>);
        }

        /// <summary>
        /// ExecuteReader 执行sql语句或者存储过程,返回DataReader---DataReader
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public DbDataReader ExecuteReader()
        {
            if (DbContext.RealExecutionSaveToDb)
            {
                DbContext.ParameterInitializes();
                DbContext.ConnectionStatusCheck();
                return DbContext.DbCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
            return default(DbDataReader);
        }

        /// <summary>
        /// ExecuteDataTable 执行sql语句或者存储过程,返回一个DataTable---DataTable
        /// Update At 2017-3-2 14:58:45
        /// Add the ExecuteDataTable Method into Sql_Helper_DG  
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns> 
        public DataTable ExecuteDataTable()
        {
            if (DbContext.RealExecutionSaveToDb)
            {
                DataSet ds = ExecuteDataSet();
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }
                return default(DataTable);
            }
            return default(DataTable);
        }

        /// <summary>
        /// ExecuteDataSet 执行sql语句或者存储过程,返回一个DataSet---DataSet
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet()
        {
            if (DbContext.RealExecutionSaveToDb)
            {
                DbContext.ParameterInitializes();
                DbContext.ConnectionStatusCheck();
                DataSet ds = new DataSet();
                DbContext.DbDataAdapter.Fill(ds);
                return ds;
            }
            return default(DataSet);
        }

        /// <summary>
        /// ExecuteList Entity 执行sql语句或者存储过程，返回一个List<T>---List<T>
        /// </summary>
        /// <typeparam name="Entity"></typeparam>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public List<Entity> ExecuteList<Entity>() where Entity : class
        {
            return GetListFromDataSetV2<Entity>(ExecuteDataSet());
        }

        /// <summary>
        /// ExecuteEntity 执行sql语句或者存储过程，返回一个Entity---Entity
        /// </summary>
        /// <typeparam name="Entity"></typeparam>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public Entity ExecuteEntity<Entity>() where Entity : class
        {
            return GetEntityFromDataSetV2<Entity>(ExecuteDataSet());
        }

        #region 通过Model反射返回结果集 Model为 Entity 泛型变量的真实类型---反射返回结果集
        //DESC:由于性能较低，现在使用全部切换到高性能的方法V2版本，V1版本代码切换成私有方法不再对外开放 -- 7tiny - 2019年1月10日 22点46分
        private List<Entity> GetListFromDataSet<Entity>(DataSet ds) where Entity : class
        {
            DataTable dt = ds.Tables[0];//获取到ds的dt
            if (dt.Rows.Count > 0)
            {
                PropertyInfo[] propertyInfos = typeof(Entity).GetProperties();     //获取T对象的所有公共属性
                List<Entity> list = new List<Entity>();//实例化一个list对象
                                                       //判断读取的行是否>0 即数据库数据已被读取
                foreach (DataRow row in dt.Rows)
                {
                    Entity model1 = System.Activator.CreateInstance<Entity>();//实例化一个对象，便于往list里填充数据
                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {
                        try
                        {
                            //遍历模型里所有的字段
                            if (row[propertyInfo.Name] != System.DBNull.Value)
                            {
                                //判断值是否为空，如果空赋值为null见else
                                if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                                {
                                    //如果convertsionType为nullable类，声明一个NullableConverter类，该类提供从Nullable类到基础基元类型的转换
                                    NullableConverter nullableConverter = new NullableConverter(propertyInfo.PropertyType);
                                    //将convertsionType转换为nullable对的基础基元类型
                                    propertyInfo.SetValue(model1, Convert.ChangeType(row[propertyInfo.Name], nullableConverter.UnderlyingType), null);
                                }
                                else
                                {
                                    propertyInfo.SetValue(model1, Convert.ChangeType(row[propertyInfo.Name], propertyInfo.PropertyType), null);
                                }
                            }
                            else
                            {
                                propertyInfo.SetValue(model1, null, null);//如果数据库的值为空，则赋值为null
                            }
                        }
                        catch (Exception)
                        {
                            propertyInfo.SetValue(model1, null, null);//如果数据库的值为空，则赋值为null
                        }
                    }
                    list.Add(model1);//将对象填充到list中
                }
                return list;
            }
            return default(List<Entity>);
        }
        private Entity GetEntityFromDataReader<Entity>(DbDataReader reader) where Entity : class
        {
            Entity model = System.Activator.CreateInstance<Entity>();           //实例化一个T类型对象
            PropertyInfo[] propertyInfos = model.GetType().GetProperties();     //获取T对象的所有公共属性
            using (reader)
            {
                if (reader.Read())
                {
                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {
                        //遍历模型里所有的字段
                        if (reader[propertyInfo.Name] != System.DBNull.Value)
                        {
                            //判断值是否为空，如果空赋值为null见else
                            if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                            {
                                //如果convertsionType为nullable类，声明一个NullableConverter类，该类提供从Nullable类到基础基元类型的转换
                                NullableConverter nullableConverter = new NullableConverter(propertyInfo.PropertyType);
                                //将convertsionType转换为nullable对的基础基元类型
                                propertyInfo.SetValue(model, Convert.ChangeType(reader[propertyInfo.Name], nullableConverter.UnderlyingType), null);
                            }
                            else
                            {
                                propertyInfo.SetValue(model, Convert.ChangeType(reader[propertyInfo.Name], propertyInfo.PropertyType), null);
                            }
                        }
                        else
                        {
                            propertyInfo.SetValue(model, null, null);//如果数据库的值为空，则赋值为null
                        }
                    }
                    return model;//返回T类型的赋值后的对象 model
                }
            }
            return default(Entity);//返回引用类型和值类型的默认值0或null
        }
        private Entity GetEntityFromDataSet<Entity>(DataSet ds) where Entity : class
        {
            return GetListFromDataSet<Entity>(ds)?.FirstOrDefault();
        }

        /// <summary>
        /// ExpressionTree高性能转换DataSet为List集合
        /// </summary>
        /// <typeparam name="Entity"></typeparam>
        /// <param name="ds"></param>
        /// <returns></returns>
        public List<Entity> GetListFromDataSetV2<Entity>(DataSet ds) where Entity : class
        {
            if (DbContext.RealExecutionSaveToDb)
            {
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    List<Entity> list = new List<Entity>();
                    foreach (DataRow row in dt.Rows)
                    {
                        Entity entity = FillAdapter<Entity>.AutoFill(row);
                        list.Add(entity);
                    }
                    return list;
                }
                return default(List<Entity>);
            }
            return default(List<Entity>);
        }
        public Entity GetEntityFromDataSetV2<Entity>(DataSet ds) where Entity : class
        {
            if (DbContext.RealExecutionSaveToDb)
            {
                DataTable dt = ds.Tables[0];// 获取到ds的dt
                if (dt.Rows.Count > 0)
                {
                    return FillAdapter<Entity>.AutoFill(dt.Rows[0]);
                }
                return default(Entity);
            }
            return default(Entity);
        }
        #endregion
    }
}