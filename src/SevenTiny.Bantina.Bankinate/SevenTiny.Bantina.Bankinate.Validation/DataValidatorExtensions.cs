/*********************************************************
* CopyRight: 7TINY CODE BUILDER. 
* Version: 5.0.0
* Author: 7tiny
* Address: Earth
* Create: 12/10/2019, 20:27:28 PM
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
    /// 缓存管理器扩展
    /// </summary>
    public static class DataValidatorExtensions
    {
        /// <summary>
        /// 初始化数据校验器
        /// </summary>
        /// <param name="dbContext"></param>
        public static void OpenDataValidation(this DbContext dbContext)
        {
            dbContext.OpenDataValidation(new DataValidator());
        }
    }
}
