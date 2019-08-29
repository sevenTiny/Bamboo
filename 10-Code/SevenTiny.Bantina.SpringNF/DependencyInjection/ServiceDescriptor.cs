using System;

namespace SevenTiny.Bantina.Spring.DependencyInjection
{
    public class ServiceDescriptor
    {
        internal ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            LifeTime = lifetime;
        }
        internal ServiceDescriptor(Type serviceType, object instance, ServiceLifetime lifetime)
        {
            if (!serviceType.IsInstanceOfType(instance))
            {
                throw new FormatException($"the type of instance registered not match of type {serviceType.Name} ");
            }

            ServiceType = serviceType;
            ImplementationInstance = instance;
            ImplementationType = instance.GetType();
            LifeTime = lifetime;
        }
        internal ServiceDescriptor(Type serviceType, Func<IServiceProvider, object> factory, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            ImplementationType = serviceType;
            ImplementationFactory = factory;
            LifeTime = lifetime;
        }
        internal ServiceDescriptor(Type serviceType, Type implementationType, Func<IServiceProvider, object> factory, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            ImplementationFactory = factory;
            LifeTime = lifetime;
        }

        public Type ServiceType { get; }
        public Type ImplementationType { get; }
        public ServiceLifetime LifeTime { get; }

        public object ImplementationInstance { get; internal set; }
        public Func<IServiceProvider, object> ImplementationFactory { get; }
    }
}
