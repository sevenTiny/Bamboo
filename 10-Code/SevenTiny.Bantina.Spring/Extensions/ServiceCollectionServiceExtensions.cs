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
    public static class ServiceCollectionServiceExtensions
    {
        ////
        //// Summary:
        ////     Adds a scoped service of the type specified in TService to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        ////
        //// Parameters:
        ////   services:
        ////     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        ////     to.
        ////
        //// Type parameters:
        ////   TService:
        ////     The type of the service to add.
        ////
        //// Returns:
        ////     A reference to this instance after the operation has completed.
        //public static IServiceCollection AddScoped<TService>(this IServiceCollection services) where TService : class{return services;}
        ////
        //// Summary:
        ////     Adds a scoped service of the type specified in serviceType with an implementation
        ////     of the type specified in implementationType to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        ////
        //// Parameters:
        ////   services:
        ////     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        ////     to.
        ////
        ////   serviceType:
        ////     The type of the service to register.
        ////
        ////   implementationType:
        ////     The implementation type of the service.
        ////
        //// Returns:
        ////     A reference to this instance after the operation has completed.
        //public static IServiceCollection AddScoped(this IServiceCollection services, Type serviceType, Type implementationType){return services;}
        ////
        //// Summary:
        ////     Adds a scoped service of the type specified in serviceType with a factory specified
        ////     in implementationFactory to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        ////
        //// Parameters:
        ////   services:
        ////     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        ////     to.
        ////
        ////   serviceType:
        ////     The type of the service to register.
        ////
        ////   implementationFactory:
        ////     The factory that creates the service.
        ////
        //// Returns:
        ////     A reference to this instance after the operation has completed.
        //public static IServiceCollection AddScoped(this IServiceCollection services, Type serviceType, Func<IServiceProvider, object> implementationFactory){return services;}
        ////
        //// Summary:
        ////     Adds a scoped service of the type specified in TService with an implementation
        ////     type specified in TImplementation to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        ////
        //// Parameters:
        ////   services:
        ////     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        ////     to.
        ////
        //// Type parameters:
        ////   TService:
        ////     The type of the service to add.
        ////
        ////   TImplementation:
        ////     The type of the implementation to use.
        ////
        //// Returns:
        ////     A reference to this instance after the operation has completed.
        //public static IServiceCollection AddScoped<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService{return services;}
        ////
        //// Summary:
        ////     Adds a scoped service of the type specified in serviceType to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        ////
        //// Parameters:
        ////   services:
        ////     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        ////     to.
        ////
        ////   serviceType:
        ////     The type of the service to register and the implementation to use.
        ////
        //// Returns:
        ////     A reference to this instance after the operation has completed.
        //public static IServiceCollection AddScoped(this IServiceCollection services, Type serviceType){return services;}
        ////
        //// Summary:
        ////     Adds a scoped service of the type specified in TService with a factory specified
        ////     in implementationFactory to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        ////
        //// Parameters:
        ////   services:
        ////     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        ////     to.
        ////
        ////   implementationFactory:
        ////     The factory that creates the service.
        ////
        //// Type parameters:
        ////   TService:
        ////     The type of the service to add.
        ////
        //// Returns:
        ////     A reference to this instance after the operation has completed.
        //public static IServiceCollection AddScoped<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory) where TService : class{return services;}
        ////
        //// Summary:
        ////     Adds a scoped service of the type specified in TService with an implementation
        ////     type specified in TImplementation using the factory specified in implementationFactory
        ////     to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        ////
        //// Parameters:
        ////   services:
        ////     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        ////     to.
        ////
        ////   implementationFactory:
        ////     The factory that creates the service.
        ////
        //// Type parameters:
        ////   TService:
        ////     The type of the service to add.
        ////
        ////   TImplementation:
        ////     The type of the implementation to use.
        ////
        //// Returns:
        ////     A reference to this instance after the operation has completed.
        //public static IServiceCollection AddScoped<TService, TImplementation>(this IServiceCollection services, Func<IServiceProvider, TImplementation> implementationFactory) where TService : class where TImplementation : class, TService{return services;}
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
        //public static IServiceCollection AddSingleton<TService, TImplementation>(this IServiceCollection services, Func<IServiceProvider, TImplementation> implementationFactory) where TService : class where TImplementation : class, TService
        //{
        //    Container.Instance.Register<TService,TImplementation>();
        //    return services;
        //}
        ////
        //// Summary:
        ////     Adds a singleton service of the type specified in TService with a factory specified
        ////     in implementationFactory to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        ////
        //// Parameters:
        ////   services:
        ////     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        ////     to.
        ////
        ////   implementationFactory:
        ////     The factory that creates the service.
        ////
        //// Type parameters:
        ////   TService:
        ////     The type of the service to add.
        ////
        //// Returns:
        ////     A reference to this instance after the operation has completed.
        //public static IServiceCollection AddSingleton<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory) where TService : class
        //{
        //    Container.Instance.Register<TService>(()=> { return implementationFactory(t); });
        //    return services;
        //}
        ////
        //// Summary:
        ////     Adds a singleton service of the type specified in TService to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        ////
        //// Parameters:
        ////   services:
        ////     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        ////     to.
        ////
        //// Type parameters:
        ////   TService:
        ////     The type of the service to add.
        ////
        //// Returns:
        ////     A reference to this instance after the operation has completed.
        //public static IServiceCollection AddSingleton<TService>(this IServiceCollection services) where TService : class
        //{
        //    return services;
        //}
        ////
        //// Summary:
        ////     Adds a singleton service of the type specified in serviceType to the specified
        ////     Microsoft.Extensions.DependencyInjection.IServiceCollection.
        ////
        //// Parameters:
        ////   services:
        ////     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        ////     to.
        ////
        ////   serviceType:
        ////     The type of the service to register and the implementation to use.
        ////
        //// Returns:
        ////     A reference to this instance after the operation has completed.
        //public static IServiceCollection AddSingleton(this IServiceCollection services, Type serviceType)
        //{
        //    return services;
        //}
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
            Container.Instance.Register<TService,TImplementation>();
            return services;
        }
        //
        // Summary:
        //     Adds a singleton service of the type specified in serviceType with a factory
        //     specified in implementationFactory to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        //     to.
        //
        //   serviceType:
        //     The type of the service to register.
        //
        //   implementationFactory:
        //     The factory that creates the service.
        //
        // Returns:
        //     A reference to this instance after the operation has completed.
        //public static IServiceCollection AddSingleton(this IServiceCollection services, Type serviceType, Func<IServiceProvider, object> implementationFactory)
        //{
        //    return services;
        //}
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
            Container.Instance.Register(serviceType, implementationType);
            return services;
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
        public static IServiceCollection AddSingleton<TService>(this IServiceCollection services, TService implementationInstance) where TService : class
        {
            Container.Instance.Register<TService>(()=> { return implementationInstance; });
            return services;
        }
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
        //    return services;
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
        //public static IServiceCollection AddTransient<TService, TImplementation>(this IServiceCollection services, Func<IServiceProvider, TImplementation> implementationFactory) where TService : class where TImplementation : class, TService{return services;}
        ////
        //// Summary:
        ////     Adds a transient service of the type specified in TService with a factory specified
        ////     in implementationFactory to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        ////
        //// Parameters:
        ////   services:
        ////     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        ////     to.
        ////
        ////   implementationFactory:
        ////     The factory that creates the service.
        ////
        //// Type parameters:
        ////   TService:
        ////     The type of the service to add.
        ////
        //// Returns:
        ////     A reference to this instance after the operation has completed.
        //public static IServiceCollection AddTransient<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory) where TService : class{return services;}
        ////
        //// Summary:
        ////     Adds a transient service of the type specified in TService to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        ////
        //// Parameters:
        ////   services:
        ////     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        ////     to.
        ////
        //// Type parameters:
        ////   TService:
        ////     The type of the service to add.
        ////
        //// Returns:
        ////     A reference to this instance after the operation has completed.
        //public static IServiceCollection AddTransient<TService>(this IServiceCollection services) where TService : class{return services;}
        ////
        //// Summary:
        ////     Adds a transient service of the type specified in serviceType to the specified
        ////     Microsoft.Extensions.DependencyInjection.IServiceCollection.
        ////
        //// Parameters:
        ////   services:
        ////     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        ////     to.
        ////
        ////   serviceType:
        ////     The type of the service to register and the implementation to use.
        ////
        //// Returns:
        ////     A reference to this instance after the operation has completed.
        //public static IServiceCollection AddTransient(this IServiceCollection services, Type serviceType){return services;}
        ////
        //// Summary:
        ////     Adds a transient service of the type specified in TService with an implementation
        ////     type specified in TImplementation to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        ////
        //// Parameters:
        ////   services:
        ////     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        ////     to.
        ////
        //// Type parameters:
        ////   TService:
        ////     The type of the service to add.
        ////
        ////   TImplementation:
        ////     The type of the implementation to use.
        ////
        //// Returns:
        ////     A reference to this instance after the operation has completed.
        //public static IServiceCollection AddTransient<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService{return services;}
        ////
        //// Summary:
        ////     Adds a transient service of the type specified in serviceType with a factory
        ////     specified in implementationFactory to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        ////
        //// Parameters:
        ////   services:
        ////     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        ////     to.
        ////
        ////   serviceType:
        ////     The type of the service to register.
        ////
        ////   implementationFactory:
        ////     The factory that creates the service.
        ////
        //// Returns:
        ////     A reference to this instance after the operation has completed.
        //public static IServiceCollection AddTransient(this IServiceCollection services, Type serviceType, Func<IServiceProvider, object> implementationFactory){return services;}
        ////
        //// Summary:
        ////     Adds a transient service of the type specified in serviceType with an implementation
        ////     of the type specified in implementationType to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        ////
        //// Parameters:
        ////   services:
        ////     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service
        ////     to.
        ////
        ////   serviceType:
        ////     The type of the service to register.
        ////
        ////   implementationType:
        ////     The implementation type of the service.
        ////
        //// Returns:
        ////     A reference to this instance after the operation has completed.
        //public static IServiceCollection AddTransient(this IServiceCollection services, Type serviceType, Type implementationType){return services;}
    }
}