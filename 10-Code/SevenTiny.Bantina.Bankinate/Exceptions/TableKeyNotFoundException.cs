using System;

namespace SevenTiny.Bantina.Bankinate.Exceptions
{
    public class TableKeyNotFoundException : Exception
    {
        public TableKeyNotFoundException(string message) : base(message)
        {
        }
    }
}
