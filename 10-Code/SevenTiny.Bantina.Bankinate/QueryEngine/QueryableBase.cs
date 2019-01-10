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
using SevenTiny.Bantina.Bankinate.SqlStatementManager;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// SqlQueryable的相关配置信息
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class QueryableBase<TEntity> where TEntity : class
    {
        public QueryableBase(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //context
        protected DbContext _dbContext;

        //where
        protected Expression<Func<TEntity, bool>> _where;

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
        protected List<string> _columns;
        /// <summary>
        /// 查询几条
        /// </summary>
        protected string _top;

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

        /// <summary>
        /// 查询生成符合当前条件的List结果集
        /// </summary>
        /// <returns></returns>
        public abstract List<TEntity> ToList();
        /// <summary>
        /// 查询生成符合当前条件的单个实体
        /// </summary>
        /// <returns></returns>
        public abstract TEntity ToEntity();
        /// <summary>
        /// 查询出符合当前条件的数据条数
        /// </summary>
        /// <returns></returns>
        public abstract int ToCount();
    }
}
