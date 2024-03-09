﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Entities
{
    /// <summary>
    /// 房间
    /// </summary>
    [Table("parakeet_Houses", Schema = "public")]
    [Description("房间")]
    public class House : BaseEntity
    {
        public House()
        {
        }

        public House(Guid id)
        {
            base.SetEntityPrimaryKey(id);
        }

        #region 基础字段

        /// <summary>
        /// 房间号
        /// </summary>
        [Description("房间号")]
        [MaxLength(CommonConsts.MaxLength255)]
        public string Number { get; set; }

        /// <summary>
        /// 建筑面积
        /// </summary>
        [Description("建筑面积")]
        public decimal? BuildingArea { set; get; }

        /// <summary>
        /// 使用面积
        /// </summary>
        [Description("使用面积")]
        public decimal? UseArea { set; get; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(CommonConsts.MaxLength1024)]
        [Description("描述")]
        public string Description { get; set; }


        #endregion

        #region 小区

        [Description("小区Id")]
        public Guid? SectionId { get; set; }

        /// <summary>
        /// 小区
        /// </summary>
        public virtual Section Section { get; set; }

        #endregion

        #region 房间使用装修产品

        /// <summary>
        /// 房间使用装修产品
        /// </summary>
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();

        #endregion

    }
}
