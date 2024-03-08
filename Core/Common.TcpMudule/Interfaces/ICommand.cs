using Common.Dtos;
using Common.Interfaces;
using System;
using System.Net.Sockets;

namespace Common.TcpMudule.Interfaces
{
    /// <summary>
    /// 命令接口
    /// </summary>
    public interface ICommand : IHandlerType
    {
        /// <summary>
        /// 设备信息
        /// </summary>
        DeviceDto Device { get; set; }

        /// <summary>
        /// 命令处理二进制数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        byte[] Execute(byte[] request,Action<SocketAsyncEventArgs,byte[]> action);
    }
}
