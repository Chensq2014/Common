using Common.Interfaces;

namespace Common.Entities
{
    /// <summary>
    /// 处理类型名默认为全名
    /// </summary>
    public class BaseHandlerType : IHandlerType
    {
        public virtual string HandlerType => this.GetType().FullName;
    }
}
