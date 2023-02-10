using System.Security.Cryptography;
using System.Text.Json.Serialization;
using VoTCore.Package.Interfaces;
using VoTCore.Secure;

/**
 * @author      - Timeplex
 * 
 * @created     - 20.01.2023
 * 
 * @last_change - 20.01.2023
 */
namespace VoTCore.Package.SecData
{
    public class SecData_Key_RSA : IVOTPBody
    {
        public byte[] Modulus  { get; }
        public byte[] Exponent { get; }

        public long   SourceID { get; }

        [JsonIgnore]
        public virtual BodyType Type => BodyType.SECDATA_KEY_RSA;

        public SecData_Key_RSA(RSA publicKey, long sourceID)
        {
            var KeyInfo = publicKey.ExportParameters(false);
            Modulus     = KeyInfo.Modulus  ?? Array.Empty<byte>();
            Exponent    = KeyInfo.Exponent ?? Array.Empty<byte>();
            SourceID    = sourceID;
        }

        /// <summary>
        /// For deserialiastion purposes only! If any privat key part is their, this constructor will throw an error.
        /// </summary>
        /// <param name="keyXML">Public key as XML</param>
        /// <param name="sourceID">User/Group from with the key originates</param>
        [JsonConstructor]
        public SecData_Key_RSA(byte[] modulus, byte[] exponent, long sourceID)
        {
            Modulus  = modulus;
            Exponent = exponent;
            SourceID = sourceID;
        }

        public SecData_Key_RSA(PublicRSA publicKey, long sourceID) : this(publicKey.Key, sourceID) { }

        public RSA GetKey()
        {
            var KeyParameter = new RSAParameters
            {
                Modulus  = Modulus,
                Exponent = Exponent
            };

            var Key = RSA.Create(KeyParameter);

            return Key;
        }
    }
}
