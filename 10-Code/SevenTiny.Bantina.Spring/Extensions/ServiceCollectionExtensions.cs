using SevenTiny.Bantina.Spring.Aop;
using SevenTiny.Bantina.Spring.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace SevenTiny.Bantina.Spring
{
    //
    // Summary:
    //     Extension methods for adding services to an Microsoft.Extensions.DependencyInjection.IServiceCollection.
    public static class ServiceCollectionExtensions
    {
        public static DependencyInjection.IServiceProvider BuildServiceProvider(this IServiceCollection services)
        {
            return new ServiceProvider();
        }
        private static IServiceCollection Add(this IServiceCollection collection, Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            if (serviceType != implementationType && !serviceType.IsAssignableFrom(implementationType))
                throw new TypeRegistrationException(serviceType, implementationType, "The type of registration is not an inheritance relationship");

            collection.Add(serviceType, new ServiceDescriptor(serviceType, implementationType, lifetime));
            return collection;
        }
        private static IServiceCollection Add(this IServiceCollection collection, Type serviceType, Type implementationType, Func<DependencyInjection.IServiceProvider, object> implementationFactory, ServiceLifetime lifetime)
        {
            if (serviceType != implementationType && !serviceType.IsAssignableFrom(implementationType))
                throw new TypeRegistrationException(serviceType, implementationType, "The type of registration is not an inheritance relationship");

            collection.Add(serviceType, new ServiceDescriptor(serviceType, implementationType, implementationFactory, lifetime));
            return collection;
        }
        private static IServiceCollection Add(this IServiceCollection collection, Type serviceType, Func<DependencyInjection.IServiceProvider, object> implementationFactory, ServiceLifetime lifetime)
        {
            collection.Add(serviceType, new ServiceDescriptor(serviceType, implementationFactory, lifetime));
            return collection;
        }
        private static IServiceCollection Add(this IServiceCollection collection, Assembly assembly, ServiceLifetime serviceLifetime)
        {
            var types = assembly.GetTypes();
            var interfaces = types.Where(t => t.IsInterface);
            var impTypes = types.Except(interfaces).ToList();
            foreach (var item in interfaces)
            {
                var impType = impTypes.FirstOrDefault(t => item.IsAssignableFrom(t));
                if (impType != null)
                {
                    collection.Add(item, impType, serviceLifetime);
                }
            }
            return collection;
        }

        public static IServiceCollection AddScoped(this IServiceCollection services, Assembly assembly)
        {
            return services.Add(assembly, ServiceLifetime.Scoped);
        }
        public static IServiceCollection AddScoped<TService>(this IServiceCollection services) where TService : class
        {
            return services.Add(typeof(TService), typeof(TService), ServiceLifetime.Scoped);
        }
        public static IServiceCollection AddScoped<TService>(this IServiceCollection services, Func<DependencyInjection.IServiceProvider, TService> implementationFactory) where TService : class
        {
            return services.Add(typeof(TService), implementationFactory, ServiceLifetime.Scoped);
        }
        public static IServiceCollection AddScoped(this IServiceCollection services, Type serviceType, Type implementationType)
        {
            return services.Add(serviceType, implementationType, ServiceLifetime.Scoped);
        }
        public static IServiceCollection AddScoped<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
        {
            return services.Add(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped);
        }
        public static IServiceCollection AddScoped<TService, TImplementation>(this IServiceCollection services, Func<DependencyInjection.IServiceProvider, TService> implementationFactory) where TService : class where TImplementation : class, TService
        {
            return services.Add(typeof(TService), typeof(TImplementation), implementationFactory, ServiceLifetime.Scoped);
        }
        public static IServiceCollection AddScopedWithAop<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService, new()
        {
            services.Add(typeof(TImplementation), typeof(TImplementation), ServiceLifetime.Scoped);
            services.Add(typeof(TService), typeof(TImplementation), p => DynamicProxy.CreateProxyOfRealize<TService, TImplementation>(), ServiceLifetime.Scoped);
            return services;
        }

        public static IServiceCollection AddSingleton(this IServiceCollection services, Assembly assembly)
        {
            return services.Add(assembly, ServiceLifetime.Singleton);
        }
        public static IServiceCollection AddSingleton<TService>(this IServiceCollection services) where TService : class
        {
            return services.Add(typeof(TService), typeof(TService), ServiceLifetime.Singleton);
        }
        public static IServiceCollection AddSingleton<TService>(this IServiceCollection services, Func<DependencyInjection.IServiceProvider, TService> implementationFactory) where TService : class
        {
            return services.Add(typeof(TService), implementationFactory, ServiceLifetime.Singleton);
        }
        public static IServiceCollection AddSingleton<TService, TImplementation>(this IServiceCollection services, Func<DependencyInjection.IServiceProvider, TService> implementationFactory) where TService : class where TImplementation : class, TService
        {
            return services.Add(typeof(TService), typeof(TImplementation), implementationFactory, ServiceLifetime.Singleton);
        }
        public static IServiceCollection AddSingleton<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
        {
            return services.Add(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton);
        }
        public static IServiceCollection AddSingletonWithAop<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService, new()
        {
            services.Add(typeof(TImplementation), typeof(TImplementation), ServiceLifetime.Singleton);
            services.Add(typeof(TService), typeof(TImplementation), p => DynamicProxy.CreateProxyOfRealize<TService, TImplementation>(), ServiceLifetime.Singleton);
            return services;
        }
        public static IServiceCollection AddSingleton(this IServiceCollection services, Type serviceType, Type implementationType)
        {
            return services.Add(serviceType, implementationType, ServiceLifetime.Singleton);
        }

        public static IServiceCollection AddTransien(this IServiceCollection services, Assembly assembly)
        {
            return services.Add(assembly, ServiceLifetime.Transient);
        }
        public static IServiceCollection AddTransient<TService>(this IServiceCollection services) where TService : class
        {
            return services.Add(typeof(TService), typeof(TService), ServiceLifetime.Transient);
        }
        public static IServiceCollection AddTransient<TService>(this IServiceCollection services, Func<DependencyInjection.IServiceProvider, TService> implementationFactory) where TService : class
        {
            return services.Add(typeof(TService), implementationFactory, ServiceLifetime.Transient);
        }
        public static IServiceCollection AddTransient<TService, TImplementation>(this IServiceCollection services, Func<DependencyInjection.IServiceProvider, TService> implementationFactory) where TService : class where TImplementation : class, TService
        {
            return services.Add(typeof(TService), typeof(TImplementation), implementationFactory, ServiceLifetime.Transient);
        }
        public static IServiceCollection AddTransient<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
        {
            return services.Add(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient);
        }
        public static IServiceCollection AddTransientWithAop<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService, new()
        {
            services.Add(typeof(TImplementation), typeof(TImplementation), ServiceLifetime.Transient);
            services.Add(typeof(TService), typeof(TImplementation), p => DynamicProxy.CreateProxyOfRealize<TService, TImplementation>(), ServiceLifetime.Transient);
            return services;
        }
        public static IServiceCollection AddTransient(this IServiceCollection services, Type serviceType, Type implementationType)
        {
            return services.Add(serviceType, implementationType, ServiceLifetime.Transient);
        }
    }
}