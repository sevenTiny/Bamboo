using System;

namespace SevenTiny.Bantina.Spring.DependencyInjection
{
    /// <summary>
    /// 容器实例提供器
    /// </summary>
    public interface IServiceProvider
    {
        object GetService(Type serviceType);
    }
}