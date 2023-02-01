using System.Security.Cryptography;

/**
 * @author      - Timeplex
 * 
 * @created     - 01.02.2023
 * 
 * @last_change - 01.02.2023
 */
namespace VoTCore.Secure
{
    public class PublicRSA
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

        public PublicRSA(RSA key)
        {
            var lKey = RSA.Create();
            lKey.ImportParameters(key.ExportParameters(false));
            this.key = lKey;
        }

        public void ChangeKey(RSA key)
        {
            this.key.ImportParameters(key.ExportParameters(false));
        }
    }
}
