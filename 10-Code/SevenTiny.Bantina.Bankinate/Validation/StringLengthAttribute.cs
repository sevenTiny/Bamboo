/*********************************************************
* CopyRight: 7TINY CODE BUILDER. 
* Version: 5.0.0
* Author: 7tiny
* Address: Earth
* Create: 1/10/2019, 6:10:28 PM
* Modify: 
* E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
* GitHub: https://github.com/sevenTiny 
* Personal web site: http://www.7tiny.com 
* Technical WebSit: http://www.cnblogs.com/7tiny/ 
* Description: 
* Thx , Best Regards ~
*********************************************************/
using System;
using System.Reflection;

namespace SevenTiny.Bantina.Bankinate.Validation
{
    /// <summary>
    /// String property length limit
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class StringLengthAttribute : ValidationAttribute
    {
        internal int MinLength { get; set; } = int.MinValue;
        internal int MaxLength { get; set; }

        public StringLengthAttribute(int maxLength, string errorMsg = null) : base(errorMsg)
        {
            MaxLength = maxLength;
        }

        public StringLengthAttribute(int minLength, int maxLength, string errorMsg = null) : base(errorMsg)
        {
            MinLength = minLength;
            MaxLength = maxLength;
        }

        internal static void Verify<TEntity>(TEntity entity) where TEntity : class
        {
            foreach (var propertyInfo in typeof(TEntity).GetProperties())
            {
                if (propertyInfo.GetCustomAttribute(typeof(StringLengthAttribute), true) is StringLengthAttribute stringLength)
                {
                    if (propertyInfo.PropertyType != typeof(string))
                        throw new CustomAttributeFormatException($"'{nameof(StringLengthAttribute)}' cannot be used in '{propertyInfo.PropertyType}' type property");

                    var value = propertyInfo.GetValue(entity);

                    if (value is string strValue && (strValue?.Length > stringLength.MaxLength || strValue?.Length < stringLength.MinLength))
                        throw new ArgumentOutOfRangeException(stringLength.ErrorMessage ?? $"value of '{propertyInfo.Name}' is out of range,parameter value:{value}");
                }
            }
        }
    }
}
