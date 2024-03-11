using Common.Entities;
using Volo.Abp.DependencyInjection;

namespace Common.Interfaces
{
    /// <summary>
    /// 区域工人工作明细
    /// </summary>
    public interface ISectionWorkerDetailAppService : IBaseNetAppService<SectionWorkerDetail>, ITransientDependency
    {
        
    }
}