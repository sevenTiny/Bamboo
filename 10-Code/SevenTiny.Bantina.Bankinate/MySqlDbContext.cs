/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-04-19 23:57:48
 * Modify: 2018-04-19 23:57:48
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using SevenTiny.Bantina.Bankinate.DataAccessEngine;
using SevenTiny.Bantina.Bankinate.DbContexts;

namespace SevenTiny.Bantina.Bankinate
{
    public abstract class MySqlDbContext<TDataBase> : SqlDbContext<TDataBase> where TDataBase : class
    {
        protected MySqlDbContext(string connectionString) : this(connectionString, connectionString) { }
        protected MySqlDbContext(string connectionString_Read, string connectionString_ReadWrite) : base(connectionString_Read, connectionString_ReadWrite, DataBaseType.MySql) { }
    }
}