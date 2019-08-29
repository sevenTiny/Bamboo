using SevenTiny.Bantina.Spring.DependencyInjection;

namespace SevenTiny.Bantina.Spring
{
    /// <summary>
    /// Spring 框架启动类，需要子类继承并传递对应的Configure和ConfigureServices
    /// </summary>
    public abstract class SpringStartUp
    {
        /// <summary>
        /// Pipeline Adapter
        /// </summary>
        /// <param name="app"></param>
        public abstract void Configure(IApplicationBuilder app);
        /// <summary>
        /// Service Adapter
        /// </summary>
        /// <param name="services"></param>
        public abstract void ConfigureServices(IServiceCollection services);
        /// <summary>
        /// Builder
        /// </summary>
        private static ApplicationBuilder Builder = new ApplicationBuilder();
        /// <summary>
        /// Pipline Delegate
        /// </summary>
        public static RequestDelegate RequestDelegate { get; private set; }
        /// <summary>
        /// Config Register Start
        /// </summary>
        public virtual void Start()
        {
            Configure(Builder);
            ConfigureServices(new ServiceCollection());
            RequestDelegate = Builder.Build();
        }
    }
}