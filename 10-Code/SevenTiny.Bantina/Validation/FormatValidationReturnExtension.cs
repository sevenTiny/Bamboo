/*********************************************************
* CopyRight: 7TINY CODE BUILDER. 
* Version: 5.0.0
* Author: 7tiny
* Address: Earth
* Create: 5/8/2019, 2:36:24 PM
* Modify: 
* E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
* GitHub: https://github.com/sevenTiny 
* Personal web site: http://www.7tiny.com 
* Technical WebSit: http://www.cnblogs.com/7tiny/ 
* Description: 
* Thx , Best Regards ~
*********************************************************/

using System;

namespace SevenTiny.Bantina.Validation
{
    public static class FormatValidationReturnExtension
    {
        public static object CheckNull(this object data, string errorMessage)
        => data != null ? data : throw new Exception(errorMessage);
        public static string CheckNullOrEmpty(this string data, string errorMessage)
        => !data.IsNullOrEmpty() ? data : throw new Exception(errorMessage);
        public static string CheckNullOrWhiteSpace(this string data, string errorMessage)
        => !data.IsNullOrWhiteSpace() ? data : throw new Exception(errorMessage);

        /// <summary>
        /// Check match regex string
        /// </summary>
        /// <param name="data"></param>
        /// <param name="regexString"></param>
        /// <param name="errorMessage"></param>
        public static string CheckXXX_ByRegex(this string data, string regexString, string errorMessage)
        => data.IsXXX_ByRegex(regexString) ? data : throw new Exception(errorMessage);

        public static string CheckEmail(this string data, string errorMessage)
        => data.IsEmail() ? data : throw new Exception(errorMessage);

        public static string CheckMobilePhone(this string data, string errorMessage)
        => data.IsMobilePhone() ? data : throw new Exception(errorMessage);

        public static string CheckTelPhone(this string data, string errorMessage)
        => data.IsTelPhone() ? data : throw new Exception(errorMessage);

        public static string CheckURL(this string data, string errorMessage)
        => data.IsURL() ? data : throw new Exception(errorMessage);

        public static string CheckIpAddress(this string data, string errorMessage)
        => data.IsIpAddress() ? data : throw new Exception(errorMessage);

        public static string CheckID_Card(this string data, string errorMessage)
        => data.IsID_Card() ? data : throw new Exception(errorMessage);

        //(字母开头，允许5-16字节，允许字母数字下划线)
        public static string CheckAccountName(this string data, string errorMessage)
        => data.IsAccountName() ? data : throw new Exception(errorMessage);

        //(以字母开头，长度在6~18之间，只能包含字母、数字和下划线)
        public static string CheckPassword(this string data, string errorMessage)
        => data.IsPassword() ? data : throw new Exception(errorMessage);

        //(必须包含大小写字母和数字的组合，不能使用特殊字符，长度在8-10之间)
        public static string CheckStrongCipher(this string data, string errorMessage)
        => data.IsStrongCipher() ? data : throw new Exception(errorMessage);

        //数字和字母
        public static string CheckAlnum(this string data, int minLength, int maxLength, string errorMessage)
        => data.IsAlnum(minLength, maxLength) ? data : throw new Exception(errorMessage);

        public static string CheckDataFormat(this string data, string errorMessage)
        => data.IsDataFormat() ? data : throw new Exception(errorMessage);

        public static string CheckChineseCharactor(this string data, string errorMessage)
        => data.IsChineseCharactor() ? data : throw new Exception(errorMessage);

        public static string CheckQQ_Number(this string data, string errorMessage)
        => data.IsQQ_Number() ? data : throw new Exception(errorMessage);

        public static string CheckPostalCode(this string data, string errorMessage)
        => data.IsPostalCode() ? data : throw new Exception(errorMessage);
    }
}
