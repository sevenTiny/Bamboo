using System;

namespace SevenTiny.Bantina.Spring.DependencyInjection
{
    public class TypeRegistrationException : Exception
    {
        public TypeRegistrationException(string message) : base(message)
        {
        }
        public TypeRegistrationException(Type serviceType, Type impType, string message) : base($"{serviceType.Name} & {impType.Name} {message}")
        {
        }
    }
}
