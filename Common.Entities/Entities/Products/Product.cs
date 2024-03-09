﻿using Common.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Entities
{
    /// <summary>
    /// 产品
    /// </summary>
    [Table("parakeet_Products", Schema = "public")]
    [Description("产品")]
    public class Product : BaseEntity
    {
        public Product()
        {
        }

        public Product(Guid id)
        {
            base.SetEntityPrimaryKey(id);
        }

        #region 基础字段

        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        [MaxLength(CommonConsts.MaxLength255)]
        public string Name { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        [Description("单价")]
        public decimal? Price { set; get; }

        /// <summary>
        /// Amount
        /// </summary>
        [Description("数量")]
        public decimal? Amount { set; get; }

        /// <summary>
        ///  收费类型
        /// </summary>
        [Description("收费类型")]
        public ChargeType ChargeType { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(CommonConsts.MaxLength1024)]
        [Description("描述")]
        public string Description { get; set; }

        #endregion

        #region 房间

        /// <summary>
        /// 房间Id
        /// </summary>
        [Description("房间Id")]
        public Guid? HouseId { get; set; }

        /// <summary>
        /// 房间
        /// </summary>
        public virtual House House { get; set; }

        #endregion
    }
}
