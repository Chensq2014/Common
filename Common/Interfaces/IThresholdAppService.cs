using Common.Entities;
using Volo.Abp.DependencyInjection;

namespace Common.Interfaces
{
    /// <summary>
    /// 阈值管理
    /// </summary>
    public interface IThresholdAppService : IBaseNetAppService<Threshold>, ITransientDependency
    {

    }
}
