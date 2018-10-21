using SevenTiny.Bantina.Spring.DependencyInjection;

namespace SevenTiny.Bantina.Spring
{
    public abstract class SpringStartUp
    {
        public abstract void Configure(IApplicationBuilder app);
        public abstract void ConfigureServices(IServiceCollection services);

        private static ApplicationBuilder ApplicationBuilder = new ApplicationBuilder();
        public static RequestDelegate RequestDelegate { get; private set; }
        public virtual void Start()
        {
            Configure(ApplicationBuilder);
            ConfigureServices(new ServiceCollection());

            RequestDelegate = ApplicationBuilder.Build();
        }
    }
}