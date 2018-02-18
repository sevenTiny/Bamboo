/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-15 23:05:14
 * Modify: 2018-02-15 23:05:14
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using SevenTiny.Bantina.Configuration;
using SevenTiny.Bantina.Internationalization.Configs;
using System;

namespace SevenTiny.Bantina.Internationalization
{
    public static class InternationalContext
    {
        public static string Content(int code)
        {
            switch (Convert.ToInt32(SevenTinyBantinaConfig.Get("InternationalizationLanguage")))
            {
                case (int)InternationalizationLanguage.english:
                    return Internationalization_English_Config.Get(code);
                case (int)InternationalizationLanguage.chinese:
                    return Internationalization_Chinese_Config.Get(code);
                default:
                    return default(string);
            }
        }
    }
}
