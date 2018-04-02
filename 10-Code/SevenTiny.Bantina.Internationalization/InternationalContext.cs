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
using SevenTiny.Bantina.Extensions;
using SevenTiny.Bantina.Internationalization.Configs;
using System;
using System.Collections.Generic;

namespace SevenTiny.Bantina.Internationalization
{
    public sealed class InternationalContext
    {
        /// <summary>
        /// English is default
        /// </summary>
        private static (int ID, string Code, string Content, string Description) NotFound =>
        (
            101,
            "System.Error.ConfigNotFound",
            "International config not found.",
            "Config not found or node of id is not exist."
        );

        /// <summary>
        /// internal cache
        /// </summary>
        private static Dictionary<int, (int ID, string Code, string Content, string Description)> _dictionary;

        private static void Initial()
        {
            _dictionary = new Dictionary<int, (int ID, string Code, string Content, string Description)>();

            IEnumerable<dynamic> configs;
            switch (Convert.ToInt32(ConfigRoot.Get("InternationalizationLanguage")))
            {
                case (int)InternationalizationLanguage.english:
                    configs = Internationalization_English_Config.ConfigEnumerable;
                    break;
                case (int)InternationalizationLanguage.chinese:
                    configs = Internationalization_Chinese_Config.ConfigEnumerable;
                    break;
                default:
                    configs = null;
                    break;
            }

            foreach (var item in configs)
            {
                _dictionary.AddOrUpdate((int)item.ID, ((int)item.ID, (string)item.Code, (string)item.Content, (string)item.Description));
            }
        }

        public static (int ID, string Code, string Content, string Description) Context(int id)
        {
            //get from local cache
            if (_dictionary != null)
            {
                return _dictionary.SafeGet(id);
            }
            //if not exist,initial.
            Initial();
            //get from local cache after initial.
            if (_dictionary != null)
            {
                return _dictionary.SafeGet(id);
            }
            return NotFound;
        }
        public static string Code(int id) => Context(id).Code ?? NotFound.Code;
        public static string Content(int id) => Context(id).Content ?? NotFound.Content;
        public static string Description(int id) => Context(id).Description ?? NotFound.Description;
    }
}