using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Common.Encrypts
{
    /// <summary>
    /// RSA ECC core里暂时无效
    /// 可逆非对称加密 密文+加密key------解密key+密文  加密key与解密key成对出现，只知道加密key或解密key都推导不出彼此
    /// 非对称加密算法的优点是密钥管理很方便，缺点是速度慢。
    /// 公开加密key：保证数据安全传递（有解密key的人才能解开我使用这个加密key加密的密文）
    /// 公开解密key：保证数据的不可抵赖（解密key解密的密文一定来自于某个加密key加密的密文）有加密key的这个人不可抵赖
    /// 
    /// </summary>
    public class RsaEncrypt
    {
        /// <summary>
        /// RSA最大加密明文大小
        /// </summary>
        private const int MAX_ENCRYPT_BLOCK = 117;

        /// <summary>
        /// RSA最大解密密文大小
        /// </summary>
        private const int MAX_DECRYPT_BLOCK = 128;

        /// <summary>
        /// 获取加密/解密对
        /// 给你一个，是无法推算出另外一个的
        /// 
        /// Encrypt   Decrypt
        /// </summary>
        /// <returns>Encrypt   Decrypt</returns>
        public static KeyValuePair<string, string> GetKeyPair()
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            string publicKey = RSA.ToXmlString(false);//core里不支持
            string privateKey = RSA.ToXmlString(true);
            return new KeyValuePair<string, string>(publicKey, privateKey);
        }


        /// <summary>
        /// 公匙加密 加密rsa
        /// </summary>
        /// <param name="str">加密字符串(json)</param>
        /// <param name="publicKey">base64格式公钥</param>
        /// <returns>返回加密字符串(base64格式)</returns>
        public static string EncryptByPublicKey(string str, string publicKey)
        {
            //The public key will import successfully using ImportSubjectPublicKeyInfo, which is also new in .NET Core 3.
            using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
            //rsa.ImportRSAPublicKey(Encoding.UTF8.GetBytes(publicKey), out int bytesRead); //公钥
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey), out _);//公钥
            //rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(publicKey), out _);//公钥
            var data = Encoding.UTF8.GetBytes(str);//str.GetBytes()

            //var sample = rsa.Encrypt(data, false);//长度不符合要求，需要进行分段加密，根据对方提供的java分段加密方法进行
            //var pass = Convert.ToBase64String(sample);
            //return pass;

            // 对数据分段加密
            var memoryStream = new MemoryStream();
            int inputLen = data.Length;
            var offSet = 0;
            while (inputLen - offSet > 0)
            {
                var byteArrayLength = inputLen - offSet > MAX_ENCRYPT_BLOCK ? MAX_ENCRYPT_BLOCK : inputLen - offSet;
                var byteArray = new byte[byteArrayLength];
                Array.Copy(data, offSet, byteArray, 0, byteArrayLength);
                var cache = rsa.Encrypt(byteArray, false);
                memoryStream.Write(cache, 0, cache.Length);
                offSet += byteArrayLength;
            }
            var encryptedData = memoryStream.ToArray();
            memoryStream.Close();
            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// 加密：内容+加密key
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encryptKey">加密key</param>
        /// <returns></returns>
        public static string Encrypt(string content, string encryptKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(encryptKey);
            UnicodeEncoding ByteConverter = new UnicodeEncoding();
            byte[] dataToEncrypt = ByteConverter.GetBytes(content);
            byte[] resultBytes = rsa.Encrypt(dataToEncrypt, false);
            return Convert.ToBase64String(resultBytes);
        }

        /// <summary>
        /// 解密  内容+解密key
        /// </summary>
        /// <param name="content"></param>
        /// <param name="decryptKey">解密key</param>
        /// <returns></returns>
        public static string Decrypt(string content, string decryptKey)
        {
            byte[] dataToDecrypt = Convert.FromBase64String(content);
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(decryptKey);
            byte[] resultBytes = RSA.Decrypt(dataToDecrypt, false);
            UnicodeEncoding ByteConverter = new UnicodeEncoding();
            return ByteConverter.GetString(resultBytes);
        }


        /// <summary>
        /// 可以合并在一起的，，每次产生一组新的密钥
        /// </summary>
        /// <param name="content"></param>
        /// <param name="publicKey">加密key</param>
        /// <param name="privateKey">解密key</param>
        /// <returns>加密后结果</returns>
        private static string Encrypt(string content, out string publicKey, out string privateKey)
        {
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
            publicKey = rsaProvider.ToXmlString(false);
            privateKey = rsaProvider.ToXmlString(true);

            UnicodeEncoding byteConverter = new UnicodeEncoding();
            byte[] dataToEncrypt = byteConverter.GetBytes(content);
            byte[] resultBytes = rsaProvider.Encrypt(dataToEncrypt, false);
            return Convert.ToBase64String(resultBytes);
        }
    }
}
