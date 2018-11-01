namespace SevenTiny.Bantina.Spring.DependencyInjection
{
    /// <summary>
    /// ServiceLifetime
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>
        /// 单例存储对象
        /// </summary>
        Singleton,
        /// <summary>
        /// 在同一个Scope内只初始化一个实例 ，可以理解为（ 每一个request级别只创建一个实例，同一个http request会在一个 scope内）
        /// </summary>
        Scoped,
        /// <summary>
        /// 每一次GetService都会创建一个新的实例
        /// </summary>
        Transient
    }
}
