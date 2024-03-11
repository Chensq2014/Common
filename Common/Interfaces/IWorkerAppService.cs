using Common.Entities;
using Volo.Abp.DependencyInjection;

namespace Common.Interfaces
{
    /// <summary>
    /// 工人服务
    /// </summary>
    public interface IWorkerAppService : IBaseNetAppService<Worker>, ITransientDependency
    {
        
    }
}