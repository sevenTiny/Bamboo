using SevenTiny.Bantina.Spring.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SevenTiny.Bantina.Spring.DependencyInjection
{
    public class ServiceCollection : IServiceCollection
    {
        /// <summary>
        /// local storage with lifetime
        /// </summary>
        private static IDictionary<Type, ServiceDescriptor> serviceDescriptorsDic = new Dictionary<Type, ServiceDescriptor>();
        public ServiceDescriptor this[Type key] { get => serviceDescriptorsDic[key]; set => serviceDescriptorsDic[key] = value; }

        public ICollection<Type> Keys => serviceDescriptorsDic.Keys;

        public ICollection<ServiceDescriptor> Values => serviceDescriptorsDic.Values;

        public int Count => serviceDescriptorsDic.Count;

        public bool IsReadOnly => false;

        public void Add(Type key, ServiceDescriptor value)
        {
            serviceDescriptorsDic.AddOrUpdate(key, value);
        }

        public void Add(KeyValuePair<Type, ServiceDescriptor> item)
        {
            serviceDescriptorsDic.Add(item);
        }

        public void Clear()
        {
            serviceDescriptorsDic.Clear();
        }

        public bool Contains(KeyValuePair<Type, ServiceDescriptor> item)
        {
            return serviceDescriptorsDic.Contains(item);
        }

        public bool ContainsKey(Type key)
        {
            return serviceDescriptorsDic.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<Type, ServiceDescriptor>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<Type, ServiceDescriptor>> GetEnumerator()
        {
            return serviceDescriptorsDic.GetEnumerator();
        }

        public bool Remove(Type key)
        {
            return serviceDescriptorsDic.Remove(key);
        }

        public bool Remove(KeyValuePair<Type, ServiceDescriptor> item)
        {
            return serviceDescriptorsDic.Remove(item);
        }

        public bool TryGetValue(Type key, out ServiceDescriptor value)
        {
            return serviceDescriptorsDic.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
