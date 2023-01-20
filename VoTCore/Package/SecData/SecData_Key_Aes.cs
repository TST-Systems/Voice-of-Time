using System.Security.Cryptography;
using System.Text.Json.Serialization;
using VoTCore.Package.Interfaces;

namespace VoTCore.Package.SecData
{
    public class SecData_Key_Aes : IVOTPBody
    {
        public string KeyAsB64 { get; }
        public string IVAsB64 { get; }
        public long   SourceID { get; }

        [JsonIgnore]
        public BodyType Type => BodyType.SECDATA_KEY_AES;

        public SecData_Key_Aes(Aes publicKey, long sourceID)
        {
            KeyAsB64 = Convert.ToBase64String(publicKey.Key);
            IVAsB64  = Convert.ToBase64String(publicKey.IV);
            SourceID = sourceID;
        }

        /// <summary>
        /// For deserialiastion purposes only!
        /// </summary>
        /// <param name="keyXML">Public key as XML</param>
        /// <param name="sourceID">User/Group from with the key originates</param>
        [JsonConstructor]
        public SecData_Key_Aes(string keyAsB64, string iVAsB64 , long sourceID)
        {
            KeyAsB64 = keyAsB64;
            IVAsB64  = iVAsB64;
            SourceID = sourceID;
        }

        public Aes GetKey()
        {
            var key = Aes.Create();
            key.Key = Convert.FromBase64String(KeyAsB64);
            key.IV  = Convert.FromBase64String(IVAsB64);
            return key;
        }
    }
}
