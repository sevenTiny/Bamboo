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
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SevenTiny.Bantina.Bankinate.Validation
{
    /// <summary>
    /// value data range limit,apply to IsValueType like: int,double,float,datetime,decimal...
    /// </summary>
    public class RangeLimitAttribute : ValidationAttribute
    {
        internal double MinValue { get; set; }
        internal double MaxValue { get; set; }

        public RangeLimitAttribute(double minValue = double.MinValue, double maxValue = double.MaxValue, string errorMsg = null) : base(errorMsg)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        internal static void Verify(PropertyInfo propertyInfo, object value)
        {
            if (propertyInfo.GetCustomAttribute(typeof(RangeLimitAttribute), true) is RangeLimitAttribute rangeLimit)
            {
                if (!propertyInfo.PropertyType.IsValueType)
                    throw new CustomAttributeFormatException($"'{nameof(RangeLimitAttribute)}' cannot be used in an unvaluetype property like '{propertyInfo.PropertyType}'");

                if (value == null)
                    throw new ArgumentNullException(rangeLimit.ErrorMessage ?? $"value of '{propertyInfo.Name}' can not be null");

                double val = Convert.ToDouble(value);

                if (val > rangeLimit.MaxValue || val < rangeLimit.MinValue)
                    throw new ArgumentOutOfRangeException(rangeLimit.ErrorMessage ?? $"value of '{propertyInfo.Name}' is out of range:[{rangeLimit.MinValue},{rangeLimit.MaxValue}]，parameter value:{value}");
            }
        }
    }
}
