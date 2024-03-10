﻿namespace Common.Dtos
{
    /// <summary>
    /// 获取供应商分页
    /// </summary>
    public class GetSuppliersInputDto : PagedInputDto
    {
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string Name { get; set; }
    }
}
