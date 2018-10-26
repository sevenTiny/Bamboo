using System;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Spring
{
    public static class UseExtensions
    {
        public static IApplicationBuilder Use(this IApplicationBuilder app, Func<SpringContext, Func<Task>, Task> middleware)
        {
            return app.Use(next =>
            {
                return context =>
                {
                    return middleware(context, () => next(context));
                };
            });
        }
    }
}