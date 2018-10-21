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
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<AService, AService>();
            services.AddSingleton<IAService, AService>(provider => DynamicProxy.CreateProxyOfRealize<IAService, AService>(typeof(InterceptorAttribute)));
            services.AddSingleton<IBusinessService, BusinessService>();
            services.AddSingleton<DomainService, DomainService>();
            services.AddSingleton<IDomainService, DomainService>(p => DynamicProxy.CreateProxyOfRealize<IDomainService, DomainService>());
        }

        public override void Start()
        {
            base.Start();
        }
    }
}
