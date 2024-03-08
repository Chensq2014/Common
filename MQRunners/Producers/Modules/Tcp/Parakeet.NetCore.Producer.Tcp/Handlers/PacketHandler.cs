using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Parakeet.NetCore.Entities;
using Parakeet.NetCore.TcpMudule.Interfaces;
using Parakeet.NetCore.TcpMudule.Services;
using Parakeet.NetCore.TcpMudule.Sockets;

namespace Parakeet.NetCore.Producer.Tcp.Handlers
{
    /// <summary>
    /// 包解析Handler
    /// </summary>
    public class PacketHandler : BaseHandlerType, IPacketHandler
    {
        private readonly ILogger _logger;
        private readonly ICommandFactory _commandFactory;

        public PacketHandler(ICommandFactory<EnvironmentTcpModule> commandFactory, ILogger<PacketHandler> logger)
        {
            _logger = logger;
            _commandFactory = commandFactory;
        }
        public PacketStructure Parse(byte[] buffer, int receiveSize)
        {
            var shortMessage = Encoding.UTF8.GetString(new byte[] { buffer[0], buffer[1], buffer[2], buffer[3], buffer[4], buffer[5] });

            var packetStructure = new PacketStructure
            {
                Header = shortMessage.Substring(0, 2), 
                Length = Convert.ToInt32(shortMessage.Substring(2, 4))
            };

            packetStructure.Total = packetStructure.Length + 8;
            packetStructure.Completed = packetStructure.Total <= receiveSize;

            return packetStructure;
        }

        public HandlerResult Handle(AsyncUserToken userToken, Action<SocketAsyncEventArgs, byte[]> action)
        {
            var buffer = userToken.ReceiveBuffer.CleanBuffer;
#if DEBUG
            var info = Encoding.UTF8.GetString(buffer);
            _logger.LogInformation($"收到设备 {userToken.ConnectSocket?.RemoteEndPoint} 数据为 {info}, 数据传输量为 {userToken.ReceiveEventArgs.BytesTransferred}");
#endif

            _logger.LogInformation("[环境监测]开始处理数据...");

            var wrapper = _commandFactory.Create(userToken, buffer);
            if (wrapper.Command != null)
            {
                var result = wrapper.Command.Execute(buffer, action);
                userToken.ReceiveBuffer.Clear();
                return new HandlerResult(result, wrapper.Status);
            }

            return HandlerResult.Break();
        }
    }
}
