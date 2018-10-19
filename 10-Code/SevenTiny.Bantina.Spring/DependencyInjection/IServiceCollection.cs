using System;
using System.Collections.Generic;

namespace SevenTiny.Bantina.Spring.DependencyInjection
{
    /// <summary>
    /// 容器的注册器接口
    /// </summary>
    public interface IServiceCollection : IDictionary<Type, ServiceDescriptor>
    {
    }
}