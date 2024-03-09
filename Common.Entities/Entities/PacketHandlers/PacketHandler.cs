﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Entities
{
    /// <summary>
    /// Tcp包头 不用存表，放在配置文件或者缓存里或者tcp模块写死更合适
    /// </summary>
    [Description("Tcp包头")]
    [Table("parakeet_PacketHandlers", Schema = "parakeet")]
    public class PacketHandler : BaseEntity
    {
        public PacketHandler()
        {
        }

        public PacketHandler(Guid id)
        {
            base.SetEntityPrimaryKey(id);
        }

        /// <summary>
        /// 包头
        /// </summary>
        [Required, Description("包头"), MaxLength(CommonConsts.MaxLength255)]
        public string Header { get; set; }

        /// <summary>
        /// 处理器
        /// </summary>
        [Required, Description("处理器"), MaxLength(CommonConsts.MaxLength255)]
        public string Handler { get; set; }

        public override string ToString()
        {
            return $"{Header}:{Handler}";
        }
    }
}
