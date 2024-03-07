using System;
using System.ComponentModel;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;

namespace Common.Entities
{
    /// <summary>
    /// 实体类基类 确保只含有基础字段
    /// </summary>
    [Serializable, Description("实体类基类")]
    public abstract class BaseEntity : BaseEntity<Guid>
    {
        //protected BaseEntity(Guid id) : base(id)
        //{
        //}
    }

    /// <summary>
    /// 泛型实体基类
    /// </summary>
    /// <typeparam name="TPrimaryKey">实体主键类型</typeparam>
    [Serializable, Description("实体类泛型基类")]
    public abstract class BaseEntity<TPrimaryKey> : FullAuditedAggregateRoot<TPrimaryKey>, IHasEntityVersion
    {
        ///// <summary>
        ///// 默认构造函数
        ///// </summary>
        //protected BaseEntity(TPrimaryKey id)
        //{
        //    //base.Id = id; //default(TPrimaryKey);
        //    //CreationTime = DateTime.Now;
        //    //LastModificationTime = CreationTime;
        //    SetEntityPrimaryKey(id);
        //}

        /// <summary>
        /// 提供一个公共方法设置主键 protected
        /// </summary>
        /// <param name="id"></param>
        public virtual void SetEntityPrimaryKey(TPrimaryKey id)
        {
            base.Id = id;
        }

        /// <summary>
        /// 实体版本号
        /// </summary>
        public int EntityVersion { get; }
    }
}
