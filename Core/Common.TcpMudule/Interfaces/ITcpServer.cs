using Common.TcpMudule.Sockets;
using System;
using System.Net.Sockets;

namespace Common.TcpMudule.Interfaces
{
    /// <summary>
    /// TCP服务
    /// </summary>
    public interface ITcpServer : IDisposable
    {
        void Start();

        void SendAsync(SocketAsyncEventArgs e, byte[] data);

        void Stop();

        void Close(AsyncUserToken userToken);

        event EventHandler<SocketEventArgs> OnReceiveCompleted;
    }
}
