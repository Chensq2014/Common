using Common.Entities;
using Volo.Abp.DependencyInjection;

namespace Common.Interfaces
{
    /// <summary>
    /// 小区区域服务
    /// </summary>
    public interface ISectionAppService : IBaseNetAppService<Section>, ITransientDependency
    {
        
    }
}