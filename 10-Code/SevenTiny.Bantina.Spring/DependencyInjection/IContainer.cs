using System;

namespace SevenTiny.Bantina.Spring.DependencyInjection
{
    internal interface IContainer
    {
        IContainer Register<TService>(Func<TService> service) where TService : class;
        IContainer Register<TContract, TServiceImp>() where TContract : class where TServiceImp : class;
        IContainer Register(Type serviceType, Type implementationTyp);
        void Reset();
    }
}