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
}
