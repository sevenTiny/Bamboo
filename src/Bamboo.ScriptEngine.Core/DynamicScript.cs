namespace Bamboo.ScriptEngine
{
    /// <summary>
    /// 动态脚本对象
    /// </summary>
    public class DynamicScript
    {
        /// <summary>
        /// 脚本内容
        /// </summary>
        public string Script { get; set; }
        /// <summary>
        /// 类的最全限定名
        /// python和go等语言可以写脚本名
        /// </summary>
        public string ClassFullName { get; set; }
        /// <summary>
        /// 函数名
        /// </summary>
        public string FunctionName { get; set; }
        /// <summary>
        /// 执行参数
        /// </summary>
        public object[] Parameters { get; set; }
        /// <summary>
        /// 脚本语言
        /// </summary>
        public DynamicScriptLanguage Language { get; set; }
        /// <summary>
        /// 是否收集执行统计信息
        /// 默认False：统计非常耗时且会带来更多开销，正常运行过程请关闭！
        /// </summary>
        public bool CollectExecutionStatistics { get; set; } = false;
        /// <summary>
        /// 是否在沙箱中执行脚本（默认否）
        /// 在沙箱环境中执行脚本会带来更高的安全性，但可能会增加执行时间和资源消耗，建议在执行不受信任的脚本时启用此选项。
        /// 需要额外部署沙箱运行时
        /// </summary>
        public bool IsExecutionInSandbox { get; set; } = false;
        /// <summary>
        /// 沙箱中执行脚本的超时时间（-1表示不限制，默认-1）
        /// </summary>
        public int ExecutionTimeoutMilliseconds { get; set; } = -1;
    }
}
