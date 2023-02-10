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
 * @last_change - 06.02.2023
 */
namespace VoTCore.Package.SecData
{
    public class SecData_Key_Aes : IVOTPBody, IRSACrypt
    {
        private byte[] key;
        private byte[] iv;

        public byte[] Key { get => key; }
        public byte[] IV  { get => iv;  }

        public long SourceID { get; }

        [JsonIgnore]
        public BodyType Type => BodyType.SECDATA_KEY_AES;

        private long? cryptedReciver;
        public long CryptedReciver => cryptedReciver is null ? -1 : (long)cryptedReciver;

        public SecData_Key_Aes(Aes publicKey, long sourceID)
        {
            key      = publicKey.Key;
            iv       = publicKey.IV;
            SourceID = sourceID;
        }

        /// <summary>
        /// For deserialiastion purposes only!
        /// </summary>
        /// <param name="keyXML">Public key as XML</param>
        /// <param name="sourceID">User/Group from with the key originates</param>
        [JsonConstructor]
        public SecData_Key_Aes(byte[] key, byte[] iv, long sourceID, long cryptedReciver)
        {
            SourceID            = sourceID;
            this.key            = key;
            this.iv             = iv;
            this.cryptedReciver = cryptedReciver;
        }

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

        public void EncryptData(PublicClient target) 
        {
            if (target.PublicKey is null) throw new PublicKeyMissingExeption(target);
            EncryptData(target.PublicKey.Key, target.ID);
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
