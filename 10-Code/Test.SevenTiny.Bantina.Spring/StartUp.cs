using SevenTiny.Bantina.Spring;
using SevenTiny.Bantina.Spring.Aop;
using SevenTiny.Bantina.Spring.DependencyInjection;
using SevenTiny.Bantina.Spring.Middleware;
using System;
using System.Reflection;

namespace Test.SevenTiny.Bantina.Spring
{
    public class StartUp : SpringStartUp
    {
        public override void Configure(IApplicationBuilder app)
        {
            app.UseDynamicProxy();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Assembly.Load("Test.SevenTiny.Bantina.Spring"));
            services.AddSingletonWithAop<IAService, AService>();
            services.AddSingletonWithAop<IDomainService, DomainService>();
        }

        public override void Start()
        {
            base.Start();
        }
    }
}
