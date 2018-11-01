using System;
using System.Collections.Generic;
using System.Reflection;

namespace SevenTiny.Bantina.Spring.DependencyInjection
{
    public class ServiceProvider : IServiceProvider
    {
        /// <summary>
        /// dynamic proxy get service via this method
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public object GetService(string assemblyName, string typeName)
        {
            Type type = Assembly.Load(assemblyName).GetType(typeName);
            return GetService(type);
        }

        public object GetService(Type serviceType)
        {
            ServiceDescriptor serviceDescriptor;
            if (!SpringContext.ServiceCollection.TryGetValue(serviceType, out serviceDescriptor))
            {
                throw new KeyNotFoundException($"service of {serviceType.Name} not register into container!");
            }

            if (serviceDescriptor.ImplementationInstance != null)
            {
                return serviceDescriptor.ImplementationInstance;
            }
            else if (serviceDescriptor.ImplementationFactory != null)
            {
                object instance;
                if (serviceDescriptor.ImplementationType == null)
                {
                    instance = serviceDescriptor.ImplementationFactory(this);
                }
                instance = GetServiceScanField(serviceDescriptor.ImplementationType, serviceDescriptor.ImplementationFactory(this));

                if (serviceDescriptor.LifeTime == ServiceLifetime.Transient)
                    return instance;
                else
                    return serviceDescriptor.ImplementationInstance = instance;
            }
            else
            {
                if (serviceDescriptor.ImplementationType == null)
                {
                    throw new KeyNotFoundException($"ImplementationType of {serviceType.Name} not register into container!");
                }

                if (serviceDescriptor.LifeTime == ServiceLifetime.Transient)
                    return GetServiceScanField(serviceDescriptor.ImplementationType, Activator.CreateInstance(serviceDescriptor.ImplementationType));
                else
                    return serviceDescriptor.ImplementationInstance = GetServiceScanField(serviceDescriptor.ImplementationType, Activator.CreateInstance(serviceDescriptor.ImplementationType));
            }
        }

        private object GetServiceScanField(Type serviceType, object serviceObj)
        {
            if (serviceType.IsInterface)
            {
                return serviceObj;
            }
            //if dynamic proxy object,jump scan
            if (serviceObj.GetType().Name.EndsWith("Proxy"))
            {
                return serviceObj;
            }
            var fieldInfos = serviceType.GetRuntimeFields();
            foreach (var field in fieldInfos)
            {
                if (ServiceAttribute.Exist(field))
                {
                    if (SpringContext.ServiceCollection.ContainsKey(field.FieldType))
                    {
                        field.SetValue(serviceObj, GetService(field.FieldType));
                    }
                }
            }
            return serviceObj;
        }
    }
}
