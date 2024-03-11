using Common.Entities;
using Volo.Abp.DependencyInjection;

namespace Common.Interfaces
{
    /// <summary>
    /// 房间服务
    /// </summary>
    public interface IHouseAppService : IBaseNetAppService<House>, ITransientDependency
    {
        
    }
}