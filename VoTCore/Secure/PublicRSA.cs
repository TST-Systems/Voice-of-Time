using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

/**
 * @author      - Timeplex
 * 
 * @created     - 01.02.2023
 * 
 * @last_change - 15.02.2023
 */
namespace VoTCore.Secure
{
    /// <summary>
    /// Class to hold the public key parts of a RSA key for safe handling
    /// </summary>
    [DataContract]
    public class PublicRSA
    {
        /// <summary>
        /// Get RSA key (public only)
        /// </summary>
        [JsonIgnore]
        public RSA PublicKey
        {
            get
            {
                var keyParameters = new RSAParameters
                {
                    Modulus = modulus,
                    Exponent = exponent
                };
                return RSA.Create(keyParameters);
            }
            set
            {
                var keyParameters = value.ExportParameters(false);
                modulus  = keyParameters.Modulus  ?? throw new Exception("Key is missing a Part: Modulus");
                exponent = keyParameters.Exponent ?? throw new Exception("Key is missing a Part: Exponent");
            }
        }

        // Public key parts
        private byte[] modulus;
        [DataMember]
        public byte[] Modulus { get => modulus; init => modulus = value; }

        private byte[] exponent;
        [DataMember]
        public byte[] Exponent { get => exponent; init => exponent = value; }

        /// <summary>
        /// JSON constructor
        /// </summary>
        /// <param name="modulus"></param>
        /// <param name="exponent"></param>
        [JsonConstructor]
        public PublicRSA(byte[] modulus, byte[] exponent)
        {
            this.modulus  = modulus;
            this.exponent = exponent;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="key">RSA key (only public key parts will be stored)</param>
        /// <exception cref="Exception">Key is broken</exception>
        public PublicRSA(RSA key)
        {
            var keyParameters = key.ExportParameters(false);
            modulus  = keyParameters.Modulus  ?? throw new Exception("Key is missing a Part: Modulus");
            exponent = keyParameters.Exponent ?? throw new Exception("Key is missing a Part: Exponent");
        }

        /// <summary>
        /// Create a new RSA key
        /// </summary>
        public PublicRSA() : this(RSA.Create()) {}
    }
}
