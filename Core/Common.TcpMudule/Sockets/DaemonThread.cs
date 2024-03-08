using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Common.TcpMudule.Sockets
{
    /// <summary>
    /// 守护进/线程
    /// </summary>
    internal class DaemonThread
    {
        private readonly Thread thread;
        private readonly AsyncIOCPServer server;
        private readonly ILogger logger;

        public DaemonThread(AsyncIOCPServer server, ILogger<DaemonThread> logger)
        {
            this.logger = logger;
            this.server = server;
            this.thread = new Thread(DaemonThreadStart);
            this.thread.Start();
        }

        public void DaemonThreadStart()
        {
            while (thread.IsAlive)
            {
                AsyncUserToken[] userTokenArray = null;
                server.AsyncSocketUserTokenList.CopyList(ref userTokenArray);
                for (int i = 0; i < userTokenArray.Length; i++)
                {
                    if (!thread.IsAlive)
                    {
                        break;
                    }

                    try
                    {
                        //超时Socket断开
                        if ((DateTime.Now - userTokenArray[i].ActiveTime).TotalMilliseconds > server.Timeout)
                        {
                            lock (userTokenArray[i])
                            {
                                server.Close(userTokenArray[i]);
                            }
                        }

                        logger.LogInformation($"[筑智建物联网平台]==========当前共有{userTokenArray.Length}个客户端连接");
                    }
                    catch (SemaphoreFullException ex)
                    {
                        logger.LogError(ex, string.Format("Daemon thread check timeout socket error, message: {0}", ex.Message));
                    }
                }

                //每2分钟检测一次
                for (int i = 0; i < 120; i++)
                {
                    if (!thread.IsAlive)
                    {
                        break;
                    }

                    Thread.Sleep(1000);
                }
            }
        }

        public void Close()
        {
            thread.Abort();
            thread.Join();
        }
    }
}
