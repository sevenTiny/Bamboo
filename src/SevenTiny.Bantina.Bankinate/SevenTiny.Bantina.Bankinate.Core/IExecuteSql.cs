using System.Collections.Generic;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// 执行sql语句扩展Api
    /// </summary>
    public interface IExecuteSql
    {
        int ExecuteSql(string sqlStatement, IDictionary<string, object> parms = null);
        Task<int> ExecuteSqlAsync(string sqlStatement, IDictionary<string, object> parms = null);
        int ExecuteStoredProcedure(string storedProcedureName, IDictionary<string, object> parms = null);
        Task<int> ExecuteStoredProcedureAsync(string storedProcedureName, IDictionary<string, object> parms = null);
    }
}
