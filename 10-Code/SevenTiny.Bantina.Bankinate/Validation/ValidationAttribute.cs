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

namespace SevenTiny.Bantina.Bankinate.Validation
{
    /// <summary>
    /// 校验标签的基类
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class ValidationAttribute : Attribute
    {
        internal string ErrorMessage { get; set; }

        public ValidationAttribute(string errorMsg)
        {
            ErrorMessage = errorMsg;
        }
    }
}
