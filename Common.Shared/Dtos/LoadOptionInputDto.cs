using DevExtreme.AspNet.Data;

namespace Common.Dtos
{
    /// <summary>
    /// DevExtreme loadOptions扩展
    /// </summary>
    public class LoadOptionInputDto : BaseDto
    {
        /// <summary>
        /// DevExtreme  LoadOptions 基类
        /// </summary>
        public DataSourceLoadOptionsBase LoadOptions { get; set; }
    }
}
