using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Common.Dtos
{
    /// <summary>
    /// 设备扩展
    /// </summary>
    public class DeviceExtendDto : BaseDto
    {
        #region 基础字段

        /// <summary>
        /// 自定义键不重复
        /// </summary>
        [Required, MaxLength(CommonConsts.MaxLength64)]
        [Description("自定义键不重复")]
        public string Key { get; set; }

        /// <summary>
        /// 自定义值
        /// </summary>
        [Required, MaxLength(CommonConsts.MaxLength2048)]
        [Description("自定义值")]
        public string Value { get; set; }

        #endregion

        #region 设备

        /// <summary>
        /// 设备Id
        /// </summary>
        [Required]
        [Description("设备Id")]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        public virtual DeviceDto Device { get; set; }

        #endregion
    }
}
