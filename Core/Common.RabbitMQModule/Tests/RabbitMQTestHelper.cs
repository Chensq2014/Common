using RabbitMQ.Client;
using System.Collections.Generic;

namespace Common.RabbitMQModule.Tests
{
    /// <summary>
    /// RabbitMQTestHelper 
    /// </summary>
    public class RabbitMQTestHelper
    {
        //单例ConnectionFactory
        public static ConnectionFactory _factory = new ConnectionFactory
        {
            //HostName = "127.0.0.1", //ip
            //Port = 5672, //5672默认数据通信端口
            UserName = "guest", //账号
            Password = "guest", //密码
            VirtualHost = "parakeet", //虚拟主机
            AutomaticRecoveryEnabled = true//自动recovery
        };
        public static ConnectionFactory Instance()
        {
            return _factory;
        }

        /// <summary>
        /// 获取RabbitMQ 集群连接对象
        /// </summary>
        /// <returns></returns>
        public static IConnection GetConnection()
        {
            //var factory = new ConnectionFactory
            //{
            //    //HostName = "127.0.0.1", //ip
            //    //Port = 5672, //5672默认数据通信端口
            //    UserName = "guest", //账号
            //    Password = "guest", //密码
            //    VirtualHost = "parakeet" //虚拟主机
            //};
            var endPoints = new List<AmqpTcpEndpoint>
            {
                new AmqpTcpEndpoint{HostName = "127.0.0.1",Port = 5672},
                //new AmqpTcpEndpoint{HostName = "127.0.0.1",Port = 5673},
            };
            return _factory.CreateConnection(endPoints);
        }
    }
}
