using SevenTiny.Bantina.Spring.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <summary>
        /// Add Service with lifetime
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        private static IServiceCollection Add(this IServiceCollection collection, Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            collection.Add(serviceType, new ServiceDescriptor(serviceType, implementationType, lifetime));
            return collection;
        }
        //private static IServiceCollection Add(this IServiceCollection collection, Type serviceType, object implementationInstance, ServiceLifetime lifetime)
        //{
        //    collection.Add(serviceType, new ServiceDescriptor(serviceType, implementationInstance, lifetime));
        //    return collection;
        //}
        private static IServiceCollection Add(this IServiceCollection collection, Type serviceType, Type implementationType, Func<DependencyInjection.IServiceProvider, object> implementationFactory, ServiceLifetime lifetime)
        {
            collection.Add(serviceType, new ServiceDescriptor(serviceType, implementationType, implementationFactory, lifetime));
            return collection;
        }

        // Summary:
        //     Adds a scoped service of the type specified in serviceType with an implementation
        //     of the type specified in implementationType to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        //     to.
        //
        //   serviceType:
        //     The type of the service to register.
        //
        //   implementationType:
        //     The implementation type of the service.
        //
        // Returns:
        //     A reference to this instance after the operation has completed.
        public static IServiceCollection AddScoped(this IServiceCollection services, Type serviceType, Type implementationType)
        {
            return services.Add(serviceType, implementationType, ServiceLifetime.Scoped);
        }
        //
        // Summary:
        //     Adds a scoped service of the type specified in TService with an implementation
        //     type specified in TImplementation to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        //     to.
        //
        // Type parameters:
        //   TService:
        //     The type of the service to add.
        //
        //   TImplementation:
        //     The type of the implementation to use.
        //
        // Returns:
        //     A reference to this instance after the operation has completed.
        public static IServiceCollection AddScoped<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
        {
            return services.Add(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped);
        }
        //
        // Summary:
        //     Adds a scoped service of the type specified in TService with an implementation
        //     type specified in TImplementation using the factory specified in implementationFactory
        //     to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        //     to.
        //
        //   implementationFactory:
        //     The factory that creates the service.
        //
        // Type parameters:
        //   TService:
        //     The type of the service to add.
        //
        //   TImplementation:
        //     The type of the implementation to use.
        //
        // Returns:
        //     A reference to this instance after the operation has completed.
        public static IServiceCollection AddScoped<TService, TImplementation>(this IServiceCollection services, Func<DependencyInjection.IServiceProvider, TService> implementationFactory) where TService : class where TImplementation : class, TService
        {
            return services.Add(typeof(TService), typeof(TImplementation), implementationFactory, ServiceLifetime.Scoped);
        }

        //
        // Summary:
        //     Adds a singleton service of the type specified in TService with an implementation
        //     type specified in TImplementation using the factory specified in implementationFactory
        //     to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        //     to.
        //
        //   implementationFactory:
        //     The factory that creates the service.
        //
        // Type parameters:
        //   TService:
        //     The type of the service to add.
        //
        //   TImplementation:
        //     The type of the implementation to use.
        //
        // Returns:
        //     A reference to this instance after the operation has completed.
        public static IServiceCollection AddSingleton<TService, TImplementation>(this IServiceCollection services, Func<DependencyInjection.IServiceProvider, TService> implementationFactory) where TService : class where TImplementation : class, TService
        {
            return services.Add(typeof(TService), typeof(TImplementation), implementationFactory, ServiceLifetime.Singleton);
        }
        //
        // Summary:
        //     Adds a singleton service of the type specified in TService with an implementation
        //     type specified in TImplementation to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        //     to.
        //
        // Type parameters:
        //   TService:
        //     The type of the service to add.
        //
        //   TImplementation:
        //     The type of the implementation to use.
        //
        // Returns:
        //     A reference to this instance after the operation has completed.
        public static IServiceCollection AddSingleton<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
        {
            return services.Add(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton);
        }
        //
        // Summary:
        //     Adds a singleton service of the type specified in serviceType with an implementation
        //     of the type specified in implementationType to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        //     to.
        //
        //   serviceType:
        //     The type of the service to register.
        //
        //   implementationType:
        //     The implementation type of the service.
        //
        // Returns:
        //     A reference to this instance after the operation has completed.
        public static IServiceCollection AddSingleton(this IServiceCollection services, Type serviceType, Type implementationType)
        {
            return services.Add(serviceType, implementationType, ServiceLifetime.Singleton);
        }

        //
        // Summary:
        //     Adds a singleton service of the type specified in TService with an instance specified
        //     in implementationInstance to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        //     to.
        //
        //   implementationInstance:
        //     The instance of the service.
        //
        // Returns:
        //     A reference to this instance after the operation has completed.
        //public static IServiceCollection AddSingleton<TService>(this IServiceCollection services, TService implementationInstance) where TService : class
        //{
        //    return services.Add(typeof(TService), implementationInstance, ServiceLifetime.Singleton);
        //}
        //
        // Summary:
        //     Adds a singleton service of the type specified in serviceType with an instance
        //     specified in implementationInstance to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        //     to.
        //
        //   serviceType:
        //     The type of the service to register.
        //
        //   implementationInstance:
        //     The instance of the service.
        //
        // Returns:
        //     A reference to this instance after the operation has completed.
        //public static IServiceCollection AddSingleton(this IServiceCollection services, Type serviceType, object implementationInstance)
        //{
        //    return services.Add(serviceType, implementationInstance, ServiceLifetime.Singleton);
        //}
        //
        // Summary:
        //     Adds a transient service of the type specified in TService with an implementation
        //     type specified in TImplementation using the factory specified in implementationFactory
        //     to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        //     to.
        //
        //   implementationFactory:
        //     The factory that creates the service.
        //
        // Type parameters:
        //   TService:
        //     The type of the service to add.
        //
        //   TImplementation:
        //     The type of the implementation to use.
        //
        // Returns:
        //     A reference to this instance after the operation has completed.
        public static IServiceCollection AddTransient<TService, TImplementation>(this IServiceCollection services, Func<DependencyInjection.IServiceProvider, TService> implementationFactory) where TService : class where TImplementation : class, TService
        {
            return services.Add(typeof(TService), typeof(TImplementation), implementationFactory, ServiceLifetime.Transient);
        }
        //
        // Summary:
        //     Adds a transient service of the type specified in TService with an implementation
        //     type specified in TImplementation to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        //     to.
        //
        // Type parameters:
        //   TService:
        //     The type of the service to add.
        //
        //   TImplementation:
        //     The type of the implementation to use.
        //
        // Returns:
        //     A reference to this instance after the operation has completed.
        public static IServiceCollection AddTransient<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
        {
            return services.Add(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient);
        }
        //
        // Summary:
        //     Adds a transient service of the type specified in serviceType with an implementation
        //     of the type specified in implementationType to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        //     to.
        //
        //   serviceType:
        //     The type of the service to register.
        //
        //   implementationType:
        //     The implementation type of the service.
        //
        // Returns:
        //     A reference to this instance after the operation has completed.
        public static IServiceCollection AddTransient(this IServiceCollection services, Type serviceType, Type implementationType)
        {
            return services.Add(serviceType, implementationType, ServiceLifetime.Transient);
        }
    }
}