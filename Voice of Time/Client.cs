using System.Security.Cryptography;

namespace Voice_of_Time
{
    /// <summary>
    /// Server bound Client Identity
    /// </summary>
    internal class Client
    {
        public long UserID { get; }
        public string UserName { get; }
        public RSA UserKey { get; }
        private Dictionary<long, RSA> PublicKeyDictionary {get; }
        public Dictionary<long, RSA> PublicKeyDictionaryCopy
        {
            get
            {
                var copy = new Dictionary<long, RSA>(PublicKeyDictionary);
                return copy;
            }
        }
        public List<TextChat> TextChats { get; }

        public Client(long userID, string userName, RSA userKey, Dictionary<long, RSA>? publicKeyDictionary = null, List<TextChat>? textChats = null)
        {
            UserID              = userID;
            UserName            = userName            ?? throw new ArgumentNullException(nameof(userName));
            UserKey             = userKey             ?? throw new ArgumentNullException(nameof(userKey));
            PublicKeyDictionary = publicKeyDictionary ?? new();
            TextChats           = textChats           ?? new();
        }

        public KeyStatus AddPublicKey (long targetID, RSA publicKey)
        {
            //Check if target is self
            if (targetID == UserID) { return KeyStatus.SELF; }
            //Check if the Key contains any part of the private key
            try
            {
                var fullKeyParam = publicKey.ExportParameters(true);
                return KeyStatus.PRIVATE_KEY_FOUND;
            }
            catch (Exception)
            {
                /*throw new ArgumentException(ex.Message);*/
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
    }

    internal enum KeyStatus
    {
        ADD,
        OVERWRITE,
        NO_CHANGE,
        PRIVATE_KEY_FOUND,
        SELF
    }
}
