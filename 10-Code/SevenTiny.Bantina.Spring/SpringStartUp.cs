using SevenTiny.Bantina.Spring.DependencyInjection;

namespace SevenTiny.Bantina.Spring
{
    public abstract class SpringStartUp
    {
        public abstract void Configure(IApplicationBuilder app);
        public abstract void ConfigureServices(IServiceCollection services);
    }
}