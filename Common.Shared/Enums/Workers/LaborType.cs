using System.ComponentModel;

namespace Common.Enums
{
    /// <summary>
    /// 工种
    /// 杂工 = 0,
    /// 点工 = 10,
    /// 架子 = 20,
    /// 绿化 = 90
    /// </summary>
    public enum LaborType
    {
        /// <summary>
        /// 杂工
        /// </summary>
        [Description("杂工")]
        杂工 = 0,
        /// <summary>
        /// 点工
        /// </summary>
        [Description("点工")]
        点工 = 10,
        /// <summary>
        /// 架子
        /// </summary>
        [Description("架子")]
        架子 = 20,
        /// <summary>
        /// 绿化
        /// </summary>
        [Description("绿化")]
        绿化 = 90
    }
}
