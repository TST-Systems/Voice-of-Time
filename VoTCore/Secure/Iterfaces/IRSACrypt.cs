using System.Security.Cryptography;
using VoTCore.User;

/**
 * @author      - Timeplex
 * 
 * @created     - 21.01.2023
 * 
 * @last_change - 21.01.2023
 */
namespace VoTCore.Secure.Iterfaces
{
    /// <summary>
    /// Interface for classes which can be enrcypted and decrypteded with an RSA key
    /// </summary>
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
        /// Wrapper for <see cref="EncryptData"/>
        /// </summary>
        /// <param name="reciver"></param>
        public void EncryptData(PublicClient reciver)
        {
            EncryptData(reciver.Key.PublicKey, reciver.UserID);
        }
        /// <summary>
        /// Decrypt Data
        /// </summary>
        /// <param name="key"></param>
        public void DecryptData(RSA key);

    }
}