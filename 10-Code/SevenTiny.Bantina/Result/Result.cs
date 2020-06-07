using SevenTiny.Bantina.Validation;
using System;

namespace SevenTiny.Bantina
{
    /// <summary>
    /// 适合于链式写法的返回值类型，可以按照强类型的数据数目进行扩展
    /// Author:7tiny
    /// Create:2019年5月6日 16点28分
    /// </summary>
    public struct Result
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

        /// <summary>
        /// 继续一个参数校验
        /// </summary>
        /// <param name="result"></param>
        /// <param name="argument">参数</param>
        /// <param name="argumentName">参数名</param>
        /// <param name="errorMessage">自定义错误信息</param>
        /// <returns></returns>
        public static Result ContinueEnsureArgumentNotNullOrEmpty(this Result result, object argument, string argumentName, string errorMessage = null)
        {
            if (!result.IsSuccess)
                return result;

            if (FormatValidationExtension.IsNullOrEmpty(argument))
                return Result.Error(errorMessage ?? $"Parameter cannot be null or empty. Parameter name: {argumentName}");

            return result;
        }

        /// <summary>
        /// 继续执行表达式，如果抛出异常，则返回自定义错误结果（如没有则默认返回错误结果）
        /// </summary>
        /// <param name="result"></param>
        /// <param name="executor"></param>
        /// <param name="catchErrorMessage">自定义异常错误提示</param>
        /// <returns></returns>
        public static Result ContinueWithTryCatch(this Result result, Func<Result, Result> executor, Action<Exception> catchExecutor = null, string catchErrorMessage = null)
        {
            if (!result.IsSuccess)
                return result;

            try
            {
                return executor(result);
            }
            catch (Exception ex)
            {
                catchExecutor?.Invoke(ex);
                return Result.Error(catchErrorMessage ?? ex.Message);
            }
        }

        /// <summary>
        /// 转成Result_1
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="result"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result<T1> ToResult<T1>(this Result result, T1 data)
        {
            return new Result<T1>
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                MoreMessage = result.MoreMessage,
                TipType = result.TipType,
                Data = data
            };
        }
    }
}
