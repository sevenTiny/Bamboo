/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-04-19 21:34:01
 * Modify: 2018-04-19 21:34:01
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Bankinate
{
    public enum DataBaseType
    {
        SqlServer,
        MySql,
        Oracle,
        MongoDB
    }
    public abstract class DbHelper
    {
        #region ConnString 链接字符串声明

        /// <summary>
        /// 连接字符串 ConnString_Default 默认，且赋值时会直接覆盖掉读写
        /// </summary>
        private static string _connString;
        public static string ConnString_Default
        {
            get { return _connString; }
            set
            {
                _connString = value;
                ConnString_RW = _connString;
                ConnString_R = _connString;
            }
        }
        /// <summary>
        /// 连接字符串 ConnString_RW 读写数据库使用
        /// </summary>
        public static string ConnString_RW { get; set; } = _connString;
        /// <summary>
        /// 连接字符串 ConnString_R 读数据库使用
        /// </summary>
        public static string ConnString_R { get; set; } = _connString;
        /// <summary>
        /// DataBaseType Select default:sqlserver
        /// </summary>
        public static DataBaseType DbType { get; set; } = DataBaseType.MySql;

        #endregion

        #region ExcuteNonQuery 执行sql语句或者存储过程,返回影响的行数---ExcuteNonQuery
        public static int ExecuteNonQuery(string commandTextOrSpName, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection_RW conn = new SqlConnection_RW(DbType, ConnString_RW))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DbType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, commandTextOrSpName, commandType);
                    return cmd.DbCommand.ExecuteNonQuery();
                }
            }
        }
        public static int ExecuteNonQuery(string commandTextOrSpName, CommandType commandType, IDictionary<string, object> dictionary)
        {
            using (SqlConnection_RW conn = new SqlConnection_RW(DbType, ConnString_RW))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DbType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, commandTextOrSpName, commandType, dictionary);//参数增加了commandType 可以自己编辑执行方式
                    return cmd.DbCommand.ExecuteNonQuery();
                }
            }
        }
        public static Task<int> ExecuteNonQueryAsync(string commandTextOrSpName, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection_RW conn = new SqlConnection_RW(DbType, ConnString_RW))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DbType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, commandTextOrSpName, commandType);
                    return cmd.DbCommand.ExecuteNonQueryAsync();
                }
            }
        }
        public static Task<int> ExecuteNonQueryAsync(string commandTextOrSpName, CommandType commandType, IDictionary<string, object> dictionary)
        {
            using (SqlConnection_RW conn = new SqlConnection_RW(DbType, ConnString_RW))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DbType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, commandTextOrSpName, commandType, dictionary);//参数增加了commandType 可以自己编辑执行方式
                    return cmd.DbCommand.ExecuteNonQueryAsync();
                }
            }
        }
        #endregion

        #region ExecuteScalar 执行sql语句或者存储过程,执行单条语句，返回单个结果---ScalarExecuteScalar
        public static object ExecuteScalar(string commandTextOrSpName, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection_RW conn = new SqlConnection_RW(DbType, ConnString_R, ConnString_RW))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DbType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, commandTextOrSpName, commandType);
                    return cmd.DbCommand.ExecuteScalar();
                }
            }
        }
        public static object ExecuteScalar(string commandTextOrSpName, CommandType commandType, IDictionary<string, object> dictionary)
        {
            using (SqlConnection_RW conn = new SqlConnection_RW(DbType, ConnString_R, ConnString_RW))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DbType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, commandTextOrSpName, commandType, dictionary);
                    return cmd.DbCommand.ExecuteScalar();
                }

            }
        }
        public static Task<object> ExecuteScalarAsync(string commandTextOrSpName, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection_RW conn = new SqlConnection_RW(DbType, ConnString_R, ConnString_RW))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DbType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, commandTextOrSpName, commandType);
                    return cmd.DbCommand.ExecuteScalarAsync();
                }
            }
        }
        public static Task<object> ExecuteScalarAsync(string commandTextOrSpName, CommandType commandType, IDictionary<string, object> dictionary)
        {
            using (SqlConnection_RW conn = new SqlConnection_RW(DbType, ConnString_R, ConnString_RW))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DbType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, commandTextOrSpName, commandType, dictionary);
                    return cmd.DbCommand.ExecuteScalarAsync();
                }

            }
        }
        #endregion

        #region ExecuteReader 执行sql语句或者存储过程,返回DataReader---DataReader
        public static DbDataReader ExecuteReader(string commandTextOrSpName, CommandType commandType = CommandType.Text)
        {
            //sqlDataReader不能用using 会关闭conn 导致不能获取到返回值。注意：DataReader获取值时必须保持连接状态
            SqlConnection_RW conn = new SqlConnection_RW(DbType, ConnString_R, ConnString_RW);
            DbCommandCommon cmd = new DbCommandCommon(DbType);
            PreparCommand(conn.DbConnection, cmd.DbCommand, commandTextOrSpName, commandType);
            return cmd.DbCommand.ExecuteReader(CommandBehavior.CloseConnection);
        }
        public static DbDataReader ExecuteReader(string commandTextOrSpName, CommandType commandType, IDictionary<string, object> dictionary)
        {
            //sqlDataReader不能用using 会关闭conn 导致不能获取到返回值。注意：DataReader获取值时必须保持连接状态
            SqlConnection_RW conn = new SqlConnection_RW(DbType, ConnString_R, ConnString_RW);
            DbCommandCommon cmd = new DbCommandCommon(DbType);
            PreparCommand(conn.DbConnection, cmd.DbCommand, commandTextOrSpName, commandType, dictionary);
            return cmd.DbCommand.ExecuteReader(CommandBehavior.CloseConnection);
        }
        #endregion

        #region ExecuteDataTable 执行sql语句或者存储过程,返回一个DataTable---DataTable

        /**
         * Update At 2017-3-2 14:58:45
         * Add the ExecuteDataTable Method into Sql_Helper_DG  
         **/
        public static DataTable ExecuteDataTable(string commandTextOrSpName, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection_RW conn = new SqlConnection_RW(DbType, ConnString_R, ConnString_RW))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DbType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, commandTextOrSpName, commandType);
                    using (DbDataAdapterCommon da = new DbDataAdapterCommon(DbType, cmd.DbCommand))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        if (ds.Tables.Count > 0)
                        {
                            return ds.Tables[0];
                        }
                        return default(DataTable);
                    }
                }
            }
        }
        public static DataTable ExecuteDataTable(string commandTextOrSpName, CommandType commandType, IDictionary<string, object> dictionary)
        {
            using (SqlConnection_RW conn = new SqlConnection_RW(DbType, ConnString_R, ConnString_RW))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DbType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, commandTextOrSpName, commandType, dictionary);
                    using (DbDataAdapterCommon da = new DbDataAdapterCommon(DbType, cmd.DbCommand))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        if (ds.Tables.Count > 0)
                        {
                            return ds.Tables[0];
                        }
                        return default(DataTable);
                    }
                }
            }
        }
        #endregion

        #region ExecuteDataSet 执行sql语句或者存储过程,返回一个DataSet---DataSet
        public static DataSet ExecuteDataSet(string commandTextOrSpName, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection_RW conn = new SqlConnection_RW(DbType, ConnString_R, ConnString_RW))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DbType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, commandTextOrSpName, commandType);
                    using (DbDataAdapterCommon da = new DbDataAdapterCommon(DbType, cmd.DbCommand))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        return ds;
                    }
                }
            }
        }
        public static DataSet ExecuteDataSet(string commandTextOrSpName, CommandType commandType, IDictionary<string, object> dictionary)
        {
            using (SqlConnection_RW conn = new SqlConnection_RW(DbType, ConnString_R, ConnString_RW))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DbType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, commandTextOrSpName, commandType, dictionary);
                    using (DbDataAdapterCommon da = new DbDataAdapterCommon(DbType, cmd.DbCommand))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        return ds;
                    }
                }
            }
        }
        #endregion

        #region ExecuteList Entity 执行sql语句或者存储过程，返回一个List<T>---List<T>
        public static List<Entity> ExecuteList<Entity>(string commandTextOrSpName, CommandType commandType = CommandType.Text) where Entity : class
        {
            return GetListFromDataSetV2<Entity>(ExecuteDataSet(commandTextOrSpName, commandType));
        }
        public static List<Entity> ExecuteList<Entity>(string commandTextOrSpName, CommandType commandType, IDictionary<string, object> dictionary) where Entity : class
        {
            return GetListFromDataSetV2<Entity>(ExecuteDataSet(commandTextOrSpName, commandType, dictionary));
        }
        #endregion

        #region ExecuteEntity 执行sql语句或者存储过程，返回一个Entity---Entity
        public static Entity ExecuteEntity<Entity>(string commandTextOrSpName, CommandType commandType = CommandType.Text) where Entity : class
        {
            return GetEntityFromDataSetV2<Entity>(ExecuteDataSet(commandTextOrSpName, commandType));
        }
        public static Entity ExecuteEntity<Entity>(string commandTextOrSpName, CommandType commandType, IDictionary<string, object> dictionary) where Entity : class
        {
            return GetEntityFromDataSetV2<Entity>(ExecuteDataSet(commandTextOrSpName, commandType, dictionary));
        }
        #endregion

        #region ---PreparCommand 构建一个通用的command对象供内部方法进行调用---
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
                            parameters[i] = new SqlParameter(keyArray[i], valueArray[i]);
                            break;
                        case MySqlConnection m:
                            parameters[i] = new MySqlParameter(keyArray[i], valueArray[i]);
                            break;
                        //case OracleConnection o:
                        // parameters[i] = new OracleParameter(keyArray[i], valueArray[i]);
                        // break;
                        default:
                            parameters[i] = new SqlParameter(keyArray[i], valueArray[i]);
                            break;
                    }
                }
                cmd.Parameters.AddRange(parameters);
            }
        }
        #endregion

        #region 通过Model反射返回结果集 Model为 Entity 泛型变量的真实类型---反射返回结果集
        public static List<Entity> GetListFromDataSet<Entity>(DataSet ds) where Entity : class
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
        public static Entity GetEntityFromDataReader<Entity>(DbDataReader reader) where Entity : class
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
        public static Entity GetEntityFromDataSet<Entity>(DataSet ds) where Entity : class
        {
            return GetListFromDataSet<Entity>(ds).FirstOrDefault();
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

    /// <summary>
    /// Auto Fill Adapter
    /// </summary>
    /// <typeparam name="Entity"></typeparam>
    internal class FillAdapter<Entity>
    {
        private static readonly Func<DataRow, Entity> funcCache = GetFactory();
        public static Entity AutoFill(DataRow row)
        {
            return funcCache(row);
        }
        private static Func<DataRow, Entity> GetFactory()
        {
            var type = typeof(Entity);
            var rowType = typeof(DataRow);
            var rowDeclare = Expression.Parameter(rowType, "row");
            var instanceDeclare = Expression.Parameter(type, "t");
            //new Student()
            var newExpression = Expression.New(type);
            //(t = new Student())
            var instanceExpression = Expression.Assign(instanceDeclare, newExpression);
            //row == null
            var nullEqualExpression = Expression.NotEqual(rowDeclare, Expression.Constant(null));
            var containsMethod = typeof(DataColumnCollection).GetMethod("Contains");
            var indexerMethod = rowType.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string) }, new[] { new ParameterModifier(1) });
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var setExpressions = new List<Expression>();
            //row.Table.Columns
            var columns = Expression.Property(Expression.Property(rowDeclare, "Table"), "Columns");
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.CanWrite)
                {
                    //Id,Id is a property of Entity
                    var propertyName = Expression.Constant(propertyInfo.Name, typeof(string));
                    //row.Table.Columns.Contains("Id")
                    var checkIfContainsColumn =Expression.Call(columns, containsMethod, propertyName);
                    //t.Id
                    var propertyExpression = Expression.Property(instanceDeclare, propertyInfo);
                    //row.get_Item("Id")
                    var value = Expression.Call(rowDeclare, indexerMethod, propertyName);
                    //t.Id = Convert(row.get_Item("Id"), Int32)
                    var propertyAssign = Expression.Assign(propertyExpression, Expression.Convert(value, propertyInfo.PropertyType));
                    //t.Id = default(Int32)
                    var propertyAssignDefault = Expression.Assign(propertyExpression, Expression.Default(propertyInfo.PropertyType));
                    //if(row.Table.Columns.Contains("Id")&&!value.Equals(DBNull.Value<>)) {t.Id = Convert(row.get_Item("Id"), Int32)}else{t.Id = default(Int32)}
                    var checkRowNull = Expression.IfThenElse(Expression.AndAlso(checkIfContainsColumn, Expression.NotEqual(value, Expression.Constant(System.DBNull.Value))), propertyAssign, propertyAssignDefault);
                    //var checkContains = Expression.IfThen(checkIfContainsColumn, propertyAssign);
                    setExpressions.Add(checkRowNull);
                }
            }
            var checkIfRowIsNull = Expression.IfThen(nullEqualExpression, Expression.Block(setExpressions));
            var body = Expression.Block(new[] { instanceDeclare }, instanceExpression, checkIfRowIsNull, instanceDeclare);
            return Expression.Lambda<Func<DataRow, Entity>>(body, rowDeclare).Compile();
        }
    }

    /**
    * author:qixiao
    * time:2017-9-18 18:02:23
    * description:safe create sqlconnection support
    * */
    internal class SqlConnection_RW : IDisposable
    {
        /// <summary>
        /// SqlConnection
        /// </summary>
        public DbConnection DbConnection { get; set; }

        public SqlConnection_RW(DataBaseType dataBaseType, string ConnString_RW)
        {
            this.DbConnection = GetDbConnection(dataBaseType, ConnString_RW);
        }
        /**
         * if read db disabled,switchover to read write db immediately
         * */
        public SqlConnection_RW(DataBaseType dataBaseType, string ConnString_R, string ConnString_RW)
        {
            try
            {
                this.DbConnection = GetDbConnection(dataBaseType, ConnString_R);
            }
            catch (Exception)
            {
                this.DbConnection = GetDbConnection(dataBaseType, ConnString_RW);
            }
        }

        /// <summary>
        /// GetDataBase ConnectionString by database type and connection string -- private use
        /// </summary>
        /// <param name="dataBaseType"></param>
        /// <param name="ConnString"></param>
        /// <returns></returns>
        private DbConnection GetDbConnection(DataBaseType dataBaseType, string ConnString)
        {
            switch (dataBaseType)
            {
                case DataBaseType.SqlServer:
                    return new SqlConnection(ConnString);
                case DataBaseType.MySql:
                    return new MySqlConnection(ConnString);
                case DataBaseType.Oracle:
                //return new OracleConnection(ConnString);
                default:
                    return new SqlConnection(ConnString);
            }
        }
        /// <summary>
        /// Must Close Connection after use
        /// </summary>
        public void Dispose()
        {
            if (this.DbConnection != null)
            {
                this.DbConnection.Dispose();
            }
        }
    }
    /// <summary>
    /// Common sqlcommand
    /// </summary>
    internal class DbCommandCommon : IDisposable
    {
        /// <summary>
        /// common dbcommand
        /// </summary>
        public DbCommand DbCommand { get; set; }
        public DbCommandCommon(DataBaseType dataBaseType)
        {
            this.DbCommand = GetDbCommand(dataBaseType);
        }

        /// <summary>
        /// Get DbCommand select database type
        /// </summary>
        /// <param name="dataBaseType"></param>
        /// <returns></returns>
        private DbCommand GetDbCommand(DataBaseType dataBaseType)
        {
            switch (dataBaseType)
            {
                case DataBaseType.SqlServer:
                    return new SqlCommand();
                case DataBaseType.MySql:
                    return new MySqlCommand();
                case DataBaseType.Oracle:
                //return new OracleCommand();
                default:
                    return new SqlCommand();
            }
        }
        /// <summary>
        /// must dispose after use
        /// </summary>
        public void Dispose()
        {
            if (this.DbCommand != null)
            {
                this.DbCommand.Dispose();
            }
        }
    }
    /// <summary>
    /// DbDataAdapterCommon
    /// </summary>
    internal class DbDataAdapterCommon : DbDataAdapter, IDisposable
    {
        public DbDataAdapter DbDataAdapter { get; set; }
        public DbDataAdapterCommon(DataBaseType dataBaseType, DbCommand dbCommand)
        {
            //get dbAdapter
            this.DbDataAdapter = GetDbAdapter(dataBaseType, dbCommand);
            //provid select command
            this.SelectCommand = dbCommand;
        }
        private DbDataAdapter GetDbAdapter(DataBaseType dataBaseType, DbCommand dbCommand)
        {
            switch (dataBaseType)
            {
                case DataBaseType.SqlServer:
                    return new SqlDataAdapter();
                case DataBaseType.MySql:
                    return new MySqlDataAdapter();
                case DataBaseType.Oracle:
                //return new OracleDataAdapter();
                default:
                    return new SqlDataAdapter();
            }
        }
        /// <summary>
        /// must dispose after use
        /// </summary>
        public new void Dispose()
        {
            if (this.DbDataAdapter != null)
            {
                this.DbDataAdapter.Dispose();
            }
        }
    }
}
