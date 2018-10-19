using SevenTiny.Bantina.Spring.Aop;
using SevenTiny.Bantina.Spring.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SevenTiny.Bantina.Spring.DependencyInjection
{
    internal class Container : IContainer
    {
        #region Fields
        readonly Dictionary<Type, Func<object>> ServiceMappings;
        readonly IDictionary<Type, Type> ContractServiceMappings;
        #endregion

        #region Singleton
        static readonly Container _Instance = new Container();
        public static Container Instance
        {
            get
            {
                return _Instance;
            }
        }
        #endregion

        #region Ctor
        private Container()
        {
            ServiceMappings = new Dictionary<Type, Func<object>>();
            ContractServiceMappings = new Dictionary<Type, Type>();
        }
        #endregion

        public IContainer Register<TService>(Func<TService> factory) where TService : class
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            ServiceMappings.AddOrUpdate(typeof(TService), factory);

            return this;
        }
        public IContainer Register<TContract, TServiceImp>() where TContract : class where TServiceImp : class
        {
            ContractServiceMappings.AddOrUpdate(typeof(TContract), typeof(TServiceImp));
            return this;
        }
        public IContainer Register(Type serviceType, Type implementationTyp)
        {
            ContractServiceMappings.AddOrUpdate(serviceType, implementationTyp);
            return this;
        }

        public TService Resolve<TService>() where TService : class
        {
            return Resolve(typeof(TService)) as TService;
        }

        public object Resolve(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            Func<object> factory;
            if (ServiceMappings.TryGetValue(type, out factory))
            {
                return factory();
            }
            else if (ContractServiceMappings.ContainsKey(type))
            {
                //if contractservice mappings contanins key,call constructor to generate type
                var serviceType = ContractServiceMappings[type];
                //generate instance of servicetype
                object[] parameters = ConstructorParametersGenerate(serviceType);
                /**
                 * 这里需要进行优化，将动态代理和构造参数实例的创建整合
                 * 下面注释的这句话是不带动态代理的原获取对象的方法
                 * 由下面的判断可以得知，目前仅仅支持最底层无参构造方法的aop切面
                 * */
                //object instance = CreateObjectFactory.CreateInstance(serviceType, parameters);
                object instance;
                if (parameters.Length > 0)
                    instance = CreateObjectFactory.CreateInstance(serviceType, parameters);
                else
                    instance = DynamicProxy.CreateProxyOfRealize(type, serviceType);
                //store instance
                ServiceMappings.AddOrUpdate(type, () => instance);

                return instance;
            }

            throw new KeyNotFoundException($"The type {type.Name} is not registered in the container!");
        }

        private object[] ConstructorParametersGenerate(Type type)
        {
            var construcorParameters = type.GetConstructors().FirstOrDefault().GetParameters();
            object[] parameterObjs = new object[construcorParameters.Count()];
            for (int i = 0; i < construcorParameters.Count(); i++)
            {
                var partype = construcorParameters[i].ParameterType;
                parameterObjs[i] = Resolve(partype);
            }
            return parameterObjs;
        }

        public void Reset()
        {
            ServiceMappings.Clear();
            ContractServiceMappings.Clear();
        }
    }

    public class Container<T> where T : class
    {
        public static T Instance
        {
            get
            {
                return Container.Instance.Resolve<T>();
            }
        }
    }
}