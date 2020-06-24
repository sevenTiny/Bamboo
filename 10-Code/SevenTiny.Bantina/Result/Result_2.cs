using SevenTiny.Bantina.Validation;
using System;
using System.Collections.Generic;

namespace SevenTiny.Bantina
{
    /// <summary>
    /// 适合于链式写法的返回值类型，可以按照强类型的数据数目进行扩展
    /// Author:7tiny
    /// Create:2019年5月6日 16点28分
    /// </summary>
    public struct Result<T1, T2>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<string> MoreMessage { get; set; }
        public TipType TipType { get; set; }

        public T1 Data { get; set; }
        public T2 Data2 { get; set; }

        public static Result<T1, T2> Success(string message = null, T1 data1 = default(T1), T2 data2 = default(T2))
            => new Result<T1, T2> { IsSuccess = true, TipType = TipType.Success, Message = message, Data = data1, Data2 = data2 };

        public static Result<T1, T2> Warning(string message = null, T1 data1 = default(T1), T2 data2 = default(T2))
            => new Result<T1, T2> { IsSuccess = true, TipType = TipType.Warning, Message = message, Data = data1, Data2 = data2 };

        public static Result<T1, T2> Info(string message = null, T1 data1 = default(T1), T2 data2 = default(T2))
            => new Result<T1, T2> { IsSuccess = true, TipType = TipType.Info, Message = message, Data = data1, Data2 = data2 };

        public static Result<T1, T2> Error(string message = null, T1 data1 = default(T1), T2 data2 = default(T2))
            => new Result<T1, T2> { IsSuccess = false, TipType = TipType.Error, Message = message, Data = data1, Data2 = data2 };
    }

    public static class Result_2_Extension
    {
        public static Result<T1, T2> Continue<T1, T2>(this Result<T1, T2> result, Func<Result<T1, T2>, Result<T1, T2>> executor)
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
        public static Result<T1, T2> ContinueAssert<T1, T2>(this Result<T1, T2> result, Func<Result<T1, T2>, bool> assertExecutor, string errorMessage)
        {
            if (!result.IsSuccess)
                return result;

            return assertExecutor(result) ? result : Result<T1, T2>.Error(errorMessage);
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
        public static Result<T1, T2> ContinueEnsureArgumentNotNullOrEmpty<T1, T2>(this Result<T1, T2> result, object argument, string argumentName, string errorMessage = null)
        {
            if (!result.IsSuccess)
                return result;

            if (FormatValidationExtension.IsNullOrEmpty(argument))
                return Result<T1, T2>.Error(errorMessage ?? $"Parameter cannot be null or empty. Parameter name: {argumentName}");

            return result;
        }

        /// <summary>
        /// 继续执行表达式，如果抛出异常，则返回自定义错误结果（如没有则默认返回错误结果）
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="result"></param>
        /// <param name="executor"></param>
        /// <param name="catchErrorMessage"></param>
        /// <returns></returns>
        public static Result<T1, T2> ContinueWithTryCatch<T1, T2>(this Result<T1, T2> result, Func<Result<T1, T2>, Result<T1, T2>> executor, Action<Exception> catchExecutor = null, string catchErrorMessage = null)
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
                return Result<T1, T2>.Error(catchErrorMessage ?? ex.Message);
            }
        }
    }
}
