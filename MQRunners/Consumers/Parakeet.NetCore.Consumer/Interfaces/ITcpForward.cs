using System;
using System.Collections.Generic;
using System.Text;
using Parakeet.NetCore.Dtos;

namespace Parakeet.NetCore.Consumer.Interfaces
{
    /// <summary>
    /// Tcp转发基类接口
    /// </summary>
    /// <typeparam name="TDeviceRecordDto"></typeparam>
    public interface ITcpForward<TDeviceRecordDto> : IForward<TDeviceRecordDto> where TDeviceRecordDto : DeviceRecordDto
    {

    }
}
