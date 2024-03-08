using Common.Dtos;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace Common.TcpMudule.Sockets
{
    /// <summary>
    /// SocketClient 客户端
    /// </summary>
    public sealed class AsyncSocketClient : IDisposable
    {
        /// <summary>
        /// 缓冲大小 默认4Kb
        /// </summary>
        public const int BufferSize = 4 * 1024;

        public bool Connected { get; private set; } = false;
        public AsyncUserToken UserToken { get; private set; }

        public event EventHandler<SocketAsyncEventArgs> OnSocketConnectedEvent;

        public event EventHandler<SocketEventArgs> OnDataReceived;

        public event EventHandler<SocketEventArgs> OnDataSendCompleted;

        public event EventHandler<SocketException> OnError;

        public event EventHandler OnConnectClosed;

        private readonly Socket _clientSocket;
        private readonly AutoResetEvent _autoConnectEvent;
        private readonly AutoResetEvent _autoSendEvent;
        private readonly AutoResetEvent _autoReceiveEvent;

        private readonly BlockingCollection<byte[]> _sendingQueue;
        private readonly BlockingCollection<byte[]> _receivedQueue;

        private readonly Thread _messageSenderWorker;
        private readonly Thread _messageReceiverWorker;
        private bool _disposed = false;
        public AsyncSocketClient(MediatorDto mediator)
        {
            IPEndPoint endPoint;
            if (Regex.IsMatch(mediator.Host, @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$"))
            {
                endPoint = new IPEndPoint(IPAddress.Parse(mediator.Host), mediator.Port);
            }
            else
            {
                IPHostEntry host = Dns.GetHostEntry(mediator.Host);
                endPoint = new IPEndPoint(host.AddressList[0], mediator.Port);
            }

            //根据Mediator中的域名解析IP地址，new 一个Socket连接
            this._clientSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            this._autoConnectEvent = new AutoResetEvent(false);
            this._autoSendEvent = new AutoResetEvent(false);
            this._autoReceiveEvent = new AutoResetEvent(false);

            this._sendingQueue = new BlockingCollection<byte[]>();
            this._receivedQueue = new BlockingCollection<byte[]>();

            this._messageSenderWorker = new Thread(new ThreadStart(ProcessSendQueue));
            this._messageReceiverWorker = new Thread(new ThreadStart(ProcessReceivedQueue));

            this.UserToken = new AsyncUserToken(BufferSize)
            {
                ConnectSocket = this._clientSocket, ConnectEventArgs = {RemoteEndPoint = endPoint}
            };

            this.UserToken.ConnectEventArgs.Completed += Connect_Completed;

            this.UserToken.ReceiveEventArgs.RemoteEndPoint = endPoint;
            this.UserToken.ReceiveEventArgs.Completed += Receive_Completed;

            this.UserToken.SendEventArgs.RemoteEndPoint = endPoint;
            this.UserToken.SendEventArgs.Completed += Send_Completed;

            this.UserToken.Tag = mediator;
        }

        public bool ConnectAsync()
        {
            bool willRaiseEvent = _clientSocket.ConnectAsync(UserToken.ConnectEventArgs);

            _autoConnectEvent.WaitOne();

            SocketError errorCode = UserToken.ConnectEventArgs.SocketError;
            if (errorCode != SocketError.Success)
            {
                OnError?.Invoke(this, new SocketException((int)errorCode));
                return false;
            }

            if (!willRaiseEvent)
            {
                ProcessConnect(UserToken.ConnectEventArgs);
                UserToken.ConnectEventArgs.Completed -= Connect_Completed;
                UserToken.ConnectEventArgs.Dispose();
            }

            _messageSenderWorker.Start();
            _messageReceiverWorker.Start();

            return true;
        }

        public void Disconnect()
        {
            _clientSocket.Disconnect(false);
        }

        public void SendAsync(byte[] message)
        {
            _sendingQueue.Add(message);
        }

        #region 连接事件

        private void ProcessConnect(SocketAsyncEventArgs e)
        {
            var userToken = e.UserToken as AsyncUserToken;
            if (e.SocketError == SocketError.Success)
            {
                AcceptReceive(e.ConnectSocket, userToken);
            }
            else
            {
                OnError?.Invoke(this, new SocketException((int)e.SocketError));
            }
        }

        private void AcceptReceive(Socket socket, AsyncUserToken userToken)
        {
            var willRaiseEvent = _clientSocket.ReceiveAsync(UserToken.ReceiveEventArgs);
            if (!willRaiseEvent)
            {
                ProcessReceive(UserToken.ReceiveEventArgs);
            }
        }

        private void Connect_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessConnect(e);

            //触发事件;
            OnSocketConnectedEvent?.Invoke(this, e);

            e.Completed -= Connect_Completed;
            e.Dispose();

            _autoConnectEvent.Set();
            Connected = (e.SocketError == SocketError.Success);
        }

        #endregion 连接事件

        #region 发送消息

        private void Send_Completed(object sender, SocketAsyncEventArgs e)
        {
            UserToken.SendBuffer.Clear();
            _autoSendEvent.Set();
        }

        private void ProcessSendQueue()
        {
            while (true)
            {
                var buffer = _sendingQueue.Take();
                if (buffer != null && !_disposed)
                {
                    try
                    {
                        OnDataSendCompleted?.Invoke(this, new SocketEventArgs(buffer));

                        UserToken.SendEventArgs.SetBuffer(buffer, 0, buffer.Length);
                        var willRaiseEvent = _clientSocket.SendAsync(UserToken.SendEventArgs);
                        if (!willRaiseEvent)
                        {
                            this.Send_Completed(this, UserToken.SendEventArgs);
                        }
                    }
                    catch (ObjectDisposedException ex)
                    {
                        Console.WriteLine($"处理发送队列失败-异常信息：{ex.Message}-堆栈信息{ex.StackTrace}");
                        _autoSendEvent.Set();
                    }

                    _autoSendEvent.WaitOne();
                }
            }
        }

        #endregion 发送消息

        #region 接收消息

        private void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessReceive(e);
            _autoReceiveEvent.Set();
        }

        /// <summary>
        ///接收完成时处理函数
        /// </summary>
        /// <param name="e">与接收完成操作相关联的SocketAsyncEventArg对象</param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            var userToken = e.UserToken as AsyncUserToken;

            if (userToken?.ConnectSocket == null)
            {
                return;
            }

            userToken.ActiveTime = DateTime.Now;

            if (userToken.ReceiveEventArgs.BytesTransferred > 0 && userToken.ReceiveEventArgs.SocketError == SocketError.Success)
            {
                var socket = userToken.ConnectSocket;

                //把收到的数据写入到缓存区里面
                userToken.ReceiveBuffer.WriteBuffer(e.Buffer, e.Offset, e.BytesTransferred);

                if (socket.Available == 0)
                {
                    _receivedQueue.Add(userToken.ReceiveBuffer.CleanBuffer);
                    UserToken.ReceiveBuffer.Clear();
                }

                //为接收下一段数据，投递接收请求，这个函数有可能同步完成，
                //这时返回false，并且不会引发SocketAsyncEventArgs.Completed事件
                if (socket.Connected && !socket.ReceiveAsync(userToken.ReceiveEventArgs))
                {
                    //同步接收时处理接收完成事件
                    ProcessReceive(userToken.ReceiveEventArgs);
                }
            }
            else
            {
                Close(userToken);
            }
        }

        /// <summary>
        /// 处理接收消息队列
        /// </summary>
        private void ProcessReceivedQueue()
        {
            while (true)
            {
                var buffer = _receivedQueue.Take();
                if (buffer == null || _disposed)
                {
                    continue;
                }

                OnDataReceived?.Invoke(this, new SocketEventArgs(buffer));
                _autoReceiveEvent.WaitOne();
            }
        }

        #endregion 接收消息

        #region 错误消息
        private void ProcessError(SocketAsyncEventArgs e)
        {
            if (e.UserToken is Socket socket && socket.Connected)
            {
                // close the socket associated with the client
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception)
                {
                    // throws if client process has already closed
                    OnError?.Invoke(this, new SocketException((int)e.SocketError));
                }
                finally
                {
                    if (socket.Connected)
                    {
                        socket.Close();
                    }
                }
            }
        }

        #endregion

        #region Close

        /// <summary>
        /// 关闭socket连接
        /// </summary>
        public void Close()
        {
            Close(this.UserToken);
        }

        /// <summary>
        /// 关闭socket连接
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed send/receive operation.</param>
        public void Close(AsyncUserToken userToken)
        {
            if (userToken.ConnectSocket == null || !userToken.ConnectSocket.Connected)
            {
                this.Connected = false;
                return;
            }

            try
            {
                userToken.ConnectSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception)
            {
                // Throw if client has closed, so it is not necessary to catch.
                OnError?.Invoke(this, new SocketException((int)SocketError.Shutdown));
            }
            finally
            {
                this.Connected = false;
                userToken.ConnectSocket.Close();
            }

            //释放引用，并清理缓存，包括释放协议对象等资源
            userToken.ConnectSocket = null;
            OnConnectClosed?.Invoke(this, new EventArgs());
        }

        #endregion Close

        #region IDisposable Members


        public void Dispose()
        {
            try
            {
                this._autoConnectEvent.Close();
                this._autoSendEvent.Close();
                this._autoReceiveEvent.Close();
                this.UserToken = null;

                if (this._clientSocket != null && this._clientSocket.Connected)
                {
                    this._clientSocket.Close();
                }

                _disposed = true;

                GC.SuppressFinalize(this);
            }
            catch (Exception)
            {
                OnError?.Invoke(this, new SocketException((int)SocketError.Shutdown));
            }
        }

        #endregion IDisposable Members
    }
}
