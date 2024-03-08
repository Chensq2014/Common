using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Common.Extensions;
using MiniExcelLibs.Attributes;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Uow;

namespace Common.Entities
{
    /// <summary>
    /// 自定义EntityBase 便于对基类字段设置属性
    /// </summary>
    public abstract class EntityBase : EntityBase<Guid>
    {

    }
    public abstract class EntityBase<TKey> :
        IFullAuditedObject,
        IHasEntityVersion,
        IHasExtraProperties,
        IHasConcurrencyStamp,
        IAggregateRoot<TKey>,
        IGeneratesDomainEvents
    {
        #region 属性字段

        /// <summary>
        /// 主键Id
        /// </summary>
        [ExcelIgnore]
        [Description("主键Id")]
        public virtual TKey Id { get; protected set; }

        /// <summary>
        /// 并发戳 可能是个guid，加密字符串等 表示唯一
        /// </summary>
        [DisableAuditing]
        [ExcelIgnore]
        [Description("并发戳")]//[MaxLength(CommonConsts.MaxLength2048)]
        public virtual string ConcurrencyStamp { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ExcelIgnore]
        [Description("创建时间")]
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [ExcelIgnore]
        [Description("创建人")]
        public virtual Guid? CreatorId { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [ExcelIgnore]
        [Description("更新时间")]
        public virtual DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [Description("更新人")]
        public virtual Guid? LastModifierId { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [ExcelIgnore]
        [Description("是否删除")]
        public virtual bool IsDeleted { get; set; }

        /// <summary>
        /// 删除人
        /// </summary>
        [ExcelIgnore]
        [Description("删除人")]
        public virtual Guid? DeleterId { get; set; }

        /// <summary>
        /// 删除时间
        /// </summary>
        [ExcelIgnore]
        [Description("删除时间")]
        public virtual DateTime? DeletionTime { get; set; }

        /// <summary>
        /// 实体版本号
        /// </summary>
        [ExcelIgnore]
        [Description("实体版本号")]
        public virtual int EntityVersion { get; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        [ExcelIgnore]
        [Description("扩展属性")]
        public virtual ExtraPropertyDictionary ExtraProperties { get; protected set; }


        private readonly ICollection<DomainEventRecord> _distributedEvents = new Collection<DomainEventRecord>();
        private readonly ICollection<DomainEventRecord> _localEvents = new Collection<DomainEventRecord>();

        #endregion


        protected EntityBase()
        {
            ConcurrencyStamp = GuidExtensions.NewIdString();
            ExtraProperties = new ExtraPropertyDictionary();
            this.SetDefaultsForExtraProperties();
        }

        protected EntityBase(TKey id)
        {
            Id = id;
            ConcurrencyStamp = GuidExtensions.NewIdString();
            ExtraProperties = new ExtraPropertyDictionary();
            this.SetDefaultsForExtraProperties();
            EntityHelper.TrySetTenantId(this);
        }

        ///// <summary>
        ///// 提供一个公共方法设置主键 protected
        ///// </summary>
        ///// <param name="id"></param>
        //public virtual void SetEntityPrimaryKey(TKey id)
        //{
        //    Id = id;
        //}

        public bool EntityEquals(IEntity other) => EntityHelper.EntityEquals(this, other);

        public virtual object?[] GetKeys() => new object[1]
        {
             Id
        };

        /// <summary>
        /// ToString方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 2);
            //interpolatedStringHandler.AppendLiteral("[ENTITY: ");
            //interpolatedStringHandler.AppendFormatted(this.GetType().Name);
            //interpolatedStringHandler.AppendLiteral("] Id = ");
            //interpolatedStringHandler.AppendFormatted<TKey>(this.Id);
            //return interpolatedStringHandler.ToStringAndClear();
            return $"[ENTITY: {GetType().Name}] Keys = {GetKeys().JoinAsString(", ")}";
        }
        public virtual IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            return ExtensibleObjectValidator.GetValidationErrors(this, validationContext);
        }

        public virtual IEnumerable<DomainEventRecord> GetLocalEvents() => _localEvents;

        public virtual IEnumerable<DomainEventRecord> GetDistributedEvents() => _distributedEvents;

        public virtual void ClearLocalEvents() => _localEvents.Clear();

        public virtual void ClearDistributedEvents() => _distributedEvents.Clear();

        protected virtual void AddLocalEvent(object eventData) => _localEvents.Add(new DomainEventRecord(eventData, EventOrderGenerator.GetNext()));

        protected virtual void AddDistributedEvent(object eventData) => _distributedEvents.Add(new DomainEventRecord(eventData, EventOrderGenerator.GetNext()));

    }

}
