using Common.Dtos;
using Common.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Common.Interfaces
{
    /// <summary>
    /// 安全策略服务
    /// </summary>
    public interface ISecurePolicyAppService : IBaseNetAppService<SecurePolicy>, ITransientDependency
    {
        /// <summary>
        /// 获取当前登录用户所有启用的安全策略
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<List<SecurePolicyDto>> GetCurrentUserPolicies(InputIdDto input);
    }
}
