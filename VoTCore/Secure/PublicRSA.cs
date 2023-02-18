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
    [DataContract]
    public class PublicRSA
    {
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

        private byte[] modulus;
        [DataMember]
        public byte[] Modulus { get => modulus; init => modulus = value; }

        private byte[] exponent;
        [DataMember]
        public byte[] Exponent { get => exponent; init => exponent = value; }

        [JsonConstructor]
        public PublicRSA(byte[] modulus, byte[] exponent)
        {
            this.modulus  = modulus;
            this.exponent = exponent;
        }

        public PublicRSA(RSA key)
        {
            var keyParameters = key.ExportParameters(false);
            modulus  = keyParameters.Modulus ?? throw new Exception("Key is missing a Part: Modulus");
            exponent = keyParameters.Exponent ?? throw new Exception("Key is missing a Part: Exponent");
        }

        public PublicRSA() : this(RSA.Create()) {}
    }
}
