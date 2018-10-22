using SevenTiny.Bantina.Spring;
using SevenTiny.Bantina.Spring.Aop;
using SevenTiny.Bantina.Spring.DependencyInjection;
using SevenTiny.Bantina.Spring.Middleware;
using System;

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
            services.AddSingletonWithAop<IAService, AService>();
            services.AddSingleton<IBusinessService, BusinessService>();
            services.AddSingletonWithAop<IDomainService, DomainService>();
        }

        public override void Start()
        {
            base.Start();
        }
    }
}
