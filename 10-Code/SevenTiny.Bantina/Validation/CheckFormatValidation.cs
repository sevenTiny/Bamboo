/*********************************************************
 * CopyRight: QIXIAO CODE BUILDER. 
 * Version:4.2.0
 * Author:qixiao(柒小)
 * Create:2017-09-14 15:51:02
 * Update:2017-09-14 15:51:02
 * E-mail: dong@qixiao.me | wd8622088@foxmail.com 
 * GitHub: https://github.com/dong666 
 * Personal web site: http://qixiao.me 
 * Technical WebSit: http://www.cnblogs.com/qixiaoyizhan/ 
 * Description:
 * Thx , Best Regards ~
 *********************************************************/


using System;

namespace SevenTiny.Bantina.Validation
{
    public static class CheckFormatValidation
    {
        /// <summary>
        /// Check match regex string
        /// </summary>
        /// <param name="data"></param>
        /// <param name="regexString"></param>
        /// <param name="errorMessage"></param>
        public static void CheckXXX_ByRegex(this string data, string regexString, string errorMessage)
        {
            if (!data.IsXXX_ByRegex(regexString)) { throw new Exception(errorMessage); }
        }
        public static void CheckEmail(this string data, string errorMessage)
        {
            if (!data.IsEmail()) { throw new Exception(errorMessage); }
        }
        public static void CheckMobilePhone(this string data, string errorMessage)
        {
            if (!data.IsMobilePhone()) { throw new Exception(errorMessage); }
        }
        public static void CheckTelPhone(this string data, string errorMessage)
        {
            if (!data.IsTelPhone()) { throw new Exception(errorMessage); }
        }
        public static void CheckURL(this string data, string errorMessage)
        {
            if (!data.IsURL()) { throw new Exception(errorMessage); }
        }
        public static void CheckIpAddress(this string data, string errorMessage)
        {
            if (!data.IsIpAddress()) { throw new Exception(errorMessage); }
        }
        public static void CheckID_Card(this string data, string errorMessage)
        {
            if (!data.IsID_Card()) { throw new Exception(errorMessage); }
        }
        //(字母开头，允许5-16字节，允许字母数字下划线)
        public static void CheckAccountName(this string data, string errorMessage)
        {
            if (!data.IsAccountName()) { throw new Exception(errorMessage); }
        }
        //(以字母开头，长度在6~18之间，只能包含字母、数字和下划线)
        public static void CheckPassword(this string data, string errorMessage)
        {
            if (!data.IsPassword()) { throw new Exception(errorMessage); }
        }
        //(必须包含大小写字母和数字的组合，不能使用特殊字符，长度在8-10之间)
        public static void CheckStrongCipher(this string data, string errorMessage)
        {
            if (!data.IsStrongCipher()) { throw new Exception(errorMessage); }
        }
        //数字和字母
        public static void CheckAlnum(this string data, int minLength, int maxLength, string errorMessage)
        {
            if (!data.IsAlnum(minLength, maxLength)) { throw new Exception(errorMessage); }
        }

        public static void CheckDataFormat(this string data, string errorMessage)
        {
            if (!data.IsDataFormat()) { throw new Exception(errorMessage); }
        }
        public static void CheckChineseCharactor(this string data, string errorMessage)
        {
            if (!data.IsChineseCharactor()) { throw new Exception(errorMessage); }
        }
        public static void CheckQQ_Number(this string data, string errorMessage)
        {
            if (!data.IsQQ_Number()) { throw new Exception(errorMessage); }
        }
        public static void CheckPostalCode(this string data, string errorMessage)
        {
            if (!data.IsPostalCode()) { throw new Exception(errorMessage); }
        }
    }
}
