using Common.Interfaces;
using Common.TcpMudule.Services;
using Common.TcpMudule.Sockets;
using System;
using System.Net.Sockets;

namespace Common.TcpMudule.Interfaces
{
    /// <summary>
    /// 包头处理接口
    /// </summary>
    public interface IPacketHandler : IHandlerType
    {
        PacketStructure Parse(byte[] buffer, int receiveSize);
        HandlerResult Handle(AsyncUserToken userToken, Action<SocketAsyncEventArgs, byte[]> action);
    }
}
