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
        
        public static byte[] AbcToChar(byte[] chars)
        {
            return chars;
        }

        
        public static byte[] CharToAbc(byte[] chars)
        {
            return chars;
        }

        /// <summary>
        /// 数据加密方法
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Encrypt(string source)
        {
            return Encoding.Default.GetString(CharToAbc(Encrypt(Encoding.Default.GetBytes(string.Join("", source)), true)));
        }

        /// <summary>
        /// 数据加密方法
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string DEncrypt(string source)
        {
            return Encoding.Default.GetString(Encrypt(AbcToChar(Encoding.Default.GetBytes(source)),false));
        }

        /// <summary>
        /// 数据加密方法
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
