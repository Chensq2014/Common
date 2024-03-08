using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Nest;
using Common.Extensions;

namespace Common.Dtos
{
    /// <summary>
    /// 地块(区域)工人 用工明细
    /// </summary>
    [Description("地块(区域)工人 用工明细")]
    public class SectionWorkerDetailListDto : SectionWorkerDetailDto
    {

        /// <summary>
        /// 项目名
        /// </summary>
        public string ProjectName => SectionWorker?.Section?.Project?.Name;
        
        /// <summary>
        /// 地块名
        /// </summary>
        public string SectionName => SectionWorker?.Section?.Name;

        /// <summary>
        /// 地块工人名称
        /// </summary>
        public string SectionWorkerName => SectionWorker?.Name;

        /// <summary>
        /// 工种名称
        /// </summary>
        public string WorkerTypeName => SectionWorker?.WorkType?.Name;

        /// <summary>
        /// 工种名称
        /// </summary>
        public string LaborTypeName => SectionWorker?.LaborType.DisplayName();

        /// <summary>
        /// 工人
        /// </summary>
        public string WorkerName => SectionWorker?.Worker?.Name;
        
    }
}
