using System;

namespace SevenTiny.Bantina.Spring
{
    public static class RunExtension
    {
        public static void Run(this IApplicationBuilder app, RequestDelegate handler)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            app.Use(_ => handler);
        }
    }
}
