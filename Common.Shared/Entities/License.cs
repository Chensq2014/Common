using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Entities
{
    /// <summary>
    /// 许可证
    /// </summary>
    [Table("parakeet_Licenses", Schema = "parakeet")]
    public class License : BaseEntity
    {
        public License() { }
        public License(Guid id)
        {
            SetEntityPrimaryKey(id);
        }

        /// <summary>
        /// AppId
        /// </summary>
        [Description("AppId"), MaxLength(CommonConsts.MaxLength64)]
        public string AppId { get; set; }

        /// <summary>
        /// AppKey
        /// </summary>
        [Description("AppKey"), MaxLength(CommonConsts.MaxLength64)]
        public string AppKey { get; set; }

        /// <summary>
        /// AppSecret
        /// </summary>
        [Description("AppSecret"), MaxLength(CommonConsts.MaxLength255)]
        public string AppSecret { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        [Description("Token")]
        public string Token { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        [Description("过期时间")]
        public DateTime ExpiredAt { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称"), MaxLength(CommonConsts.MaxLength64)]
        public string Name { get; set; }

        /// <summary>
        /// 可访问资源
        /// </summary>
        public virtual ICollection<LicenseResource> LicenseResources { get; set; } = new HashSet<LicenseResource>();
    }
}
