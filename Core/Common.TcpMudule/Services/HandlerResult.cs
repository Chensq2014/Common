using Common.Enums;

namespace Common.TcpMudule.Services
{
    public class HandlerResult
    {
        public HandlerResult()
        {
        }

        public HandlerResult(byte[] bits, HandlerStatus status)
        {
            this.Data = bits;
            this.Status = status;
        }

        public byte[] Data { get; set; }

        public HandlerStatus Status { get; set; } = HandlerStatus.Normal;

        public static HandlerResult Continue(byte[] bits)
        {
            return new HandlerResult(bits, HandlerStatus.Continue);
        }

        public static HandlerResult Normal(byte[] bits)
        {
            return new HandlerResult(bits, HandlerStatus.Normal);
        }

        public static HandlerResult Break()
        {
            return new HandlerResult(null, HandlerStatus.Break);
        }
    }
}
