using Common.Enums;
using Common.TcpMudule.Interfaces;

namespace Common.TcpMudule.Services
{
    /// <summary>
    /// 命令包装类
    /// </summary>
    public class CommandWrapper
    {
        /// <summary>
        /// 命令
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// 处理状态
        /// </summary>
        public HandlerStatus Status { get; set; }

        /// <summary>
        /// 正常
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandWrapper Normal(ICommand command)
        {
            return new CommandWrapper
            {
                Command = command,
                Status = HandlerStatus.Normal
            };
        }

        /// <summary>
        /// 继续
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommandWrapper Continue(ICommand command)
        {
            return new CommandWrapper
            {
                Command = command,
                Status = HandlerStatus.Continue
            };
        }

        /// <summary>
        /// 中断
        /// </summary>
        /// <returns></returns>
        public static CommandWrapper Break()
        {
            return new CommandWrapper
            {
                Command = null,
                Status = HandlerStatus.Break
            };
        }
    }
}
