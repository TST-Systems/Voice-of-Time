using System.Security.Cryptography;
using System.Text.Json.Serialization;
using VoTCore.Exeptions;
using VoTCore.Package.Interfaces;
using VoTCore.Secure.Iterfaces;
using VoTCore.User;

/**
 * @author      - Timeplex
 * 
 * @created     - 20.01.2023
 * 
 * @last_change - 10.02.2023
 */
namespace VoTCore.Package.SecData
{
    /// <summary>
    /// Body for sending aes keys securly
    /// </summary>
    public class SecData_Key_Aes : IVOTPBody, IRSACrypt
    {
        // Parts of the key
        private byte[] key;
        private byte[] iv;

        public byte[] Key { get => key; }
        public byte[] IV  { get => iv;  }

        /// <summary>
        /// Source id of key (chat/client/channel)
        /// </summary>
        public long SourceID { get; }

        [JsonIgnore]
        public BodyType Type => BodyType.SECDATA_KEY_AES;

        private long? cryptedReciver;
        public long CryptedReciver => cryptedReciver is null ? -1 : (long)cryptedReciver;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="publicKey">Key to transfer</param>
        /// <param name="sourceID">ID of source</param>
        public SecData_Key_Aes(Aes publicKey, long sourceID)
        {
            key      = publicKey.Key;
            iv       = publicKey.IV;
            SourceID = sourceID;
        }

        /// <summary>
        /// JSON constructror
        /// </summary>
        /// <param name="sourceID">User/Group from with the key originates</param>
        /// <param name="key">Aes.key</param>
        /// <param name="iv">Aes.iv</param>
        /// <param name="cryptedReciver">Target id for encryption</param>
        [JsonConstructor]
        public SecData_Key_Aes(byte[] key, byte[] iv, long sourceID, long cryptedReciver)
        {
            SourceID            = sourceID;
            this.key            = key;
            this.iv             = iv;
            this.cryptedReciver = cryptedReciver;
        }

        /// <summary>
        /// Get the key
        /// </summary>
        /// <returns>Aes key</returns>
        public Aes GetKey()
        {
            var aesKey = Aes.Create();
            aesKey.Key = key;
            aesKey.IV  = iv;
            return aesKey;
        }

        public void EncryptData(RSA rsa, long revicerID)
        {
            if (cryptedReciver is not null) return;

            cryptedReciver = revicerID;

            key = rsa.Encrypt(key, RSAEncryptionPadding.Pkcs1);
            iv  = rsa.Encrypt(iv,  RSAEncryptionPadding.Pkcs1);
        }


        public void DecryptData(RSA rsa)
        {
            if (cryptedReciver is null) return;

            key = rsa.Decrypt(key, RSAEncryptionPadding.Pkcs1);
            iv  = rsa.Decrypt(iv,  RSAEncryptionPadding.Pkcs1);

            cryptedReciver = null;
        }
    }
}
