using System;
using System.Collections.Generic;
using System.Reflection;

namespace SevenTiny.Bantina.Spring.DependencyInjection
{
    internal class ServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            ServiceDescriptor serviceDescriptor;
            if (!SpringContext.ServiceCollection.TryGetValue(serviceType, out serviceDescriptor))
            {
                throw new KeyNotFoundException($"service of {serviceType.Name} not register into container!");
            }
            if (serviceDescriptor.ImplementationType == null)
            {
                throw new KeyNotFoundException($"ImplementationType of {serviceType.Name} not register into container!");
            }

            if (serviceDescriptor.ImplementationInstance != null)
            {
                return serviceDescriptor.ImplementationInstance;
            }
            else if (serviceDescriptor.ImplementationFactory != null)
            {
                return serviceDescriptor.ImplementationInstance = GetServiceScanField(serviceDescriptor.ImplementationType, serviceDescriptor.ImplementationFactory(this));
            }
            else
            {
                return serviceDescriptor.ImplementationInstance = GetServiceScanField(serviceDescriptor.ImplementationType, Activator.CreateInstance(serviceDescriptor.ImplementationType));
            }
        }

        private object GetServiceScanField(Type serviceType, object serviceObj)
        {
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
