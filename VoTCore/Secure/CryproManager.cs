using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

/**
 * @author      - Timeplex
 * 
 * @created     - 30.01.2023
 * 
 * @last_change - 30.01.2023
 */
namespace VoTCore.Secure
{
    public static class CryproManager
    {
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

        public static byte[] AesEncyrpt(Aes key, byte[] toEncrypt)
        {
            return AesEncyrpt(key, toEncrypt, 0, toEncrypt.Length);
        }

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

        public static byte[] AesDecyrpt(Aes key, byte[] toDecrypt)
        {
            return AesDecyrpt(key, toDecrypt, 0, toDecrypt.Length);
        }
    }
}
