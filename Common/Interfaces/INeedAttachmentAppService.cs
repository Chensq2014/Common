using Common.Dtos;
using Common.Entities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Common.Interfaces
{
    /// <summary>
    /// 需求附件
    /// </summary>
    [Description("需求附件")]
    public interface INeedAttachmentAppService : IBaseNetAppService<NeedAttachment>, ITransientDependency
    {
        /// <summary>
        /// 需求附件
        /// </summary>
        /// <param name="input">id:needId需求Id</param>
        /// <returns></returns>
        Task<IList<NeedAttachmentDto>> GetNeedAttachments(InputIdDto input);
    }
}
