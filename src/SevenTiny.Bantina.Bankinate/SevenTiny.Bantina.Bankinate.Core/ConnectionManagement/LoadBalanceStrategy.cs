namespace SevenTiny.Bantina.Bankinate.Configs
{
    /// <summary>
    /// 负载均衡策略
    /// </summary>
    public enum LoadBalanceStrategy
    {
        /// <summary>
        /// 轮询
        /// </summary>
        RoundRobin,
        /// <summary>
        /// 最小连接
        /// </summary>
        LeastConnection
    }
}
