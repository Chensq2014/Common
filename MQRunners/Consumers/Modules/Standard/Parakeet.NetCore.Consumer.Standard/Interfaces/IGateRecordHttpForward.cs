using Parakeet.NetCore.Consumer.Interfaces;
using Parakeet.NetCore.Dtos;

namespace Parakeet.NetCore.Consumer.Standard.Interfaces
{
    /// <summary>
    /// 闸机考勤转发接口
    /// </summary>
    public interface IGateRecordHttpForward : IHttpForward<GateRecordDto>
    {

    }
}
