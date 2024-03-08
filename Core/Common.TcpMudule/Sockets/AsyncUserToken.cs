using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Common.TcpMudule.Sockets
{
    /// <summary>
    /// 用户传输的自定义对象
    /// 注意事项:一个Socket的Send和Receive最好分别对应一个SocketAsyncEventArgs
    /// </summary>
    public class AsyncUserToken
    {
        #region 字段

        /// <summary>
        /// 接收数据的缓冲区
        /// </summary>
        private byte[] _asyncReceiveBuffer;

        /// <summary>
        /// 发送数据的缓冲区
        /// </summary>
        private byte[] _asyncSendBuffer;

        #endregion 字段


        #region 属性

        /// <summary>
        /// 连接SocketAsyncEventArgs
        /// </summary>
        public SocketAsyncEventArgs ConnectEventArgs { get; set; }

        /// <summary>
        /// 接收数据的SocketAsyncEventArgs
        /// </summary>
        public SocketAsyncEventArgs ReceiveEventArgs { get; set; }

        /// <summary>
        /// 发送数据的SocketAsyncEventArgs
        /// </summary>
        public SocketAsyncEventArgs SendEventArgs { get; set; }

        /// <summary>
        /// 连接的Socket对象
        /// </summary>
        public Socket ConnectSocket { get; set; }

        /// <summary>
        /// 动态的接收缓冲区
        /// </summary>
        public DynamicBufferManager ReceiveBuffer { get; set; }

        /// <summary>
        /// 动态的发送缓冲区
        /// </summary>
        public DynamicBufferManager SendBuffer { get; set; }

        /// <summary>
        /// 活动时间
        /// </summary>
        public DateTime ActiveTime { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public object Tag { get; set; } = Guid.NewGuid().ToString();

        #endregion 属性


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="receiveBufferSize"></param>
        public AsyncUserToken(int receiveBufferSize)
        {
            ConnectSocket = null;

            ConnectEventArgs = new SocketAsyncEventArgs {UserToken = this};

            _asyncReceiveBuffer = new byte[receiveBufferSize];
            ReceiveEventArgs = new SocketAsyncEventArgs {UserToken = this};
            ReceiveEventArgs.SetBuffer(_asyncReceiveBuffer, 0, _asyncReceiveBuffer.Length);//设置接收缓冲区

            _asyncSendBuffer = new byte[receiveBufferSize];
            SendEventArgs = new SocketAsyncEventArgs {UserToken = this};
            SendEventArgs.SetBuffer(_asyncSendBuffer, 0, _asyncSendBuffer.Length);//设置发送缓冲区

            ReceiveBuffer = new DynamicBufferManager(receiveBufferSize);
            SendBuffer = new DynamicBufferManager(receiveBufferSize);
        }
    }
}
