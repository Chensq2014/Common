using System.ComponentModel;

namespace Common.Enums
{
    /// <summary>
    /// 性别,  1：男 2：女
    /// </summary>
    public enum GenderType
    {
        [Description("男")]
        男 = 1,
        [Description("女")]
        女 = 2
    }
}
