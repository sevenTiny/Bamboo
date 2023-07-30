using SevenTiny.Bantina.Bankinate.Configs;
using SevenTiny.Bantina.Bankinate.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SevenTiny.Bantina.Bankinate.ConnectionManagement
{
    /// <summary>
    /// 数据库连接管理器
    /// </summary>
    public class ConnectionManager
    {
        internal ConnectionManager(string connectionString_Write, string[] connectionStrings_Read)
        {
            this.ConnectionString_Write = connectionString_Write;
            this.ConnectionStrings_Read = connectionStrings_Read?.Distinct()?.ToArray();
            //默认当前连接字符串是写
            this.CurrentConnectionString = connectionString_Write;

            //初始化
            //如果写字符串不需要负载策略，则直接返回
            if (ConnectionStrings_Read == null || !ConnectionStrings_Read.Any() || ConnectionStrings_Read.Length == 1)
                return;

            connectionStatuses = new List<ConnectionStatus>();

            //初始化连接使用情况集合
            ConnectionStrings_Read.Distinct().ToArray().Foreach(item => connectionStatuses.Add(new ConnectionStatus { HashKey = item.GetHashCode(), ConnectionString = item, Count = 0 }));
        }
        /// <summary>
        /// 写数据的连接字符串
        /// </summary>
        public string ConnectionString_Write { get; private set; }
        /// <summary>
        /// 读数据的连接字符串
        /// </summary>
        public string[] ConnectionStrings_Read { get; private set; }
        /// <summary>
        /// 连接负载均衡类型
        /// </summary>
        public LoadBalanceStrategy ConnectionLoadBalanceStrategy { get; protected set; } = LoadBalanceStrategy.LeastConnection;
        /// <summary>
        /// 当前使用的连接字符串
        /// </summary>
        public string CurrentConnectionString { get; private set; }
        /// <summary>
        /// 下次执行要用的连接字符串
        /// </summary>
        public string NextConnectionString { get; private set; }

        /// <summary>
        /// 连接字符串使用情况
        /// </summary>
        IList<ConnectionStatus> connectionStatuses = null;

        /// <summary>
        /// 设置下次执行要使用的连接字符串，效果保持一次查询！
        /// DESC：采用该功能可以在执行查询过程中手动切换执行下次查询的数据库
        /// </summary>
        /// <param name="connectionNext"></param>
        public void SetNextConnectionString(string connectionNext) => NextConnectionString = connectionNext;

        /// <summary>
        /// 设置连接字符串
        /// </summary>
        /// <param name="operationType"></param>
        /// <returns></returns>
        internal string SetConnectionString(OperationType operationType)
        {
            //写
            if (operationType == OperationType.Write)
            {
                CurrentConnectionString = ConnectionString_Write;
                return CurrentConnectionString;
            }

            //读
            //先校验下次执行的连接字符串
            if (!string.IsNullOrEmpty(NextConnectionString))
            {
                CurrentConnectionString = NextConnectionString;
                NextConnectionString = string.Empty;
                return CurrentConnectionString;
            }

            if (ConnectionStrings_Read == null || !ConnectionStrings_Read.Any())
            {
                CurrentConnectionString = ConnectionString_Write;
            }
            else if (ConnectionStrings_Read.Length == 1)
            {
                CurrentConnectionString = ConnectionStrings_Read[0];
            }
            else
            {
                if (connectionStatuses == null)
                    throw new NullReferenceException("Connection status list is null,please call Init() first!");

                //根据策略选取对应的连接字符串
                switch (ConnectionLoadBalanceStrategy)
                {
                    case LoadBalanceStrategy.RoundRobin:
                        CurrentConnectionString = GetByRoundRobin();
                        break;
                    case LoadBalanceStrategy.LeastConnection:
                        CurrentConnectionString = LeastConnection();
                        break;
                    default:
                        CurrentConnectionString = LeastConnection();
                        break;
                }
            }
            return CurrentConnectionString;
        }

        /// <summary>
        /// 轮询获取
        /// </summary>
        /// <returns></returns>
        private string GetByRoundRobin()
        {
            var current = connectionStatuses.FirstOrDefault(t => t.HashKey == CurrentConnectionString.GetHashCode());
            if (current == null)
                throw new KeyNotFoundException("current connection not fount in connection strings,please check the connection list has been change");

            //获取当前元素索引
            int currentIndex = connectionStatuses.IndexOf(current);

            if (currentIndex < connectionStatuses.Count)
                return connectionStatuses.ElementAt(currentIndex + 1).ConnectionString;
            else
                return connectionStatuses.First().ConnectionString;
        }

        /// <summary>
        /// 最小连接获取
        /// </summary>
        /// <returns></returns>
        private string LeastConnection()
        {
            var current = connectionStatuses.OrderBy(t => t.Count).First();
            current.Count++;
            return current.ConnectionString;
        }
    }
}
