namespace Bamboo.ScriptEngine
{
    public struct ExecutionResult<T>
    {
        /// <summary>
        /// 执行信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 返回结果
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// 执行耗时
        /// </summary>
        public long ProcessorTime { get; set; }
        /// <summary>
        /// 内存占用
        /// </summary>
        public long TotalMemoryAllocated { get; set; }

        public static ExecutionResult<T> Success(T data)
        {
            return new ExecutionResult<T> { Data = data };
        }
    }
}
