using System;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Spring
{
    public delegate Task RequestDelegate(SpringContext context);

    public interface IApplicationBuilder
    {
        /// <summary>
        /// Common pipeline injector
        /// </summary>
        /// <param name="middleware"></param>
        /// <returns></returns>
        IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware);
        RequestDelegate Build();
    }
}