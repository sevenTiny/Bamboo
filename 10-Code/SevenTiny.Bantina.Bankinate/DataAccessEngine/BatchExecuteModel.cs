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
using System.Collections.Generic;
using System.Data;

namespace SevenTiny.Bantina.Bankinate.DataAccessEngine
{
    /// <summary>
    /// 用于批量操作的批量操作实体
    /// </summary>
    public class BatchExecuteModel
    {
        /// <summary>
        /// 执行的语句或者存储过程名称
        /// </summary>
        public string CommandTextOrSpName { get; set; }
        /// <summary>
        /// 执行类别，默认执行sql语句
        /// </summary>
        public CommandType CommandType { get; set; } = CommandType.Text;
        /// <summary>
        /// 执行语句的参数字典
        /// </summary>
        public IDictionary<string, object> ParamsDic { get; set; }
    }
}
