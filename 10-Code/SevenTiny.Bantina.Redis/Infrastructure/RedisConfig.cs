/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-27 19:23:12
 * Modify: 2018-02-27 19:23:12
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using SevenTiny.Bantina.Configuration;

namespace SevenTiny.Bantina.Redis.Infrastructure
{
    [ConfigClass(Name = "Redis")]
    public class RedisConfig : ConfigBase<RedisConfig>
    {
        public int ID { get; set; }
        public string InstanceName { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
    }
}
