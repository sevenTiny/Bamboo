using System;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Spring
{
    public delegate Task RequestDelegate(SpringContext context);

    public interface IApplicationBuilder
    {
        IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware);
        RequestDelegate Build();
    }
}