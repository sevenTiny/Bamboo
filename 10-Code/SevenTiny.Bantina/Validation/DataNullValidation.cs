/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-15 17:32:02
 * Modify: 2018-02-15 17:32:02
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
    public static class DataNullValidation
    {
        public static bool IsNull(this object data)
        {
            return data == null;
        }
        public static object CheckNullGet(this object data, string errorMessage)
        {
            if (data == null)
                throw new Exception(errorMessage);
            return data;
        }
        public static void CheckNull(this object data, string errorMessage)
        {
            if (data == null)
                throw new Exception(errorMessage);
        }

    }
}
