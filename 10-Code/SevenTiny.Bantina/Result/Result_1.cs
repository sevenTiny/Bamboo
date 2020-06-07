using SevenTiny.Bantina.Validation;
using System;

namespace SevenTiny.Bantina
{
    /// <summary>
    /// 适合于链式写法的返回值类型，可以按照强类型的数据数目进行扩展
    /// Author:7tiny
    /// Create:2019年5月6日 16点28分
    /// </summary>
    public struct Result<T>
    {
        public bool IsSuccess { get; internal set; }
        public string Message { get; set; }
        public string MoreMessage { get; set; }
        public TipType TipType { get; internal set; }

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

        /// <summary>
        /// 继续一个参数校验
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="result"></param>
        /// <param name="argument"></param>
        /// <param name="argumentName"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static Result<T1> ContinueEnsureArgumentNotNullOrEmpty<T1>(this Result<T1> result, object argument, string argumentName, string errorMessage = null)
        {
            if (!result.IsSuccess)
                return result;

            if (FormatValidationExtension.IsNullOrEmpty(argument))
                return Result<T1>.Error(errorMessage ?? $"Parameter cannot be null or empty. Parameter name: {argumentName}");

            return result;
        }

        /// <summary>
        /// 继续执行表达式，如果抛出异常，则返回自定义错误结果（如没有则默认返回错误结果）
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="result"></param>
        /// <param name="executor"></param>
        /// <param name="catchErrorMessage"></param>
        /// <returns></returns>
        public static Result<T1> ContinueWithTryCatch<T1>(this Result<T1> result, Func<Result<T1>, Result<T1>> executor, Action<Exception> catchExecutor = null, string catchErrorMessage = null)
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
                return Result<T1>.Error(catchErrorMessage ?? ex.Message);
            }
        }

        /// <summary>
        /// 转成Result
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Result ToResult<T1>(this Result<T1> result)
        {
            return new Result
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                MoreMessage = result.MoreMessage,
                TipType = result.TipType
            };
        }
    }
}
