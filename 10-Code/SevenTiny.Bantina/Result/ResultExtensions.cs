namespace SevenTiny.Bantina
{
    public static class ResultExtensions
    {
        internal static Result CopyCommonProperties(this Result self, Result from)
        {
            self.IsSuccess = from.IsSuccess;
            self.Message = from.Message;
            self.MoreMessage = from.MoreMessage;
            self.TipType = from.TipType;
            return self;
        }
        internal static Result<T> CopyCommonProperties<T>(this Result<T> self, Result from)
        {
            return (self as Result).CopyCommonProperties(from) as Result<T>;
        }
        internal static Result<T1, T2> CopyCommonProperties<T1, T2>(this Result<T1, T2> self, Result from)
        {
            return (self as Result).CopyCommonProperties(from) as Result<T1, T2>;
        }
        internal static Result<T1, T2, T3> CopyCommonProperties<T1, T2, T3>(this Result<T1, T2, T3> self, Result from)
        {
            return (self as Result).CopyCommonProperties(from) as Result<T1, T2, T3>;
        }

        #region Result
        public static Result<T> AsResult<T>(Result result, T t = default(T))
        {
            return new Result<T>
            {
                Data = t
            }
            .CopyCommonProperties(result);
        }

        public static Result<T1, T2> AsResult<T1, T2>(Result result, T1 t1 = default(T1), T2 t2 = default(T2))
        {
            return new Result<T1, T2>
            {
                Data = t1,
                Data2 = t2
            }
            .CopyCommonProperties(result);
        }

        public static Result<T1, T2, T3> AsResult<T1, T2, T3>(Result result, T1 t1 = default(T1), T2 t2 = default(T2), T3 t3 = default(T3))
        {
            return new Result<T1, T2, T3>
            {
                Data = t1,
                Data2 = t2,
                Data3 = t3
            }
            .CopyCommonProperties(result);
        }
        #endregion

        #region Result<T>
        public static Result<T1, T2> AsResult<T1, T2>(Result<T1> result, T2 t2 = default(T2))
        {
            return new Result<T1, T2>
            {
                Data = result.Data,
                Data2 = t2
            }
            .CopyCommonProperties(result);
        }

        public static Result<T1, T2, T3> AsResult<T1, T2, T3>(Result<T1> result, T2 t2 = default(T2), T3 t3 = default(T3))
        {
            return new Result<T1, T2, T3>
            {
                Data = result.Data,
                Data2 = t2,
                Data3 = t3
            }
            .CopyCommonProperties(result);
        }
        #endregion

        #region Result<T1,T2>
        public static Result<T1, T2, T3> AsResult<T1, T2, T3>(Result<T1,T2> result, T3 t3 = default(T3))
        {
            return new Result<T1, T2, T3>
            {
                Data = result.Data,
                Data2 = result.Data2,
                Data3 = t3
            }
            .CopyCommonProperties(result);
        }
        #endregion
    }
}
