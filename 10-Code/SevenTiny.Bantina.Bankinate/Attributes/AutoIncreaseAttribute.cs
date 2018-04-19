using System;

namespace SevenTiny.Bantina.Bankinate
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AutoIncreaseAttribute : Attribute
    {
    }
}
