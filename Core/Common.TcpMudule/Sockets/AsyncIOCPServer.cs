using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Common.Extensions;
using Common.TcpMudule.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Common.TcpMudule.Sockets
{
    /// <summary>
    /// Socket实现IOCP异步服务器 TCP服务默认实现 比较稳定
    /// </summary>
    public class AsyncIOCPServer : ITcpServer
    {
        #region Fields

        /// <summary>
        /// 服务器程序允许的最大客户端连接数
        /// </summary>
        private static readonly int _maxPool = 1024;

        /// <summary>
        /// 监听Socket，用于接受客户端的连接请求
        /// </summary>
        private Socket _serverSock;

        /// <summary>
        /// 当前的连接的客户端数
        /// </summary>
        private int _clientCount;

        /// <summary>
        /// 信号量
        /// </summary>
        private static readonly Semaphore _maxSemaphoreClients = new Semaphore(_maxPool, _maxPool);

        /// <summary>
        /// 对象池
        /// </summary>
        private readonly AsyncUserTokenPool _userTokenPool;

        /// <summary>
        /// 是否销毁
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// 日志记录
        /// </summary>
        private readonly ILogger _logger;

        private readonly ILoggerFactory _loggerFactory;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 服务器是否正在运行
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// 监听的IP地址
        /// </summary>
        public IPAddress Address { get; private set; }

        /// <summary>
        /// 监听的端口
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// 通信使用的编码
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// 缓冲大小
        /// </summary>
        public int BufferSize { get; set; } = 4 * 1024;

        /// <summary>
        /// 连接超时时间
        /// </summary>
#if DEBUG
        public int Timeout { get; set; } = 2 * 60 * 1000;
#else
        public int Timeout { get; set; } = 5 * 60 * 1000;
#endif

        /// <summary>
        /// 客户端信息集合
        /// </summary>
        public AsyncUserTokenCollection AsyncSocketUserTokenList { get; }

        public event EventHandler<SocketEventArgs> OnReceiveCompleted;

        #endregion Properties

        #region 构造函数

        /// <summary>
        /// 异步Socket TCP服务器
        /// </summary>
        /// <param name="localIPAddress">监听的IP地址</param>
        /// <param name="listenPort">监听的端口</param>
        /// <param name="maxClient">最大客户端数量</param>
        public AsyncIOCPServer(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.Address = IPAddress.Parse(configuration.GetValue<string>($"{Magics.TCPOPTIONS}:Host"));
            this.Port = configuration.GetValue<int>($"{Magics.TCPOPTIONS}:Port");
            this.Encoding = Encoding.UTF8;

            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<AsyncIOCPServer>();

            _serverSock = new Socket(this.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            AsyncSocketUserTokenList = new AsyncUserTokenCollection();

            _userTokenPool = new AsyncUserTokenPool(_maxPool);

            //_maxSemaphoreClients = new Semaphore(_maxPool, _maxPool);
        }

        #endregion 构造函数

        #region Method

        /// <summary>
        /// 初始化函数
        /// </summary>
        public void Init()
        {
            AsyncUserToken userToken;
            for (int i = 0; i < _maxPool; i++)
            {
                userToken = new AsyncUserToken(BufferSize);
                userToken.ReceiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
                userToken.SendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
                _userTokenPool.Push(userToken);
            }
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            if (!IsRunning)
            {
                Init();

                IsRunning = true;

                IPEndPoint localEndPoint = new IPEndPoint(Address, Port);
                // 创建监听socket
                _serverSock = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                if (localEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    // 配置监听socket为 dual-mode (IPv4 & IPv6)
                    // 27 is equivalent to IPV6_V6ONLY socket option in the winsock snippet below,
                    _serverSock.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, false);
                    _serverSock.Bind(new IPEndPoint(IPAddress.IPv6Any, localEndPoint.Port));
                }
                else
                {
                    _serverSock.Bind(localEndPoint);
                }

                // 开始监听
                _logger.LogInformation($"TCP服务开启监听端口{this.Port},允许最大连接数为{_maxPool}");
                _serverSock.Listen(_maxPool);

                // 在监听Socket上投递一个接受请求。
                StartAccept(null);

                //开启监测线程
                _ = new DaemonThread(this, _loggerFactory.CreateLogger<DaemonThread>());
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                _serverSock.Close();
                //TODO 关闭对所有客户端的连接
            }
        }

        /// <summary>
        /// 从客户端开始接受一个连接操作
        /// </summary>
        private void StartAccept(SocketAsyncEventArgs asyniar)
        {
            if (asyniar == null)
            {
                asyniar = new SocketAsyncEventArgs();
                asyniar.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            }
            else
            {
                //socket must be cleared since the context object is being reused
                asyniar.AcceptSocket = null;
            }

            _maxSemaphoreClients.WaitOne();
            if (!_serverSock.AcceptAsync(asyniar))
            {
                ProcessAccept(asyniar);
                //如果I/O挂起等待异步则触发AcceptAsyn_Asyn_Completed事件
                //此时I/O操作同步完成，不会触发Asyn_Completed事件，所以指定BeginAccept()方法
            }
        }

        /// <summary>
        /// accept 操作完成时回调函数
        /// </summary>
        /// <param name="sender">Object who raised the event.</param>
        /// <param name="e">SocketAsyncEventArg associated with the completed accept operation.</param>
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        /// <summary>
        /// 监听Socket接受处理
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed accept operation.</param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                //和客户端关联的socket
                Socket socket = e.AcceptSocket;
                if (socket.Connected)
                {
                    try
                    {
                        //原子操作加1
                        Interlocked.Increment(ref _clientCount);

                        AsyncUserToken userToken = _userTokenPool.Pop();
                        userToken.ConnectSocket = socket;

                        AsyncSocketUserTokenList.Add(userToken); //添加到正在连接列表

                        _logger.LogInformation($"设备 {socket.RemoteEndPoint} 连入, 共有 {_clientCount} 个连接。");

                        if (!socket.ReceiveAsync(userToken.ReceiveEventArgs))//投递接收请求
                        {
                            ProcessReceive(userToken.ReceiveEventArgs);
                        }
                    }
                    catch (SocketException ex)
                    {
                        _logger.LogError(ex, string.Format($"接收设备 {socket.RemoteEndPoint} 数据出错, 异常信息： {ex} 。"));
                        //TODO 异常处理
                    }

                    //投递下一个接受请求
                    StartAccept(e);
                }
            }
        }

        /// <summary>
        /// 异步的发送数据
        /// </summary>
        /// <param name="e"></param>
        /// <param name="data"></param>
        public void SendAsync(SocketAsyncEventArgs e, byte[] data)
        {
            if (data == null)
            {
                _logger.LogInformation("无返回数据...");
                return;
            }

            AsyncUserToken userToken = e.UserToken as AsyncUserToken;

#if DEBUG
            _logger.LogInformation($"向连接客户端[{userToken.ConnectSocket?.RemoteEndPoint}]设备发送数据{ Encoding.UTF8.GetString(data)}");
#endif

            userToken.SendBuffer.WriteBuffer(data, 0, data.Length);//写入要发送的数据
            if (userToken.SendEventArgs.SocketError == SocketError.Success)
            {
                if (userToken.ConnectSocket != null && userToken.ConnectSocket.Connected)
                {
                    //设置发送数据
                    //userToken.SendEventArgs.SetBuffer(userToken.SendBuffer.Buffer,0,userToken.SendBuffer.DataCount);
                    e.SetBuffer(data, 0, data.Length);
                    //Array.Copy(data, 0, e.Buffer, 0, data.Length);
                    //投递发送请求，这个函数有可能同步发送出去，这时返回false，并且不会引发SocketAsyncEventArgs.Completed事件

                    if (!userToken.ConnectSocket.SendAsync(userToken.SendEventArgs))
                    {
                        // 同步发送时处理发送完成事件
                        ProcessSend(userToken.SendEventArgs);
                    }

                    userToken.SendBuffer.Clear();
                }
                else
                {
                    _logger.LogWarning($"设备{userToken.Tag}已断开");

                    Close(userToken);
                }
            }
            else
            {
                Close(userToken);
            }
        }

        /// <summary>
        /// 同步的使用socket发送数据
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <param name="timeout"></param>
        public void Send(Socket socket, byte[] buffer, int offset, int size, int timeout)
        {
            socket.SendTimeout = 0;
            int startTickCount = Environment.TickCount;
            int sent = 0; // how many bytes is already sent
            do
            {
                if (Environment.TickCount > startTickCount + timeout)
                {
                    //throw new Exception("Timeout.");
                }
                try
                {
                    sent += socket.Send(buffer, offset + sent, size - sent, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
                    ex.SocketErrorCode == SocketError.IOPending ||
                    ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
                    {
                        // socket buffer is probably full, wait and try again
                        Thread.Sleep(30);
                    }
                    else
                    {
                        throw ex; // any serious error occurr
                    }
                }
            } while (sent < size);
        }

        /// <summary>
        /// 发送完成时处理函数
        /// </summary>
        /// <param name="e">与发送完成操作相关联的SocketAsyncEventArg对象</param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            AsyncUserToken userToken = e.UserToken as AsyncUserToken;
            userToken.ActiveTime = DateTime.Now;

            if (userToken.SendEventArgs.SocketError == SocketError.Success)
            {
                _logger.LogInformation(userToken.ConnectSocket?.RemoteEndPoint + "发送完毕");
            }
            else
            {
                Close(userToken);
            }
        }

        /// <summary>
        ///接收完成时处理函数
        /// </summary>
        /// <param name="e">与接收完成操作相关联的SocketAsyncEventArg对象</param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.UserToken == null)
            {
                return;
            }
            //AsyncUserToken 自定义类  从e.UserToken 这个设备传递的object对象中转化
            AsyncUserToken userToken = e.UserToken as AsyncUserToken;

            if (userToken == null)
            {
                return;
            }

            if (userToken.ConnectSocket == null)
            {
                return;
            }

            userToken.ActiveTime = DateTime.Now;

            if (userToken.ReceiveEventArgs.BytesTransferred > 0 && userToken.ReceiveEventArgs.SocketError == SocketError.Success)
            {
                try
                {
                    Socket socket = userToken.ConnectSocket;

                    //把收到的数据写入到缓存区里面
                    userToken.ReceiveBuffer.WriteBuffer(e.Buffer, e.Offset, e.BytesTransferred);

                    //判断所有需接收的数据是否已经完成
                    if (socket.Available == 0)
                    {
                        var buffer = userToken.ReceiveBuffer.CleanBuffer;
                        if (buffer == null || buffer.Length < 2)
                        {
                            return;
                        }

                        OnReceiveCompleted?.Invoke(userToken, new SocketEventArgs(buffer));

#if DEBUG
                        var info = buffer.ToHexString();
                        _logger.LogInformation(string.Format("收到设备 {0} 数据为 [{1}], 数据传输量为 {2}", socket?.RemoteEndPoint.ToString(), info, e?.BytesTransferred));
#endif

                        //                    var header = Encoding.UTF8.GetString(new byte[] { buffer[0], buffer[1], buffer[2], buffer[3] });
                        //                    var packetHandler = serviceProvider.Resolve<IPacketHandler>(DeviceHandlerPool.Instance[header]);
                        //                    if (packetHandler != null)
                        //                    {
                        //                        var packageStructure = packetHandler.Parse(buffer);
                        //                        if (packageStructure.Total <= userToken.ReceiveBuffer.Size)
                        //                        {
                        //                            var result = packetHandler?.Handle(userToken, SendAsync);
                        //                            SendAsync(userToken.SendEventArgs, result);
                        //                            userToken.ReceiveBuffer.Clear();
                        //                        }
                        //                    }
                        //                    else
                        //                    {
                        //                        userToken.ReceiveBuffer.Clear();
                        //                        _logger.LogError($"设备{header}不存在...");
                        //                    }
                    }

                    //为接收下一段数据，投递接收请求，这个函数有可能同步完成，这时返回false，并且不会引发SocketAsyncEventArgs.Completed事件
                    if (socket.Connected && !socket.ReceiveAsync(userToken.ReceiveEventArgs))
                    {
                        //同步接收时处理接收完成事件
                        ProcessReceive(userToken.ReceiveEventArgs);
                    }
                }
                catch (ObjectDisposedException)
                {
                    _logger.LogWarning("[ProcessReceive]Socket已被dispose....");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[ProcessReceive]接收完成时处理函数发生异常");
                }
            }
            else
            {
                Close(userToken);
            }
        }

        /// <summary>
        /// 当Socket上的发送或接收请求被完成时，调用此函数
        /// </summary>
        /// <param name="sender">激发事件的对象</param>
        /// <param name="e">与发送或接收完成操作相关联的SocketAsyncEventArg对象</param>
        private void OnIOCompleted(object sender, SocketAsyncEventArgs e)
        {
            // Determine which type of operation just completed and call the associated handler.
            if (e.UserToken is AsyncUserToken userToken)
            {
                userToken.ActiveTime = DateTime.Now;

                lock (userToken)
                {
                    switch (e.LastOperation)
                    {
                        case SocketAsyncOperation.Accept:
                            ProcessAccept(e);
                            break;

                        case SocketAsyncOperation.Receive:
                            ProcessReceive(e);
                            break;
                        //FIXME 此处有可能触发socket断线
                        case SocketAsyncOperation.Send:
                            ProcessSend(e);
                            break;
                    }
                }
            }
        }

        #endregion Method

        #region Close

        /// <summary>
        /// 关闭socket连接
        /// </summary>
        /// <param name="e">SocketAsyncEventArg associated with the completed send/receive operation.</param>
        public void Close(AsyncUserToken userToken)
        {
            try
            {
                var count = _userTokenPool.Count;

                try
                {
                    _logger.LogInformation($"MAXPOOL最大连接池数[{_maxPool}],连接池数剩余数[{count}],现有客户端连接数[{_clientCount}],");

                    if (userToken.ConnectSocket == null)
                    {
                        Interlocked.Decrement(ref _clientCount);
                        //释放引用，并清理缓存，包括释放协议对象等资源
                        _maxSemaphoreClients.Release();
                        _userTokenPool.Push(userToken);
                        AsyncSocketUserTokenList.Remove(userToken);
                    }
                    else
                    {
                        _logger.LogWarning($"设备 {userToken.ConnectSocket?.RemoteEndPoint} 断开连接!");
                        try
                        {
                            userToken.ConnectSocket.Shutdown(SocketShutdown.Both);
                        }
                        catch (Exception ex)
                        {
                            // Throw if client has closed, so it is not necessary to catch.
                            _logger.LogWarning($"client has closed message:{ex.Message}");
                        }
                        finally
                        {
                            userToken.ConnectSocket.Close();
                        }

                        Interlocked.Decrement(ref _clientCount);
                        //释放引用，并清理缓存，包括释放协议对象等资源
                        userToken.ConnectSocket = null;
                        _maxSemaphoreClients.Release();
                        _userTokenPool.Push(userToken);
                        AsyncSocketUserTokenList.Remove(userToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "未知错误");
                }
                finally
                {
                    if (_maxPool - (count + _clientCount) > 100)
                    {
                        _logger.LogWarning("It is too sad to here! This place may make some error~~");
                        for (int i = 0; i < 100; i++)
                        {
                            var newUserToken = new AsyncUserToken(BufferSize);
                            newUserToken.ReceiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
                            newUserToken.SendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
                            _userTokenPool.Push(newUserToken);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "未知错误");
            }
        }

        #endregion Close

        #region 释放

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release
        /// both managed and unmanaged resources; <c>false</c>
        /// to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    try
                    {
                        Stop();

                        if (_serverSock != null)
                        {
                            _serverSock = null;
                        }
                    }
                    catch (SocketException ex)
                    {
                        //TODO 事件
                        _logger.LogError(ex, "dispose failed");
                    }
                }

                _disposed = true;
            }
        }

        #endregion 释放
    }
}
