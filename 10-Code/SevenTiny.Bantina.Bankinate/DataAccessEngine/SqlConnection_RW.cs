/*********************************************************
* CopyRight: 7TINY CODE BUILDER. 
* Version: 5.0.0
* Author: 7tiny
* Address: Earth
* Create: 1/7/2019, 3:42:48 PM
* Modify: 
* E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
* GitHub: https://github.com/sevenTiny 
* Personal web site: http://www.7tiny.com 
* Technical WebSit: http://www.cnblogs.com/7tiny/ 
* Description: 
* Thx , Best Regards ~
*********************************************************/
using MySql.Data.MySqlClient;
using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace SevenTiny.Bantina.Bankinate.DataAccessEngine
{
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
}
