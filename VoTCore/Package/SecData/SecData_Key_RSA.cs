using System.Security.Cryptography;
using System.Text.Json.Serialization;
using VoTCore.Package.Interfaces;

namespace VoTCore.Package.SecData
{
    public class SecData_Key_RSA : IVOTPBody
    {
        public string PublicKeyAsXML { get; }
        public long   SourceID { get; }

        [JsonIgnore]
        public BodyType Type => BodyType.SECDATA_KEY_RSA;

        public SecData_Key_RSA(RSA publicKey, long sourceID)
        {
            PublicKeyAsXML = publicKey.ToXmlString(false);
            SourceID       = sourceID;
        }

        /// <summary>
        /// For deserialiastion purposes only! If any privat key part is their, this constructor will throw an error.
        /// </summary>
        /// <param name="keyXML">Public key as XML</param>
        /// <param name="sourceID">User/Group from with the key originates</param>
        [JsonConstructor]
        public SecData_Key_RSA(string publicKeyAsXML, long sourceID)
        {
            var testRSAKey = RSA.Create();
            testRSAKey.FromXmlString(publicKeyAsXML);
            try
            {
                _ = testRSAKey.ExportParameters(true);
                throw new Exception();
            }
            catch(CryptographicException) { }
            PublicKeyAsXML = publicKeyAsXML;
            SourceID       = sourceID;
        }

        public RSA GetKey()
        { 
            var Key = RSA.Create();
            Key.FromXmlString(PublicKeyAsXML);
            return Key;
        }
    }
}
