using SevenTiny.Bantina.Spring.Aop;
using System;

namespace SevenTiny.Bantina.Spring.Middleware
{
    public static class DynamicProxyMiddleware
    {
        public static void UseDynamicProxy(this IApplicationBuilder app)
        {
            app.UseDynamicProxy(context => true);
        }

        public static void UseDynamicProxy(this IApplicationBuilder app, Func<SpringContext, bool> beforeCheck)
        {
            app.Use((context, next) =>
            {
                if (beforeCheck(context))
                {
                    context.Result = new InterceptorBaseAttribute().Invoke(context.InstanceObject, context.Method, context.Parameters);
                }
                return next();
            });
        }
    }
}