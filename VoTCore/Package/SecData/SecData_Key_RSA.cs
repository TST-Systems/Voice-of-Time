using System.Security.Cryptography;
using System.Text.Json.Serialization;
using VoTCore.Package.Interfaces;
using VoTCore.Secure;

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
    /// Body for sending only the public part of a RSA key
    /// </summary>
    public class SecData_Key_RSA : IVOTPBody
    {
        // Public key parts
        public byte[] Modulus  { get; }
        public byte[] Exponent { get; }
        // Public key owner
        public long   SourceID { get; }

        [JsonIgnore]
        public virtual BodyType Type => BodyType.SECDATA_KEY_RSA;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="publicKey">RSA key (only the public part will be used)</param>
        /// <param name="sourceID">User/Group from with the key originates</param>
        public SecData_Key_RSA(RSA publicKey, long sourceID)
        {
            var KeyInfo = publicKey.ExportParameters(false);
            Modulus     = KeyInfo.Modulus  ?? Array.Empty<byte>();
            Exponent    = KeyInfo.Exponent ?? Array.Empty<byte>();
            SourceID    = sourceID;
        }

        /// <summary>
        /// JSON constrcutor
        /// </summary>
        /// <param name="modulus"></param>
        /// <param name="exponent"></param>
        /// <param name="sourceID">User/Group from with the key originates</param>
        [JsonConstructor]
        public SecData_Key_RSA(byte[] modulus, byte[] exponent, long sourceID)
        {
            Modulus  = modulus;
            Exponent = exponent;
            SourceID = sourceID;
        }

        /// <summary>
        /// Wrapper for default contrcutor
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="sourceID">User/Group from with the key originates</param>
        public SecData_Key_RSA(PublicRSA publicKey, long sourceID) : this(publicKey.PublicKey, sourceID) { }

        /// <summary>
        /// Get the RSA key with only public parts
        /// </summary>
        /// <returns>RSA key (only public)</returns>
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
