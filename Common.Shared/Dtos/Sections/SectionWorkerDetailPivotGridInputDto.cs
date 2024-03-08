using Common.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Common.Dtos
{
    /// <summary>
    /// PivotGridInputDto
    /// </summary>
    public class SectionWorkerDetailPivotGridInputDto: InputDateTimeDto
    {
        /// <summary>
        /// 工作位置名称
        /// </summary>
        [MaxLength(CommonConsts.MaxLength255)]
        [Description("工作位置名称")]
        public string PositionName { get; set; }

        /// <summary>
        /// 区域工人
        /// </summary>
        [Description("区域工人")]
        public Guid? SectionWorkerId { get; set; }

        /// <summary>
        /// 工种Id
        /// </summary>
        [Description("工种Id")]
        public Guid? WorkerTypeId { get; set; }

        /// <summary>
        /// 劳务人员Id
        /// </summary>
        [Description("劳务人员Id")]
        public Guid? WorkerId { get; set; }

        /// <summary>
        /// 劳务类型
        /// </summary>
        public LaborType? LaborType { get; set; }
    }
}
