using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Helpers
{
    /// <summary>
    /// 自定义基于Encoding的字符串数据加密
    /// </summary>
    public class EncodingEncryptHelper
    {
        /// <summary>
        /// 正处理 可逆
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] AbcToChar(byte[] source)
        {
            List<byte> list = new List<byte>();
            for (int i = 0; i < (source.Length - 1); i += 2)
            {
                int num2 = source[i] - 0x61;
                int num3 = source[i + 1] - 0x61;
                list.Add((byte)((num2 * 0x1a) + num3));
            }
            return list.ToArray();
        }
        
        /// <summary>
        /// 反处理 可逆
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] CharToAbc(byte[] source)
        {
            List<byte> list = new List<byte>();
            //char[] chArray2 = new char[] {
            //    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
            //    'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
            //};
            for (int i = 0; i < source.Length; i++)
            {
                //if (i == 0x1d)
                //{
                //    i = 0x1d;
                //}
                int num2 = source[i];
                list.Add((byte)((num2 / 0x1a) + 0x61));
                list.Add((byte)((num2 % 0x1a) + 0x61));
            }
            return list.ToArray();
        }

        /// <summary>
        /// 数据加密方法
        /// 加密：字符串A---->GetBytes得到二进制数组B -->B加密二进制C->C被处理二进制D-->GetString(D)-->加密字符串E
        /// 解密：加密字符串E--->GetBytes(E)得到二进制D-->D反处理二进制C--->C反加密二进制B--->GetString(二进制数组B)-->字符串A
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Encrypt(string source)
        {
            return Encoding.UTF8.GetString(CharToAbc(Encrypt(Encoding.UTF8.GetBytes(source), true)));
        }

        /// <summary>
        /// 数据解密方法
        /// 加密：字符串A---->GetBytes得到二进制数组B -->B加密二进制C->C被处理二进制D-->GetString(D)-->加密字符串E
        /// 解密：加密字符串E--->GetBytes(E)得到二进制D-->D反处理二进制C--->C反加密二进制B--->GetString(二进制数组B)-->字符串A
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string DEncrypt(string source)
        {
            return Encoding.UTF8.GetString(Encrypt(AbcToChar(Encoding.UTF8.GetBytes(source)),false));
        }

        /// <summary>
        /// 数据加密方法 根据flag 可逆处理数据源与结果
        /// </summary>
        /// <param name="source"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] source,bool flag)
        {
            int num = 0;
            List<byte> list = new List<byte>();
            char[] chArray = new char[] {
                '\x001f', '\x0017', '\x0003', '\x0006', '\x0019', '\x0016', '\x001a', '\t', '\x0019', '\x001c', '\x001d', '\x0018', '\t', '\a', '\v', '\f',
                '\x001a', '\x0015', '\x001b', '\x000f', '\x0011', '\x001d', '\x0016', '\x0013', '\x0015', '\v', '\x0016'
            };
            for (int i = 0; i < source.Length; i++)
            {
                int num2;
                if (i < chArray.Count<char>())
                {
                    num2 = i;
                }
                else
                {
                    num2 = ((i + 1) % ++num) - 1;
                    if (num2 == -1)
                    {
                        num2 = 0;
                    }
                }
                if (flag)
                {
                    list.Add((byte)(source[i] + chArray[num2]));
                }
                else
                {
                    list.Add((byte)(source[i] - chArray[num2]));
                }
            }
            return list.ToArray();
        }


    }
}
