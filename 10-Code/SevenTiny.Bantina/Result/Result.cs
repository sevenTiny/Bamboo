using System;

namespace SevenTiny.Bantina
{
    /// <summary>
    /// 适合于链式写法的返回值类型，可以按照强类型的数据数目进行扩展
    /// Author:7tiny
    /// Create:2019年5月6日 16点28分
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public static Result Success(string message = null)
            => new Result { IsSuccess = true, Message = message };

        public static Result Error(string message = null)
            => new Result { IsSuccess = false, Message = message };
    }

    public static class ResultExtension
    {
        public static Result Continue(this Result result, Func<Result, Result> executor)
        {
            return result.IsSuccess ? executor(result) : result;
        }
        /// <summary>
        /// 继续一个断言（可以用来参数校验）
        /// </summary>
        /// <param name="result"></param>
        /// <param name="assertResult">断言，返回断言的结果</param>
        /// <param name="errorMessage">断言返回的信息</param>
        /// <returns></returns>
        public static Result ContinueAssert(this Result result, bool assertResult, string errorMessage)
        {
            return result.IsSuccess ? assertResult ? result : Result.Error(errorMessage) : result;
        }
    }
}
