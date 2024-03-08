using Parakeet.NetCore.Consumer.Interfaces;
using Parakeet.NetCore.Dtos;

namespace Parakeet.NetCore.Consumer.Chongqing.Interfaces
{
    /// <summary>
    /// 环境转发接口
    /// </summary>
    public interface IEnvironmentRecordHttpForward : IHttpForward<EnvironmentRecordDto>
    {
    }
}
