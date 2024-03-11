using Common.Entities;
using Volo.Abp.DependencyInjection;

namespace Common.Interfaces
{
    /// <summary>
    /// 工种服务
    /// </summary>
    public interface IWorkerTypeAppService : IBaseNetAppService<WorkerType>, ITransientDependency
    {
        
    }
}