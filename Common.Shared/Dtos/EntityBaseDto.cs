using MiniExcelLibs.Attributes;
using System;
using System.ComponentModel;
using Volo.Abp.Application.Dtos;
using Volo.Abp.MultiTenancy;

namespace Common.Dtos
{
    /// <summary>
    /// Dto 抽象基类设计
    /// </summary>
    public abstract class EntityBaseDto : EntityBaseDto<Guid>
    {
    }
    public abstract class TenantEntityBaseDto : TenantEntityBaseDto<Guid>
    {

    }
    public abstract class TenantEntityBaseDto<TKey> : EntityBaseDto<TKey>, IMultiTenant
    {
        /// <summary>
        /// 租户Id
        /// </summary>
        [ExcelIgnore]
        [Description("租户Id")]
        public virtual Guid? TenantId { get; set; }
    }
    public abstract class EntityBaseDto<TKey> : IEntityDto<TKey>
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [ExcelIgnore]
        [Description("主键Id")]
        public TKey Id { get; set; }

        /// <summary>
        /// 扩展Tostring()
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"[DTO: {GetType().Name}] Id = {Id}";
        
        #region Dto基本字段

        ///// <summary>
        ///// 并发戳 可能是个guid，加密字符串等 表示唯一
        ///// </summary>
        //[DisableAuditing]
        //[ExcelIgnore]
        //[Description("并发戳")]//[MaxLength(CommonConsts.MaxLength2048)]
        //public virtual string ConcurrencyStamp { get; set; }

        ///// <summary>
        ///// 创建时间
        ///// </summary>
        //[ExcelIgnore]
        //[Description("创建时间")]
        //public virtual DateTime CreationTime { get; set; }

        ///// <summary>
        ///// 创建人
        ///// </summary>
        //[ExcelIgnore]
        //[Description("创建人")]
        //public virtual Guid? CreatorId { get; set; }

        ///// <summary>
        ///// 更新时间
        ///// </summary>
        //[ExcelIgnore]
        //[Description("更新时间")]
        //public virtual DateTime? LastModificationTime { get; set; }

        ///// <summary>
        ///// 更新人
        ///// </summary>
        //[Description("更新人")]
        //public virtual Guid? LastModifierId { get; set; }

        ///// <summary>
        ///// 是否删除
        ///// </summary>
        //[ExcelIgnore]
        //[Description("是否删除")]
        //public virtual bool IsDeleted { get; set; }

        ///// <summary>
        ///// 删除人
        ///// </summary>
        //[ExcelIgnore]
        //[Description("删除人")]
        //public virtual Guid? DeleterId { get; set; }

        ///// <summary>
        ///// 删除时间
        ///// </summary>
        //[ExcelIgnore]
        //[Description("删除时间")]
        //public virtual DateTime? DeletionTime { get; set; }

        ///// <summary>
        ///// 实体版本号
        ///// </summary>
        //[ExcelIgnore]
        //[Description("实体版本号")]
        //public virtual int EntityVersion { get; }

        ///// <summary>
        ///// 扩展属性
        ///// </summary>
        //[ExcelIgnore]
        //[Description("扩展属性")]
        //public virtual ExtraPropertyDictionary ExtraProperties { get; protected set; }

        #endregion
    }

}
