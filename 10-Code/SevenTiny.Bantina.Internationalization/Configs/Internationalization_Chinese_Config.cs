/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-15 23:04:25
 * Modify: 2018-02-15 23:04:25
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using SevenTiny.Bantina.Configuration;
using System.Collections.Generic;

namespace SevenTiny.Bantina.Internationalization.Configs
{
    [ConfigName(Name = "Internationalization_Chinese")]
    public class Internationalization_Chinese_Config : ConfigBase<Internationalization_Chinese_Config>
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public static IEnumerable<Internationalization_Chinese_Config> ConfigEnumerable => Configs;
    }
}
