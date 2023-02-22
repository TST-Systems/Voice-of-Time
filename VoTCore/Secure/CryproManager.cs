using System.Security.Cryptography;

/**
 * @author      - Timeplex
 * 
 * @created     - 30.01.2023
 * 
 * @last_change - 30.01.2023
 */
namespace VoTCore.Secure
{
    /// <summary>
    /// Static class for methoths used for enrypting and decrypting
    /// </summary>
    public static class CryproManager
    {
        /// <summary>
        /// Encrypt data with a Aes key
        /// </summary>
        /// <param name="key">Aes full key</param>
        /// <param name="toEncrypt">Array of byts to encrypt</param>
        /// <param name="offset">parmeter of <see cref="MemoryStream"/></param>
        /// <param name="lenght">parmeter of <see cref="MemoryStream"/></param>
        /// <returns>Enrypted data</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] AesEncyrpt(Aes key, byte[] toEncrypt, int offset, int lenght)
        {
            if (key is null) throw new ArgumentNullException(nameof(key),"Key cannot be null");

            using var encryptor = key.CreateEncryptor();
            using MemoryStream memoryStream = new();
            using CryptoStream cryptostream = new(memoryStream, encryptor, CryptoStreamMode.Write);

            cryptostream.Write(toEncrypt, offset, lenght);
            cryptostream.FlushFinalBlock();

            var encypted = memoryStream.ToArray();

            cryptostream.Close();
            memoryStream.Close();

            return encypted;
        }

        /// <summary>
        /// Wrapper for <see cref="AesEncyrpt"/> by using default values for not nessary fields
        /// </summary>
        /// <param name="key">Aes full key</param>
        /// <param name="toEncrypt">Array of byts to encrypt</param>
        /// <returns>Enrypted data</returns>
        public static byte[] AesEncyrpt(Aes key, byte[] toEncrypt)
        {
            return AesEncyrpt(key, toEncrypt, 0, toEncrypt.Length);
        }

        /// <summary>
        /// Decrypt data with a Aes key
        /// </summary>
        /// <param name="key">Aes full key</param>
        /// <param name="toDecrypt">Array of byts to decrypt</param>
        /// <param name="offset">parmeter of <see cref="MemoryStream"/></param>
        /// <param name="lenght">parmeter of <see cref="MemoryStream"/></param>
        /// <returns>Derypted data</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static byte[] AesDecyrpt(Aes key, byte[] toDecrypt, int offset, int lenght)
        {
            if (key is null) throw new ArgumentNullException(nameof(key), "Key cannot be null");

            using var encryptor = key.CreateDecryptor();
            using MemoryStream memoryStream = new();
            using CryptoStream cryptostream = new(memoryStream, encryptor, CryptoStreamMode.Write);

            cryptostream.Write(toDecrypt, offset, lenght);
            cryptostream.FlushFinalBlock();

            var decrypted = memoryStream.ToArray();

            cryptostream.Close();
            memoryStream.Close();

            return decrypted;
        }

        /// <summary>
        /// Wrapper for <see cref="AesDecyrpt"/> by using default values for not nessary fields
        /// </summary>
        /// <param name="key">Aes full key</param>
        /// <param name="toDecrypt">Array of byts to decrypt</param>
        /// <returns>Derypted data</returns>
        public static byte[] AesDecyrpt(Aes key, byte[] toDecrypt)
        {
            return AesDecyrpt(key, toDecrypt, 0, toDecrypt.Length);
        }
    }
}
