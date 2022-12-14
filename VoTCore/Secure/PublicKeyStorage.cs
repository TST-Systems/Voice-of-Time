using System.Security.Cryptography;

namespace VoTCore.Secure
{
    public class PublicKeyStorage
    {
        private Dictionary<long, RSA> PublicKeyDictionary { get; }
        public Dictionary<long, RSA> PublicKeyDictionaryCopy
        {
            get
            {
                var copy = new Dictionary<long, RSA>(PublicKeyDictionary);
                return copy;
            }
        }

        public PublicKeyStorage(Dictionary<long, RSA>? publicKeyDictionary = null)
        {
            PublicKeyDictionary = publicKeyDictionary ?? new();
        }

        public virtual KeyStatus AddPublicKey(long targetID, RSA publicKey)
        {
            //Check if the Key contains any part of the private key
            try
            {
                var fullKeyParam = publicKey.ExportParameters(true);
                return KeyStatus.PRIVATE_KEY_FOUND;
            }
            catch (Exception)
            {
                /*throw new ArgumentException(ex.Message);*/
            }
            //Check if the User is allready known
            if (!PublicKeyDictionary.ContainsKey(targetID))
            {
                PublicKeyDictionary[targetID] = publicKey;
                return KeyStatus.ADD;
            }
            //If yes check if public Key is the same
            if (PublicKeyDictionary[targetID].Equals(publicKey))
            {
                return KeyStatus.NO_CHANGE;
            }
            // else override it
            PublicKeyDictionary[targetID] = publicKey;
            return KeyStatus.OVERWRITE;
        }

    }

    public enum KeyStatus
    {
        ADD,
        OVERWRITE,
        NO_CHANGE,
        PRIVATE_KEY_FOUND,
        SELF
    }
}
