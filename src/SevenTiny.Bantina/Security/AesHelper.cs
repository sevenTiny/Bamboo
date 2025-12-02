using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SevenTiny.Bantina.Security
{
    /// <summary>
    /// AES 加密
    /// CBC 模式加密较快
    /// GCM 更加安全
    /// </summary>
    public static class AesHelper
    {
        /// <summary>
        /// generate new key
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateSecretKey(int length = 32)
        {
            return Convert.ToBase64String(GenerateRandomBytes(length));
        }

        /// <summary>
        /// Aes Encrypt (CBC mode)
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string CbcEncrypt(string plainText, string key)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            using var aesAlg = Aes.Create();
            aesAlg.Key = Convert.FromBase64String(key);
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            // Generate random IV (16 bytes for AES-CBC)
            aesAlg.GenerateIV();
            byte[] iv = aesAlg.IV;

            // Encrypt
            using var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, iv);
            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            byte[] cipherBytes = msEncrypt.ToArray();

            // Combine IV + CipherText
            byte[] combinedData = new byte[iv.Length + cipherBytes.Length];
            Buffer.BlockCopy(iv, 0, combinedData, 0, iv.Length);
            Buffer.BlockCopy(cipherBytes, 0, combinedData, iv.Length, cipherBytes.Length);

            // Return as Base64
            return Convert.ToBase64String(combinedData);
        }

        /// <summary>
        /// Aes Decrypt (CBC mode)
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static string CbcDecrypt(string cipherText, string key)
        {
            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;

            // Decode the combined data (IV + CipherText)
            byte[] combinedData = Convert.FromBase64String(cipherText);

            // Extract IV (first 16 bytes for AES-CBC)
            int ivLength = 16; // AES block size is 16 bytes (128-bit)
            if (combinedData.Length < ivLength)
                throw new ArgumentException("Invalid cipher text (too short)");

            byte[] iv = new byte[ivLength];
            byte[] cipherBytes = new byte[combinedData.Length - ivLength];

            Buffer.BlockCopy(combinedData, 0, iv, 0, ivLength);
            Buffer.BlockCopy(combinedData, ivLength, cipherBytes, 0, cipherBytes.Length);

            // Decrypt
            using var aesAlg = Aes.Create();
            aesAlg.Key = Convert.FromBase64String(key);
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            using var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv);
            using var msDecrypt = new MemoryStream(cipherBytes);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }

        /// <summary>
        /// Aes Encrypt (GCM mode)
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GcmEncrypt(string plainText, string key)
        {
            using var aesGcm = new AesGcm(Convert.FromBase64String(key), AesGcm.TagByteSizes.MaxSize);

            // 生成随机 Nonce（12字节）
            byte[] nonce = GenerateRandomBytes(12);

            // 加密数据
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherText = new byte[plainTextBytes.Length];
            byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];

            aesGcm.Encrypt(nonce, plainTextBytes, cipherText, tag);

            // 合并 Nonce + CipherText + Tag
            byte[] combinedData = new byte[nonce.Length + cipherText.Length + tag.Length];
            Buffer.BlockCopy(nonce, 0, combinedData, 0, nonce.Length);
            Buffer.BlockCopy(cipherText, 0, combinedData, nonce.Length, cipherText.Length);
            Buffer.BlockCopy(tag, 0, combinedData, nonce.Length + cipherText.Length, tag.Length);

            // 返回 Base64 字符串
            return Convert.ToBase64String(combinedData);
        }

        /// <summary>
        /// Aes Decrypt (GCM mode)
        /// </summary>
        /// <param name="base64Input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GcmDecrypt(string base64Input, string key)
        {
            // 解码 Base64 字符串
            byte[] combinedData = Convert.FromBase64String(base64Input);

            using var aesGcm = new AesGcm(Convert.FromBase64String(key), AesGcm.TagByteSizes.MaxSize);

            // 提取 Nonce、CipherText、Tag
            int nonceLength = 12; // GCM 标准 Nonce 大小
            int tagLength = AesGcm.TagByteSizes.MaxSize; // 16 字节
            int cipherTextLength = combinedData.Length - nonceLength - tagLength;

            byte[] nonce = new byte[nonceLength];
            byte[] cipherText = new byte[cipherTextLength];
            byte[] tag = new byte[tagLength];

            Buffer.BlockCopy(combinedData, 0, nonce, 0, nonceLength);
            Buffer.BlockCopy(combinedData, nonceLength, cipherText, 0, cipherTextLength);
            Buffer.BlockCopy(combinedData, nonceLength + cipherTextLength, tag, 0, tagLength);

            // 解密
            byte[] plainTextBytes = new byte[cipherText.Length];
            aesGcm.Decrypt(nonce, cipherText, tag, plainTextBytes);

            return Encoding.UTF8.GetString(plainTextBytes);
        }

        private static byte[] GenerateRandomBytes(int length = 32)
        {
            // AES-256 requires a 32-byte key
            byte[] key = new byte[length];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }

            return key;
        }
    }
}
