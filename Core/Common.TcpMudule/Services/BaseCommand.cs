using Common.Dtos;
using Common.Entities;
using Common.TcpMudule.Interfaces;
using System;
using System.Net.Sockets;

namespace Common.TcpMudule.Services
{
    /// <summary>
    /// 命令接口ICommand的抽象类
    /// </summary>
    public abstract class BaseCommand : BaseHandlerType, ICommand
    {
        /// <summary>
        /// 设备信息
        /// </summary>
        public DeviceDto Device { get; set; }

        /// <summary>
        /// 抽象处理类
        /// </summary>
        /// <param name="request"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public abstract byte[] Execute(byte[] request, Action<SocketAsyncEventArgs, byte[]> action);
    }
}
