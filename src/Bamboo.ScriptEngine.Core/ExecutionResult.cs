namespace Bamboo.ScriptEngine
{
    public struct ExecutionResult
    {
        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool IsSuccess { get; private set; }
        /// <summary>
        /// 执行消息
        /// </summary>
        public string Message { get; set; }

        public static ExecutionResult Error(string message = null)
        {
            return new ExecutionResult() { IsSuccess = false, Message = message };
        }
        public static ExecutionResult Success(string message = null)
        {
            return new ExecutionResult { IsSuccess = true, Message = message };
        }
    }

    public struct ExecutionResult<T>
    {
        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool IsSuccess { get; private set; }
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

        public static ExecutionResult<T> Error(string message = null)
        {
            return new ExecutionResult<T> { IsSuccess = false, Message = message };
        }
        public static ExecutionResult<T> Success(string message = null, T data = default)
        {
            return new ExecutionResult<T> { IsSuccess = true, Message = message, Data = data };
        }
    }
}
