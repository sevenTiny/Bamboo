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
