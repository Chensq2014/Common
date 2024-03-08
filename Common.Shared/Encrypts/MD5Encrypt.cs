using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Common.Extensions;

namespace Common.Encrypts
{
    /// <summary>
    /// 不可逆加密
    /// 1 防止被篡改
    /// 2 防止明文存储
    /// 3 防止抵赖，数字签名
    /// </summary>
    public class MD5Encrypt
    {
        #region MD5

        /// <summary>
        /// Md5
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public string Md5(string conn)
        {
            return conn.ToMd5();
        }

        /// <summary>
        /// 构造签名字符串
        /// </summary>
        /// <param name="forSignString">请求要md5的符串</param>
        /// <returns></returns>
        public static string BuildSign(string forSignString)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(forSignString));
            string sign = ConvertBinaryToHexValueString(bytes);
            return sign.ToLower();
        }

        /// <summary>
        /// 转换二进制为16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string ConvertBinaryToHexValueString(IEnumerable<byte> bytes)
        {
            var sb = new StringBuilder();
            foreach (byte _byte in bytes)
            {
                #region 待移除

                //var hex = _byte.ToString("x");//小写16进制
                //if (hex.Length == 1)
                //{
                //    sb.Append("0");
                //}
                //sb.Append(hex);

                #endregion

                //sb.Append($"{_byte:x}");//小写16进制
                //sb.Append($"{_byte:x2}");//小写两位16进制
                sb.Append($"{_byte:X2}");//大写两位16进制
            }

            return sb.ToString();
        }

        /// <summary>
        /// MD5加密,和动网上的16/32位MD5加密结果相同,
        /// 使用的UTF8编码
        /// </summary>
        /// <param name="source">待加密字串</param>
        /// <param name="length">16或32值之一,其它则采用.net默认MD5加密算法</param>
        /// <returns>加密后的字串</returns>
        public static string Encrypt(string source, int length = 32)//默认参数
        {
            if (!source.HasValue())
            {
                return string.Empty;
            }
            var sb = new StringBuilder();
            using (var provider = MD5.Create("MD5"))
            {
                //HashAlgorithm provider = CryptoConfig.CreateFromName("MD5") as HashAlgorithm;
                byte[] bytes = Encoding.UTF8.GetBytes(source);//这里需要区别编码的
                byte[] hashValue = provider.ComputeHash(bytes);
                switch (length)
                {
                    case 16://16位密文是32位密文的9到24位字符
                        for (var i = 4; i < 12; i++)
                        {
                            // X为 十六进制 2位 每次都是两位数
                            // 假设有两个数10和26，
                            // 正常情况十六进制显示0xA、0x1A，
                            // 这样看起来不整齐，为了好看，可以指定"X2"，这样显示出来就是：0x0A、0x1A
                            sb.Append(hashValue[i].ToString("x2"));
                        }
                        break;
                    case 32:
                        for (var j = 0; j < 16; j++)
                        {
                            sb.Append(hashValue[j].ToString("x2"));
                        }
                        break;
                    default:
                        foreach (var t in hashValue)
                        {
                            sb.Append(t.ToString("x2"));
                        }
                        break;
                }
            }
            return sb.ToString();
        }
        #endregion MD5

        #region MD5摘要 只要文件不发生变化，摘要就一致 (但可以更文件名)
        /// <summary>
        /// 获取文件的MD5摘要
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string AbstractFile(string fileName)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Open))
            {
                return AbstractFile(file);
            }
        }

        /// <summary>
        /// 根据stream获取文件摘要
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string AbstractFile(Stream stream)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(stream);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        #endregion

        ///// <summary>
        ///// 加密字符串
        ///// </summary>
        ///// <param name="conn"></param>
        ///// <returns></returns>
        //public string AbpEncrypt(string conn)
        //{
        //    return SimpleStringCipher.Instance.Encrypt(conn);
        //}

        ///// <summary>
        ///// 解密字符串
        ///// </summary>
        ///// <param name="conn"></param>
        ///// <returns></returns>
        //public string AbpDecrypt(string conn)
        //{
        //    return SimpleStringCipher;
        //}
    }
}
