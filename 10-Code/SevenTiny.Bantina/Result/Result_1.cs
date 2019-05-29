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
            => new Result<T> { IsSuccess = true, Message = message, Data = data };

        public static Result<T> Error(string message = null, T data = default(T))
            => new Result<T> { IsSuccess = false, Message = message, Data = data };
    }

    public static class Result_1_Extension
    {
        public static Result<T> Continue<T>(this Result<T> result, Func<Result<T>, Result<T>> executor)
        {
            return result.IsSuccess ? executor(result) : result;
        }
        public static Result<T> ContinueAssert<T>(this Result<T> result, bool assertResult, string errorMessage)
        {
            return result.IsSuccess ? assertResult ? result : Result<T>.Error(errorMessage) : result;
        }
    }
}
