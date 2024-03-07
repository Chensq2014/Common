using System;
using System.ComponentModel;

namespace Common.CustomAttributes
{
    /// <summary>
    /// 是否支持导出属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field), Description("是否支持导出属性")]
    public class ExportAttribute : Attribute
    {
    }
}
