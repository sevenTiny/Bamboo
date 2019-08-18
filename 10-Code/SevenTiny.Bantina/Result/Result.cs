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
        public bool IsSuccess { get; internal set; }
        public string Message { get; set; }
        public string MoreMessage { get; set; }
        public TipType TipType { get; internal set; }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Result Success(string message = null)
            => new Result { IsSuccess = true, TipType = TipType.Success, Message = message };

        /// <summary>
        /// 警告，【应该放在流程最后一步】
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Result Warning(string message = null)
            => new Result { IsSuccess = true, TipType = TipType.Warning, Message = message };

        /// <summary>
        /// 提示信息，【应该放在流程最后一步】
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Result Info(string message = null)
            => new Result { IsSuccess = true, TipType = TipType.Info, Message = message };

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Result Error(string message = null)
            => new Result { IsSuccess = false, TipType = TipType.Error, Message = message };
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
        /// <param name="assertExecutor">断言执行方法，返回断言的结果</param>
        /// <param name="errorMessage">断言返回的信息</param>
        /// <returns></returns>
        public static Result ContinueAssert(this Result result, Func<Result, bool> assertExecutor, string errorMessage)
        {
            if (!result.IsSuccess)
                return result;

            return assertExecutor(result) ? result : Result.Error(errorMessage);
        }
    }
}
