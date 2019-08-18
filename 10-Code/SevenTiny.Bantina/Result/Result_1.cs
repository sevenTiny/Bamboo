using System;

namespace SevenTiny.Bantina
{
    /// <summary>
    /// 适合于链式写法的返回值类型，可以按照强类型的数据数目进行扩展
    /// Author:7tiny
    /// Create:2019年5月6日 16点28分
    /// </summary>
    public class Result<T> : Result
    {
        public T Data { get; set; }

        public static Result<T> Success(string message = null, T data = default(T))
            => new Result<T> { IsSuccess = true, TipType = TipType.Success, Message = message, Data = data };

        public static Result<T> Warning(string message = null, T data = default(T))
            => new Result<T> { IsSuccess = true, TipType = TipType.Warning, Message = message, Data = data };

        public static Result<T> Info(string message = null, T data = default(T))
            => new Result<T> { IsSuccess = true, TipType = TipType.Info, Message = message, Data = data };

        public static Result<T> Error(string message = null, T data = default(T))
            => new Result<T> { IsSuccess = false, TipType = TipType.Error, Message = message, Data = data };
    }

    public static class Result_1_Extension
    {
        public static Result<T> Continue<T>(this Result<T> result, Func<Result<T>, Result<T>> executor)
        {
            return result.IsSuccess ? executor(result) : result;
        }

        /// <summary>
        /// 继续一个断言（可以用来参数校验）
        /// </summary>
        /// <param name="result"></param>
        /// <param name="assertExecutor">断言执行方法，返回断言的结果</param>
        /// <param name="errorMessage">断言返回的信息</param>
        /// <returns></returns>
        public static Result<T> ContinueAssert<T>(this Result<T> result, Func<Result<T>, bool> assertExecutor, string errorMessage)
        {
            if (!result.IsSuccess)
                return result;

            return assertExecutor(result) ? result : Result<T>.Error(errorMessage);
        }
    }
}
