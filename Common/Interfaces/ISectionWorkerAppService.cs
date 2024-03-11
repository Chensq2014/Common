using Common.Entities;
using Volo.Abp.DependencyInjection;

namespace Common.Interfaces
{
    /// <summary>
    /// 区域工人服务
    /// </summary>
    public interface ISectionWorkerAppService : IBaseNetAppService<SectionWorker>, ITransientDependency
    {
        
    }
}