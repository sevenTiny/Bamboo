using System;

namespace SevenTiny.Bantina.Bankinate.Core.Exceptions
{
    /// <summary>
    /// 未知的数据库
    /// </summary>
    public class UnknownDataBaseTypeException : Exception
    {
        public UnknownDataBaseTypeException(string message) : base(message)
        {
        }
    }
}
