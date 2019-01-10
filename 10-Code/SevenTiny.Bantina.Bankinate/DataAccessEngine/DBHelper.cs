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
using MySql.Data.MySqlClient;
using SevenTiny.Bantina.Bankinate.DbContexts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Bankinate.DataAccessEngine
{
    public abstract class DbHelper
    {
        /// <summary>
        /// ExcuteNonQuery 执行sql语句或者存储过程,返回影响的行数---ExcuteNonQuery
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(DbContext dbContext)
        {
            using (SqlConnection_RW conn = new SqlConnection_RW(dbContext.DataBaseType, dbContext.ConnString_RW))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(dbContext.DataBaseType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, dbContext.SqlStatement, dbContext.CommandType, dbContext.Parameters);//参数增加了参数增加了commandType 可以自己编辑执行方式
                    if (dbContext.OpenRealExecutionSaveToDb)
                        return cmd.DbCommand.ExecuteNonQuery();
                    return default(int);
                }
            }
        }
        public static Task<int> ExecuteNonQueryAsync(DbContext dbContext)
        {
            using (SqlConnection_RW conn = new SqlConnection_RW(dbContext.DataBaseType, dbContext.ConnString_RW))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(dbContext.DataBaseType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, dbContext.SqlStatement, dbContext.CommandType, dbContext.Parameters);//参数增加了commandType 可以自己编辑执行方式
                    if (dbContext.OpenRealExecutionSaveToDb)
                        return cmd.DbCommand.ExecuteNonQueryAsync();
                    return default(Task<int>);
                }
            }
        }
        public static void BatchExecuteNonQuery(DbContext dbContext, IEnumerable<BatchExecuteModel> batchExecuteModels)
        {
            using (SqlConnection_RW conn = new SqlConnection_RW(dbContext.DataBaseType, dbContext.ConnString_RW))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(dbContext.DataBaseType))
                {
                    foreach (var item in batchExecuteModels)
                    {
                        PreparCommand(conn.DbConnection, cmd.DbCommand, item.CommandTextOrSpName, item.CommandType, item.ParamsDic);
                        if (dbContext.OpenRealExecutionSaveToDb)
                            cmd.DbCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        public static void BatchExecuteNonQueryAsync(DbContext dbContext, IEnumerable<BatchExecuteModel> batchExecuteModels)
        {
            using (SqlConnection_RW conn = new SqlConnection_RW(dbContext.DataBaseType, dbContext.ConnString_RW))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(dbContext.DataBaseType))
                {
                    foreach (var item in batchExecuteModels)
                    {
                        PreparCommand(conn.DbConnection, cmd.DbCommand, item.CommandTextOrSpName, item.CommandType, item.ParamsDic);
                        if (dbContext.OpenRealExecutionSaveToDb)
                            cmd.DbCommand.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        /// <summary>
        /// ExecuteScalar 执行sql语句或者存储过程,执行单条语句，返回单个结果---ScalarExecuteScalar
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public static object ExecuteScalar(DbContext dbContext)
        {
            using (SqlConnection_RW conn = new SqlConnection_RW(dbContext.DataBaseType, dbContext.ConnString_RW))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(dbContext.DataBaseType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, dbContext.SqlStatement, dbContext.CommandType, dbContext.Parameters);
                    if (dbContext.OpenRealExecutionSaveToDb)
                        return cmd.DbCommand.ExecuteScalar();
                    return default(object);
                }

            }
        }
        public static Task<object> ExecuteScalarAsync(DbContext dbContext)
        {
            using (SqlConnection_RW conn = new SqlConnection_RW(dbContext.DataBaseType, dbContext.ConnString_RW))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(dbContext.DataBaseType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, dbContext.SqlStatement, dbContext.CommandType, dbContext.Parameters);
                    if (dbContext.OpenRealExecutionSaveToDb)
                        return cmd.DbCommand.ExecuteScalarAsync();
                    return default(Task<object>);
                }

            }
        }

        /// <summary>
        /// ExecuteReader 执行sql语句或者存储过程,返回DataReader---DataReader
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public static DbDataReader ExecuteReader(DbContext dbContext)
        {
            //sqlDataReader不能用using 会关闭conn 导致不能获取到返回值。注意：DataReader获取值时必须保持连接状态
            SqlConnection_RW conn = new SqlConnection_RW(dbContext.DataBaseType, dbContext.ConnString_R, dbContext.ConnString_RW);
            DbCommandCommon cmd = new DbCommandCommon(dbContext.DataBaseType);
            PreparCommand(conn.DbConnection, cmd.DbCommand, dbContext.SqlStatement, dbContext.CommandType, dbContext.Parameters);
            if (dbContext.OpenRealExecutionSaveToDb)
                return cmd.DbCommand.ExecuteReader(CommandBehavior.CloseConnection);
            return default(DbDataReader);
        }

        /// <summary>
        /// ExecuteDataTable 执行sql语句或者存储过程,返回一个DataTable---DataTable
        /// Update At 2017-3-2 14:58:45
        /// Add the ExecuteDataTable Method into Sql_Helper_DG  
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns> 
        public static DataTable ExecuteDataTable(DbContext dbContext)
        {
            DataSet ds = ExecuteDataSet(dbContext);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            return default(DataTable);
        }

        /// <summary>
        /// ExecuteDataSet 执行sql语句或者存储过程,返回一个DataSet---DataSet
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(DbContext dbContext)
        {
            using (SqlConnection_RW conn = new SqlConnection_RW(dbContext.DataBaseType, dbContext.ConnString_R, dbContext.ConnString_RW))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(dbContext.DataBaseType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, dbContext.SqlStatement, dbContext.CommandType, dbContext.Parameters);
                    if (dbContext.OpenRealExecutionSaveToDb)
                    {
                        using (DbDataAdapterCommon da = new DbDataAdapterCommon(dbContext.DataBaseType, cmd.DbCommand))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            return ds;
                        }
                    }
                    else
                    {
                        return default(DataSet);
                    }
                }
            }
        }

        /// <summary>
        /// ExecuteList Entity 执行sql语句或者存储过程，返回一个List<T>---List<T>
        /// </summary>
        /// <typeparam name="Entity"></typeparam>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public static List<Entity> ExecuteList<Entity>(DbContext dbContext) where Entity : class
        {
            return GetListFromDataSetV2<Entity>(ExecuteDataSet(dbContext));
        }

        /// <summary>
        /// ExecuteEntity 执行sql语句或者存储过程，返回一个Entity---Entity
        /// </summary>
        /// <typeparam name="Entity"></typeparam>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public static Entity ExecuteEntity<Entity>(DbContext dbContext) where Entity : class
        {
            return GetEntityFromDataSetV2<Entity>(ExecuteDataSet(dbContext));
        }

        /// <summary>
        ///  ---PreparCommand 构建一个通用的command对象供内部方法进行调用---
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="cmd"></param>
        /// <param name="commandTextOrSpName"></param>
        /// <param name="commandType"></param>
        /// <param name="dictionary"></param>
        private static void PreparCommand(DbConnection conn, DbCommand cmd, string commandTextOrSpName, CommandType commandType, IDictionary<string, object> dictionary = null)
        {
            //打开连接
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            //设置SqlCommand对象的属性值
            cmd.Connection = conn;
            cmd.CommandType = commandType;
            cmd.CommandText = commandTextOrSpName;
            cmd.CommandTimeout = 60;

            if (dictionary != null)
            {
                cmd.Parameters.Clear();
                DbParameter[] parameters;
                switch (conn)
                {
                    case SqlConnection s:
                        parameters = new SqlParameter[dictionary.Count];
                        break;
                    case MySqlConnection m:
                        parameters = new MySqlParameter[dictionary.Count];
                        break;
                    //case OracleConnection o:
                    //parameters = new OracleParameter[dictionary.Count];
                    //break;
                    default:
                        parameters = new SqlParameter[dictionary.Count];
                        break;
                }

                string[] keyArray = dictionary.Keys.ToArray();
                object[] valueArray = dictionary.Values.ToArray();

                for (int i = 0; i < parameters.Length; i++)
                {
                    switch (conn)
                    {
                        case SqlConnection s:
                            parameters[i] = new SqlParameter(keyArray[i], valueArray[i] ?? DBNull.Value);
                            break;
                        case MySqlConnection m:
                            parameters[i] = new MySqlParameter(keyArray[i], valueArray[i] ?? DBNull.Value);
                            break;
                        //case OracleConnection o:
                        // parameters[i] = new OracleParameter(keyArray[i], valueArray[i]);
                        // break;
                        default:
                            parameters[i] = new SqlParameter(keyArray[i], valueArray[i] ?? DBNull.Value);
                            break;
                    }
                }
                cmd.Parameters.AddRange(parameters);
            }
        }

        #region 通过Model反射返回结果集 Model为 Entity 泛型变量的真实类型---反射返回结果集
        //DESC:由于性能较低，现在使用全部切换到高性能的方法V2版本，V1版本代码切换成私有方法不再对外开放 -- 7tiny - 2019年1月10日 22点46分
        private static List<Entity> GetListFromDataSet<Entity>(DataSet ds) where Entity : class
        {
            List<Entity> list = new List<Entity>();//实例化一个list对象
            PropertyInfo[] propertyInfos = typeof(Entity).GetProperties();     //获取T对象的所有公共属性

            DataTable dt = ds.Tables[0];//获取到ds的dt
            if (dt.Rows.Count > 0)
            {
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
            }
            return list;
        }
        private static Entity GetEntityFromDataReader<Entity>(DbDataReader reader) where Entity : class
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
        private static Entity GetEntityFromDataSet<Entity>(DataSet ds) where Entity : class
        {
            return GetListFromDataSet<Entity>(ds).FirstOrDefault();
        }

        /// <summary>
        /// ExpressionTree高性能转换DataSet为List集合
        /// </summary>
        /// <typeparam name="Entity"></typeparam>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static List<Entity> GetListFromDataSetV2<Entity>(DataSet ds) where Entity : class
        {
            List<Entity> list = new List<Entity>();
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Entity entity = FillAdapter<Entity>.AutoFill(row);
                    list.Add(entity);
                }
            }
            return list;
        }
        public static Entity GetEntityFromDataSetV2<Entity>(DataSet ds) where Entity : class
        {
            DataTable dt = ds.Tables[0];// 获取到ds的dt
            if (dt.Rows.Count > 0)
            {
                return FillAdapter<Entity>.AutoFill(dt.Rows[0]);
            }
            return default(Entity);
        }
        #endregion
    }
}