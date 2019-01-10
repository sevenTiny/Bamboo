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
    /// 是否必填
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequireAttribute : ValidationAttribute
    {
        public RequireAttribute(string errorMsg = null) : base(errorMsg) { }

        internal static void Verify(PropertyInfo propertyInfo, object value)
        {
            if (propertyInfo.GetCustomAttribute(typeof(RequireAttribute), true) is RequireAttribute require)
            {
                if (value == null)
                    throw new ArgumentNullException(require.ErrorMessage ?? $"value of '{propertyInfo.Name}' can not be null");
            }
        }
    }
}
