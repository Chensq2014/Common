using Common.Dtos;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    /// <summary>
    /// 设备反控命令公共接口
    /// </summary>
    public interface IReverseCommand : IHandlerType
    {
        string Name { get; }

        string Area { get; }

        string SupplierCode { get; }

        Task<ResponseWrapper<string>> ExecuteAsync(DeviceDto device, string body);
    }
}
