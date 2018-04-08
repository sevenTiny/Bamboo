/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-04-08 21:54:07
 * Modify: 2018-04-08 21:54:07
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using System.Security.Cryptography;
using System.Text;

namespace SevenTiny.Bantina.Security
{
    public class RSAHelper
    {
        private RSAHelper() { }

        public static string Encrypt(string source, string publicKey)
        {
            return RSACommon.Encrypt(source, publicKey);
        }
        public static string Decrypt(string source, string privateKey)
        {
            return RSACommon.Decrypt(source, privateKey);
        }

        #region Sign

        public static string Sign(string data, string privateKey, Encoding encoding, HashAlgorithmName hashAlgorithmName)
        {
            return RSACommon.Sign(data, privateKey, encoding, hashAlgorithmName);
        }

        /// <summary>
        /// Sign with UTF-8
        /// </summary>
        /// <param name="data"></param>
        /// <param name="privateKey"></param>
        /// <param name="hashAlgorithmName"></param>
        /// <returns></returns>
        public static string Sign(string data, string privateKey, HashAlgorithmName hashAlgorithmName)
        {
            return RSACommon.Sign(data, privateKey, Encoding.UTF8, hashAlgorithmName);
        }

        public static string SignWithSHA1(string data, string privateKey)
        {
            return RSACommon.Sign(data, privateKey, Encoding.UTF8, HashAlgorithmName.SHA1);
        }

        public static string SignWithSHA1(string data, string privateKey, Encoding encoding)
        {
            return RSACommon.Sign(data, privateKey, encoding, HashAlgorithmName.SHA1);
        }

        public static string SignWithSHA256(string data, string privateKey)
        {
            return RSACommon.Sign(data, privateKey, Encoding.UTF8, HashAlgorithmName.SHA256);
        }

        public static string SignWithSHA256(string data, string privateKey, Encoding encoding)
        {
            return RSACommon.Sign(data, privateKey, encoding, HashAlgorithmName.SHA256);
        }

        #endregion

        #region Verify

        public static bool Verify(string data, string sign, string publicKey, Encoding encoding, HashAlgorithmName hashAlgorithmName)
        {
            return RSACommon.Verify(data, sign, publicKey, encoding, hashAlgorithmName);
        }

        /// <summary>
        /// Verify with UTF-8
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sign"></param>
        /// <param name="publicKey"></param>
        /// <param name="hashAlgorithmName"></param>
        /// <returns></returns>
        public static bool Verify(string data, string sign, string publicKey, HashAlgorithmName hashAlgorithmName)
        {
            return RSACommon.Verify(data, sign, publicKey, Encoding.UTF8, hashAlgorithmName);
        }
        /// <summary>
        /// Verify with UTF-8
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sign"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static bool VerifyWithSHA1(string data, string sign, string publicKey)
        {
            return RSACommon.Verify(data, sign, publicKey, Encoding.UTF8, HashAlgorithmName.SHA1);
        }
        public static bool VerifyWithSHA1(string data, string sign, string publicKey, Encoding encoding)
        {
            return RSACommon.Verify(data, sign, publicKey, encoding, HashAlgorithmName.SHA1);
        }
        /// <summary>
        /// Verify with UTF-8
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sign"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static bool VerifyWithSHA256(string data, string sign, string publicKey)
        {
            return RSACommon.Verify(data, sign, publicKey, Encoding.UTF8, HashAlgorithmName.SHA256);
        }
        public static bool VerifyWithSHA256(string data, string sign, string publicKey, Encoding encoding)
        {
            return RSACommon.Verify(data, sign, publicKey, encoding, HashAlgorithmName.SHA256);
        }
        
        #endregion
    }
}
