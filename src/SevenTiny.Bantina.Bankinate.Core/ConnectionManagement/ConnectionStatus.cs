namespace SevenTiny.Bantina.Bankinate.ConnectionManagement
{
    /// <summary>
    /// 连接状态描述类
    /// </summary>
    internal class ConnectionStatus
    {
        public int HashKey { get; set; }
        public string ConnectionString { get; set; }
        public int Count { get; set; }
    }
}
