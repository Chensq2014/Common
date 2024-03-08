using System;
using System.Text;

namespace Common.TcpMudule.Sockets
{
    /// <summary>
    /// 动态缓存管理
    /// </summary>
    public class DynamicBufferManager
    {
        /// <summary>
        /// 存放内存的数组
        /// </summary>
        public byte[] Buffer { get; set; }

        /// <summary>
        /// 写入数据大小
        /// </summary>
        public int Size { get; set; }

        private int _bufferSize;

        /// <summary>
        /// 去掉空byte
        /// </summary>
        public byte[] CleanBuffer
        {
            get
            {
                var clearBuffer = new byte[Size];
                Array.Copy(Buffer, 0, clearBuffer, 0, Size);
                return clearBuffer;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bufferSize"></param>
        public DynamicBufferManager(int bufferSize)
        {
            this._bufferSize = bufferSize;
            Size = 0;
            Buffer = new byte[bufferSize];
        }


        /// <summary>
        /// 获得剩余的字节数
        /// </summary>
        /// <returns></returns>
        public int RemainingSize => Buffer.Length - Size;

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void Clear()
        {
            Size = 0;
            Buffer = new byte[_bufferSize];
        }

        /// <summary>
        /// 清理指定大小的数据
        /// </summary>
        /// <param name="count"></param>
        public void Clear(int count)
        {
            if (count >= Size)//如果需要清理的数据大于现有数据大小，则全部清理
            {
                Size = 0;
            }
            else
            {
                for (int i = 0; i < Size - count; i++)//否则后面的数据往前移
                {
                    Buffer[i] = Buffer[count + i];
                }
                Size = Size - count;
            }
        }

        /// <summary>
        /// 设置缓存大小
        /// </summary>
        /// <param name="size"></param>
        public void SetBufferSize(int size)
        {
            if (Buffer.Length < size)
            {
                byte[] tmpBuffer = new byte[size];
                Array.Copy(Buffer, 0, tmpBuffer, 0, Size); //复制以前的数据
                Buffer = tmpBuffer; //替换
            }
        }


        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void WriteBuffer(byte[] buffer, int offset, int count)
        {
            if (RemainingSize >= count) //缓冲区空间够，不需要申请
            {
                Array.Copy(buffer, offset, Buffer, Size, count);
                Size += count;
            }
            else //缓冲区空间不够，需要申请更大的内存，并进行移位
            {
                int totalSize = Buffer.Length + count - RemainingSize; //总大小-空余大小
                byte[] tmpBuffer = new byte[totalSize];
                Array.Copy(Buffer, 0, tmpBuffer, 0, Size); //复制以前的数据
                Array.Copy(buffer, offset, tmpBuffer, Size, count); //复制新写入的数据
                Size = Size + count;
                Buffer = tmpBuffer; //替换
            }
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="buffer"></param>
        public void WriteBuffer(byte[] buffer)
        {
            WriteBuffer(buffer, 0, buffer.Length);
        }

        public void WriteShort(short value, bool convert)
        {
            if (convert)
            {
                value = System.Net.IPAddress.HostToNetworkOrder(value); //NET是小头结构，网络字节是大头结构，需要客户端和服务器约定好
            }
            byte[] tmpBuffer = BitConverter.GetBytes(value);
            WriteBuffer(tmpBuffer);
        }

        public void WriteInt(int value, bool convert)
        {
            if (convert)
            {
                value = System.Net.IPAddress.HostToNetworkOrder(value); //NET是小头结构，网络字节是大头结构，需要客户端和服务器约定好
            }
            byte[] tmpBuffer = BitConverter.GetBytes(value);
            WriteBuffer(tmpBuffer);
        }

        public void WriteLong(long value, bool convert)
        {
            if (convert)
            {
                value = System.Net.IPAddress.HostToNetworkOrder(value); //NET是小头结构，网络字节是大头结构，需要客户端和服务器约定好
            }
            byte[] tmpBuffer = BitConverter.GetBytes(value);
            WriteBuffer(tmpBuffer);
        }

        public void WriteString(string value)
        {
            byte[] tmpBuffer = Encoding.UTF8.GetBytes(value);
            WriteBuffer(tmpBuffer);
        }
    }
}
