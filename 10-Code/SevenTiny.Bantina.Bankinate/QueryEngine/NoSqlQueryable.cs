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
* Description: 适合于NoSql条件下的懒加载查询配置
* Thx , Best Regards ~
*********************************************************/
using SevenTiny.Bantina.Bankinate.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate
{
    public class NoSqlQueryable<TEntity> : QueryableBase<TEntity> where TEntity : class
    {
        public NoSqlQueryable(DbContext dbContext) : base(dbContext) { }

        public override QueryableBase<TEntity> Where(Expression<Func<TEntity, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public override int ToCount()
        {
            throw new NotImplementedException();
        }

        public override TEntity ToEntity()
        {
            throw new NotImplementedException();
        }

        public override List<TEntity> ToList()
        {
            throw new NotImplementedException();
        }
    }
}
