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
        public Dictionary<long, byte[]> PublicKeyDictionary { get; }

        public Client(long userID, string userName, RSA userKey, Dictionary<long, byte[]>? publicKeyDictionary = null)
        {
            UserID              = userID;
            UserName            = userName            ?? throw new ArgumentNullException(nameof(userName));
            UserKey             = userKey             ?? throw new ArgumentNullException(nameof(userKey));
            PublicKeyDictionary = publicKeyDictionary ?? new();
        }
    }
}
