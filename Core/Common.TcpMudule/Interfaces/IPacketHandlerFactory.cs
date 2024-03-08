using Common.Interfaces;

namespace Common.TcpMudule.Interfaces
{
    public interface IPacketHandlerFactory : IHandlerType
    {
        IPacketHandler Create(byte[] data);
    }
}
