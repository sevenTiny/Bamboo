using Bamboo.ScriptEngine.CSharp;
using Bamboo.ScriptEngine.CSharp.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Test.Bamboo.ScriptEngine.CSharp
{
    internal static class ServiceProviderBuilder
    {
        public static ServiceProvider Build()
        {
            // 构建配置
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 配置服务
            var serviceProvider = new ServiceCollection()
                .Configure<ScriptEngineCSharpConfig>(configuration.GetSection("Bamboo.ScriptEngine.CSharp"))
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton<ReferenceManager>(provider =>
                    new ReferenceManager(provider, new ReferenceConfig
                    {
                        RuntimeDependentAssemblies = []
                    }))
                .AddSingleton<ICSharpScriptEngine, CSharpScriptEngine>()
                .AddLogging()
                .BuildServiceProvider();

            return serviceProvider;
        }
    }
}
