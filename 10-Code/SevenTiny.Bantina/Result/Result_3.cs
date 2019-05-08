using System;

namespace SevenTiny.Bantina
{
    /// <summary>
    /// 适合于链式写法的返回值类型，可以按照强类型的数据数目进行扩展
    /// Author:7tiny
    /// Create:2019年5月6日 16点28分
    /// </summary>
    public class Result<T1, T2, T3> : Result<T1, T2>
    {
        public T3 Data3 { get; set; }

        public static Result<T1, T2, T3> Success(string message = null, T1 data1 = default(T1), T2 data2 = default(T2), T3 data3 = default(T3))
            => new Result<T1, T2, T3> { IsSuccess = true, Message = message, Data = data1, Data2 = data2, Data3 = data3 };

        public static Result<T1, T2, T3> Error(string message = null, T1 data1 = default(T1), T2 data2 = default(T2), T3 data3 = default(T3))
            => new Result<T1, T2, T3> { IsSuccess = false, Message = message, Data = data1, Data2 = data2, Data3 = data3 };
    }

    public static class Result_3_Extension
    {
        public static Result<T1, T2, T3> Continue<T1, T2, T3>(this Result<T1, T2, T3> result, Func<Result<T1, T2, T3>, Result<T1, T2, T3>> executor)
        {
            return result.IsSuccess ? executor(result) : result;
        }
        public static Result<T1, T2, T3> Continue<T1, T2, T3>(this Result<T1, T2, T3> result, Result<T1, T2, T3> executor)
        {
            return result.IsSuccess ? executor : result;
        }

        public static Result<T1, T2, T3> ContinueAssert<T1, T2, T3>(this Result<T1, T2, T3> result, bool assertResult, string errorMessage)
        {
            return result.IsSuccess ? assertResult ? result : Result<T1, T2, T3>.Error(errorMessage) : result;
        }
    }
}
