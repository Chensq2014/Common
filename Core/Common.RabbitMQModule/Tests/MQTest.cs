using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Common.RabbitMQModule.Core;
using RabbitMQ.Client;

namespace Common.RabbitMQModule.Tests
{
    /// <summary>
    /// MQ测试类
    /// </summary>
    public class MQTest : IMQTest
    {
        private readonly RabbitMQOptions _rabbitMQoption;
        public MQTest(IOptionsMonitor<RabbitMQOptions> option)
        {
            _rabbitMQoption = option.CurrentValue;
        }

        /// <summary>
        /// 获取连接
        /// </summary>
        /// <returns>RabbitMQ.Client.IConnection</returns>
        public IConnection GetConnection()
        {
            var factory = new ConnectionFactory
            {
                //HostName = "127.0.0.1", //ip
                //Port = 5672, //5672默认数据通信端口
                UserName = _rabbitMQoption.UserName, //账号
                Password = _rabbitMQoption.Password, //密码
                VirtualHost = _rabbitMQoption.VirtualHost, //虚拟主机
                AutomaticRecoveryEnabled = true//自动recovery
            };
            var endPoints = new List<AmqpTcpEndpoint>();
            foreach (var host in _rabbitMQoption.Hosts)
            {
                if (host.Contains(":"))
                {
                    endPoints.Add(AmqpTcpEndpoint.Parse(host));
                }
                else
                {
                    endPoints.Add(new AmqpTcpEndpoint
                    {
                        HostName = host,
                        Port = _rabbitMQoption.Port ?? 5672
                    });
                }
            }
            return factory.CreateConnection(endPoints);
        }
    }
}
