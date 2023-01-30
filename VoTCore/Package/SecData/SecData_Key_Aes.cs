using System.Security.Cryptography;
using System.Text.Json.Serialization;
using VoTCore.Package.Interfaces;
using VoTCore.Secure.Iterfaces;

/**
 * @author      - Timeplex
 * 
 * @created     - 20.01.2023
 * 
 * @last_change - 21.01.2023
 */
namespace VoTCore.Package.SecData
{
    public class SecData_Key_Aes : IVOTPBody, IRSACrypt
    {
        private string keyAsB64;
        private string iVAsB64;

        public string KeyAsB64 => keyAsB64;

        public string IVAsB64  => iVAsB64; 

        public long SourceID { get; }

        [JsonIgnore]
        public BodyType Type => BodyType.SECDATA_KEY_AES;

        private long? cryptedReciver;
        public long CryptedReciver => cryptedReciver is null ? -1 : (long)cryptedReciver;

        public SecData_Key_Aes(Aes publicKey, long sourceID)
        {
            keyAsB64 = Convert.ToBase64String(publicKey.Key);
            iVAsB64 = Convert.ToBase64String(publicKey.IV);
            SourceID = sourceID;
        }

        /// <summary>
        /// For deserialiastion purposes only!
        /// </summary>
        /// <param name="keyXML">Public key as XML</param>
        /// <param name="sourceID">User/Group from with the key originates</param>
        [JsonConstructor]
        public SecData_Key_Aes(string keyAsB64, string iVAsB64, long sourceID, long cryptedReciver)
        {
            SourceID                = sourceID;
            this.keyAsB64           = keyAsB64;
            this.iVAsB64            = iVAsB64;
            this.cryptedReciver     = cryptedReciver;
        }

        public Aes GetKey()
        {
            var key = Aes.Create();
            key.Key = Convert.FromBase64String(keyAsB64);
            key.IV = Convert.FromBase64String(iVAsB64);
            return key;
        }

        public void EncryptData(RSA rsa, long revicerID)
        {
            if (cryptedReciver is not null) return;

            cryptedReciver = revicerID;

            var key = Convert.FromBase64String(keyAsB64);
            var iv = Convert.FromBase64String(iVAsB64);

            key = rsa.Encrypt(key, RSAEncryptionPadding.Pkcs1);
            iv  = rsa.Encrypt(iv, RSAEncryptionPadding.Pkcs1);

            keyAsB64 = Convert.ToBase64String(key);
            iVAsB64  = Convert.ToBase64String(iv);
        }

        public void DecryptData(RSA rsa)
        {
            if (cryptedReciver is null) return;

            var key = Convert.FromBase64String(keyAsB64);
            var iv = Convert.FromBase64String(iVAsB64);

            key = rsa.Decrypt(key, RSAEncryptionPadding.Pkcs1);
            iv  = rsa.Decrypt(iv,  RSAEncryptionPadding.Pkcs1);

            keyAsB64 = Convert.ToBase64String(key);
            iVAsB64  = Convert.ToBase64String(iv);

            cryptedReciver = null;
        }
    }
}
