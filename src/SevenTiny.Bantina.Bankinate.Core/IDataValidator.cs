using System.Collections.Generic;

namespace SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// 数据校验接口
    /// </summary>
    public interface IDataValidator
    {
        void Verify<TEntity>(TEntity entity) where TEntity : class;
        void Verify<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
    }
}
