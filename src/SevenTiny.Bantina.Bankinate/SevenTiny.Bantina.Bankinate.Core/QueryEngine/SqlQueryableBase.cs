/*********************************************************
* CopyRight: 7TINY CODE BUILDER. 
* Version: 5.0.0
* Author: 7tiny
* Address: Earth
* Create: 1/8/2019, 5:31:04 PM
* Modify: 
* E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
* GitHub: https://github.com/sevenTiny 
* Personal web site: http://www.7tiny.com 
* Technical WebSit: http://www.cnblogs.com/7tiny/ 
* Description: 懒加载查询配置基类
* Thx , Best Regards ~
*********************************************************/
using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// SqlQueryable的相关配置信息
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal abstract class SqlQueryableBase<TEntity> where TEntity : class
    {
        public SqlQueryableBase(SqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //context
        protected SqlDbContext _dbContext;

        //query info
        public string SqlStatement => _dbContext.DbCommand.CommandText;
        public string TableName => _dbContext.TableName;
        public IDictionary<string, object> Parameters => _dbContext.Parameters;

        //where
        protected Expression<Func<TEntity, bool>> _where = t => true;

        //orderby
        protected Expression<Func<TEntity, object>> _orderby;
        protected bool _isDesc = false;

        //paging
        protected bool _isPaging = false;
        protected int _pageIndex = 0;
        protected int _pageSize = 0;

        /// <summary>
        /// 要查询的列
        /// </summary>
        protected Expression<Func<TEntity, object>> _columns;

        /// <summary>
        /// 必要条件检查
        /// </summary>
        protected void MustExistCheck()
        {
            if (_where == null)
            {
                throw new ArgumentNullException("Where condition deficiency");
            }
        }

        /// <summary>
        /// 获取TableName，并将其重新赋值
        /// </summary>
        protected void ReSetTableName()
        {
            _dbContext.TableName = TableAttribute.GetName(typeof(TEntity));
        }
    }
}
