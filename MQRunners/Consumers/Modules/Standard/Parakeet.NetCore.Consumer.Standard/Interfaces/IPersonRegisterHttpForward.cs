using Parakeet.NetCore.Consumer.Interfaces;
using Parakeet.NetCore.Dtos;

namespace Parakeet.NetCore.Consumer.Standard.Interfaces
{
    /// <summary>
    /// 注册人员接口
    /// </summary>
    public interface IPersonRegisterHttpForward : IHttpForward<DeviceWorkerDto>
    {
    }
}
