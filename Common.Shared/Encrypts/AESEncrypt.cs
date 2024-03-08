using System;
using System.Security.Cryptography;
using System.Text;

namespace Common.Encrypts
{
    /// <summary>
    /// AESEncrypt
    /// </summary>
    public class AESEncrypt
    {
        /// <summary>
        /// AES加密 
        /// </summary>
        /// <param name="text">加密字符</param>
        /// <param name="password">加密的密码</param>
        /// <param name="iv">密钥 算法的初始化向量(IV)=接入密钥KEY（app_secret值）的前16位字符</param>
        /// <param name="keySize">加密key字节长度</param>
        /// <param name="blockSize">加密块长度</param>
        /// <returns></returns>
        public static string Encrypt(string text, string password, string iv, int keySize = 128, int blockSize = 128)
        {
            using var rijndaelCipher = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = keySize,
                BlockSize = blockSize
            };
            var pwdBytes = Encoding.UTF8.GetBytes(password);
            var keyBytes = new byte[16];
            var len = pwdBytes.Length > keyBytes.Length ? keyBytes.Length : pwdBytes.Length;
            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            iv = iv?.Length > 16 ? iv.Substring(0, 16) : iv;
            //var ivBytes = Encoding.UTF8.GetBytes(iv);
            rijndaelCipher.IV = Encoding.UTF8.GetBytes(iv);//new byte[16];
            var transform = rijndaelCipher.CreateEncryptor();
            var plainText = Encoding.UTF8.GetBytes(text);
            var cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);
            return Convert.ToBase64String(cipherBytes);
        }


        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="text">解密字符</param>
        /// <param name="password">加密的密码</param>
        /// <param name="iv">密钥:接入密钥KEY（app_secret值）的前16位字符</param>
        /// <param name="keySize">加密key字节长度</param>
        /// <param name="blockSize">加密块长度</param>
        /// <returns></returns>
        public static string Decrypt(string text, string password, string iv, int keySize = 128, int blockSize = 128)
        {
            var rijndaelCipher = new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = keySize,
                BlockSize = blockSize
            };
            var encryptedData = Convert.FromBase64String(text);
            var pwdBytes = Encoding.UTF8.GetBytes(password);
            var keyBytes = new byte[16];
            var len = pwdBytes.Length > keyBytes.Length ? keyBytes.Length : pwdBytes.Length;
            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            iv = iv.Length > 16 ? iv.Substring(0, 16) : iv;
            var ivBytes = Encoding.UTF8.GetBytes(iv);
            rijndaelCipher.IV = ivBytes;
            var transform = rijndaelCipher.CreateDecryptor();
            var plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText);
        }
    }
}
