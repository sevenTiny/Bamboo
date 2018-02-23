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
    public sealed class InternationalContext
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// English is default
        /// </summary>
        private static InternationalContext NotFound => new InternationalContext
        {
            ID = 101,
            Code = "System.Error.ConfigNotFound",
            Content = "International config not found.",
            Description = "Config not found or node of id is not exist."
        };

        private static dynamic Get(int id)
        {
            try
            {
                switch (Convert.ToInt32(SevenTinyBantinaConfig.Get("InternationalizationLanguage")))
                {
                    case (int)InternationalizationLanguage.english:
                        return Internationalization_English_Config.Get(id);
                    case (int)InternationalizationLanguage.chinese:
                        return Internationalization_Chinese_Config.Get(id);
                    default:
                        return NotFound;
                }
            }
            catch (Exception)
            {
                return NotFound;
            }
        }

        /// <summary>
        /// Get InternationalContext By id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static InternationalContext Context(int id)
        {
            try
            {
                dynamic result = Get(id);
                //if result = null,return not found, if not, return international context
                return result == null ? NotFound : new InternationalContext
                {
                    ID = result.ID,
                    Code = result.Code,
                    Content = result.Content,
                    Description = result.Description
                };
            }
            catch (Exception)
            {
                return NotFound;
            }
        }

        public static string InternationalCode(int id) => Get(id)?.Code ?? NotFound.Code;
        public static string InternationalContent(int id) => Get(id)?.Content ?? NotFound.Content;
        public static string InternationalDescription(int id) => Get(id)?.Description ?? NotFound.Description;
    }
}
