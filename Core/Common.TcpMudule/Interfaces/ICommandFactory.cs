using Common.Interfaces;
using Common.TcpMudule.Services;
using Common.TcpMudule.Sockets;

namespace Common.TcpMudule.Interfaces
{
    /// <summary>
    /// 命令工厂接口
    /// </summary>
    public interface ICommandFactory : IHandlerType
    {
        CommandWrapper Create(AsyncUserToken userToken, byte[] data);
    }


    public interface ICommandFactory<TModule> : ICommandFactory
    {
    }
}
