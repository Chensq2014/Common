using System;

namespace Common.TcpMudule.Sockets
{
    public class SocketEventArgs : EventArgs
    {
        public SocketEventArgs()
        {
        }

        public SocketEventArgs(byte[] buffer)
        {
            this.Buffer = buffer;
        }

        public byte[] Buffer { get; set; }
    }
}
