using System.Runtime.Serialization;
using System.Security.Cryptography;

/**
 * @author      - Timeplex
 * 
 * @created     - 01.02.2023
 * 
 * @last_change - 03.02.2023
 */
namespace VoTCore.Secure
{
    [Serializable]
    public class PublicRSA : ISerializable
    {
        private readonly RSA key;
        public RSA Key { 
            get 
            { 
                var result = RSA.Create();
                result.ImportParameters(key.ExportParameters(false));
                return result; 
            } 
        }

        protected PublicRSA(SerializationInfo info, StreamingContext context)
        {
            var keyAsXML = info.GetString(nameof(key));
            if (keyAsXML is null or "") throw new Exception("Key not found!");
            key = RSA.Create();
            key.FromXmlString(keyAsXML);
        }

        public PublicRSA(RSA key)
        {
            var lKey = RSA.Create();
            lKey.ImportParameters(key.ExportParameters(false));
            this.key = lKey;
        }

        public PublicRSA()
        {
            key = RSA.Create();
        }

        public void ChangeKey(RSA key)
        {
            this.key.ImportParameters(key.ExportParameters(false));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var keyAsXML = key.ToXmlString(false);
            info.AddValue(nameof(key), keyAsXML);
        }
    }
}
