namespace SevenTiny.Bantina.Spring.Middleware
{
    internal static class DependencyControlMiddleware
    {
        /// <summary>
        /// this extension use in the end of pipeline
        /// </summary>
        /// <param name="app"></param>
        internal static void UseDependencyControl(this IApplicationBuilder app)
        {
            app.Use((context, next) =>
            {
                var result = next();
                SpringContext.RequestServices.ScanAbandonService();
                return result;
            });
        }
    }
}
