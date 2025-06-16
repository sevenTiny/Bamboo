/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-15 17:40:42
 * Modify: 2025年6月16日 22点39分
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SevenTiny.Bantina.Validation
{
    public static class ParameterValidationHelper
    {
        public static bool IsNull(object data) => data == null;
        public static bool IsNullOrWhiteSpace(string data) => string.IsNullOrWhiteSpace(data);
        public static bool IsNullOrEmpty(object arg)
        {
            bool notValid = arg == null;

            switch (arg)
            {
                case string b when string.IsNullOrEmpty(b):
                case Guid guid when guid == Guid.Empty:
                case int @int when @int == 0:
                case double @double when @double == 0:
                case float @float when @float == 0:
                case long @long when @long == 0:
                case decimal @decimal when @decimal == 0:
                case IEnumerable<int> c when !c.Any():
                case IEnumerable<float> d when !d.Any():
                case IEnumerable<double> e when !e.Any():
                case IEnumerable<long> @longArrary when !@longArrary.Any():
                case IEnumerable<object> enumerable when !enumerable.Any():
                    notValid = true;
                    break;
            }

            return notValid;
        }

        /// <summary>
        /// check match regex string
        /// </summary>
        /// <param name="data"></param>
        /// <param name="regexString"></param>
        /// <returns></returns>
        public static bool IsValidByRegex(string data, string regexString)
        {
            return Regex.IsMatch(data, regexString);
        }
        public static bool IsEmail(string data)
        {
            return Regex.IsMatch(data, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        }
        public static bool IsMobilePhone(string data)
        {
            return Regex.IsMatch(data, @"^(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0|1|2|3|5|6|7|8|9])\d{8}$");
        }
        public static bool IsTelPhone(string data)
        {
            return Regex.IsMatch(data, @"^(\(\d{3,4}-)|\d{3.4}-)?\d{7,8}$");
        }
        public static bool IsURL(string data)
        {
            return Regex.IsMatch(data, @"^((https?|ftp|news):\/\/)?([a-z]([a-z0-9\-]*[\.。])+([a-z]{2}|aero|arpa|biz|com|coop|edu|gov|info|int|jobs|mil|museum|name|nato|net|org|pro|travel)|(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]))(\/[a-z0-9_\-\.~]+)*(\/([a-z0-9_\-\.]*)(\?[a-z0-9+_\-\.%=&]*)?)?(#[a-z][a-z0-9_]*)?$");
        }
        public static bool IsIpAddress(string data)
        {
            return Regex.IsMatch(data, @"^(1\d{2}|2[0-4]\d|25[0-5]|[1-9]\d|[1-9])\.(1\d{2}|2[0-4]\d|25[0-5]|[1-9]\d|\d)\.(1\d{2}|2[0-4]\d|25[0-5]|[1-9]\d|\d)\.(1\d{2}|2[0-4]\d|25[0-5]|[1-9]\d|\d)$");
        }
        public static bool IsID_Card(string data)
        {
            return Regex.IsMatch(data, @"^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$");
        }
        //(字母开头，允许5-16字节，允许字母数字下划线)
        public static bool IsAccountName(string data)
        {
            return Regex.IsMatch(data, @"^[a-zA-Z][a-zA-Z0-9_]{4,15}$");
        }
        //(以字母开头，长度在6~18之间，只能包含字母、数字和下划线)
        public static bool IsPassword(string data)
        {
            return Regex.IsMatch(data, @"^[a-zA-Z]\w{5,17}$");
        }
        /// <summary>
        /// 字母和数字
        /// </summary>
        /// <param name="data"></param>
        /// <param name="minLength">最小长度</param>
        /// <param name="maxLength">最大长度</param>
        /// <returns></returns>
        public static bool IsAlnum(string data, int minLength, int maxLength)
        {
            return Regex.IsMatch(data, $@"^[a-zA-Z][a-zA-Z0-9_]{{{minLength - 1},{maxLength - 1}}}$");
        }
        //(必须包含大小写字母和数字的组合，不能使用特殊字符，长度在8-10之间)
        public static bool IsStrongCipher(string data)
        {
            return Regex.IsMatch(data, @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,10}$");
        }
        public static bool IsDataFormat(string data)
        {
            return Regex.IsMatch(data, @"^\d{4}-\d{1,2}-\d{1,2}");
        }
        public static bool IsChineseCharactor(string data)
        {
            return Regex.IsMatch(data, @"[\u4e00-\u9fa5]");
        }
        public static bool IsQQ_Number(string data)
        {
            return Regex.IsMatch(data, @"[1-9][0-9]{4,}");
        }
        public static bool IsPostalCode(string data)
        {
            return Regex.IsMatch(data, @"[1-9]\d{5}(?!\d)");
        }
    }
}
