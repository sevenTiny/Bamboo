/*********************************************************
* CopyRight: 7TINY CODE BUILDER. 
* Version: 5.0.0
* Author: 7tiny
* Address: Earth
* Create: 1/10/2019, 6:10:28 PM
* Modify: 
* E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
* GitHub: https://github.com/sevenTiny 
* Personal web site: http://www.7tiny.com 
* Technical WebSit: http://www.cnblogs.com/7tiny/ 
* Description: 
* Thx , Best Regards ~
*********************************************************/
using System.Collections.Generic;
using System.Linq;

namespace SevenTiny.Bantina.Bankinate.Validation
{
    /// <summary>
    /// 字段值合法性校验器
    /// </summary>
    public class DataValidator : IDataValidator
    {
        public void Verify<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
                return;

            foreach (var propertyInfo in typeof(TEntity).GetProperties())
            {
                var value = propertyInfo.GetValue(entity);

                //Require
                RequireAttribute.Verify(propertyInfo, value);

                //StringLength
                StringLengthAttribute.Verify(propertyInfo, value);

                //RangeLimit
                RangeLimitAttribute.Verify(propertyInfo, value);
            }
        }

        public void Verify<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            if (entities == null || !entities.Any())
                return;

            foreach (var propertyInfo in typeof(TEntity).GetProperties())
            {
                foreach (var item in entities)
                {
                    var value = propertyInfo.GetValue(item);

                    //Require
                    RequireAttribute.Verify(propertyInfo, value);

                    //StringLength
                    StringLengthAttribute.Verify(propertyInfo, value);

                    //RangeLimit
                    RangeLimitAttribute.Verify(propertyInfo, value);
                }
            }
        }
    }
}
