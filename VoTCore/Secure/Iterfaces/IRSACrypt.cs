using System.Security.Cryptography;

namespace VoTCore.Secure.Iterfaces
{
    public interface IRSACrypt
    {
        /// <summary>
        /// Givs the userID of the reciver. If no encryption is set this will retun < 0
        /// </summary>
        long CryptedReciver { get; }
        /// <summary>
        /// Encrypt Data 
        /// </summary>
        /// <param name="key">Public Key needs to be present</param>
        public void EncryptData(RSA key, long revicerID);
        /// <summary>
        /// Decrypt Data
        /// </summary>
        /// <param name="key"></param>
        public void DecryptData(RSA key);

    }
}