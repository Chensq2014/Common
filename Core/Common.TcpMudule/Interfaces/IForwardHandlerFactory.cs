using Common.Interfaces;

namespace Common.TcpMudule.Interfaces
{
    public interface IForwardHandlerFactory : IHandlerType
    {
        IForwardHandler CreateHandler(string handlerType);
    }
}
