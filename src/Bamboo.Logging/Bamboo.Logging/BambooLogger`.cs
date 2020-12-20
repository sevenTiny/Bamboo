/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-02-18 21:10:21
 * Modify: 2018-02-18 21:10:21
 * Modify: 2020-03-28 16:22:00
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using Microsoft.Extensions.Logging;

namespace Bamboo.Logging
{
    public class BambooLogger<TCategoryName> : BambooLogger, ILogger<TCategoryName>
    {
        public BambooLogger() : base(typeof(TCategoryName).Name)
        {
        }
    }
}
