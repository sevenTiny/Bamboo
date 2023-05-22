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
using SevenTiny.Bantina.Bankinate.Core.Exceptions;

namespace SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DataBaseType
    {
        UnKnown,
        SqlServer,
        MySql,
        //Oracle,
        MongoDB
    }

    /// <summary>
    /// 数据库分类
    /// </summary>
    internal enum DataBaseCategory
    {
        /// <summary>
        /// 关系型数据库
        /// </summary>
        Relational,
        /// <summary>
        /// 非关系型数据库
        /// </summary>
        NonRelational
    }
    
    /// <summary>
    /// 筛选各种数据库的分类
    /// </summary>
    internal static class DataBaseCategoryFilter
    {
        /// <summary>
        /// 通过数据库获取数据库分类
        /// </summary>
        /// <param name="dataBaseType"></param>
        /// <returns></returns>
        internal static DataBaseCategory GetCategory(this DataBaseType dataBaseType)
        {
            switch (dataBaseType)
            {
                case DataBaseType.UnKnown:
                    break;
                case DataBaseType.SqlServer:
                    return DataBaseCategory.Relational;
                case DataBaseType.MySql:
                    return DataBaseCategory.Relational;
                case DataBaseType.MongoDB:
                    return DataBaseCategory.NonRelational;
                default:
                    break;
            }
            throw new UnknownDataBaseTypeException("Please select the correct database.");
        }
    }
}
