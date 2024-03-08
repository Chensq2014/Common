namespace Common.TcpMudule.Services
{
    public class PacketStructure
    {
        /// <summary>
        /// 消息头
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// 消息体长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 总长度
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool Completed { get; set; }
    }
}
