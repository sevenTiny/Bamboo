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
using SevenTiny.Bantina.Bankinate.DbContexts;

namespace SevenTiny.Bantina.Bankinate.Validation
{
    /// <summary>
    /// 字段值合法性校验器
    /// </summary>
    internal class PropertyDataValidator
    {
        public static void Verify<TEntity>(DbContext context, TEntity entity) where TEntity : class
        {
            //判断是否开启字段值校验
            if (!context.OpenPropertyDataValidate)
                return;

            if (entity == null)
                return;

            foreach (var propertyInfo in typeof(TEntity).GetProperties())
            {
                var value = propertyInfo.GetValue(entity);

                //Require
                RequireAttribute.Verify(propertyInfo,value);

                //StringLength
                StringLengthAttribute.Verify(propertyInfo, value);

                //RangeLimit
                RangeLimitAttribute.Verify(propertyInfo, value);
            }
        }
    }
}
