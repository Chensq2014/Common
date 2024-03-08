using Parakeet.NetCore.Consumer.Interfaces;
using Parakeet.NetCore.Dtos;

namespace Parakeet.NetCore.Consumer.Standard.Interfaces
{
    /// <summary>
    /// 环境转发接口
    /// </summary>
    public interface IEnvironmentRecordHttpForward : IHttpForward<EnvironmentRecordDto>
    {
    }
}
