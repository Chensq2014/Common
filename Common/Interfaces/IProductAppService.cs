using Common.Entities;
using Volo.Abp.DependencyInjection;

namespace Common.Interfaces
{
    /// <summary>
    /// 产品服务
    /// </summary>
    public interface IProductAppService : IBaseNetAppService<Product>, ITransientDependency
    {
        
    }
}