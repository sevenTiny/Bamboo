using SevenTiny.Bantina.Spring.DependencyInjection;

namespace SevenTiny.Bantina.Spring
{
    public static class ServiceProviderExtension
    {
        public static TService GetService<TService>(this IServiceProvider serviceProvider) where TService : class
        {
            return Container<TService>.Instance;
        }
    }
}
