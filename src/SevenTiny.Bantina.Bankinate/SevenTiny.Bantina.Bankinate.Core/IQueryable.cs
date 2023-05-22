using System.Collections.Generic;
using System.Data;

namespace SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// Sql查询支持
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IQueryable<TEntity>
    {
        /// <summary>
        /// 查询符合当前条件的结果并转换成object类型，具体类型按不同场景单独维护
        /// </summary>
        /// <returns></returns>
        object ToData();
        /// <summary>
        /// 查询符合当前条件的结果并转换成DataSet
        /// </summary>
        /// <returns></returns>
        DataSet ToDataSet();
        /// <summary>
        /// 查询符合当前条件的结果并转换成泛型集合
        /// </summary>
        /// <returns></returns>
        List<TEntity> ToList();
        /// <summary>
        /// 查询符合当前条件的结果并转换成单个实体
        /// </summary>
        /// <returns></returns>
        TEntity FirstOrDefault();
    }
}
