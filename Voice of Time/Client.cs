using System.Security.Cryptography;
using VoTCore.Secure;

namespace Voice_of_Time
{
    /// <summary>
    /// Server bound Client Identity
    /// </summary>
#if DEBUG
    public class Client : PublicKeyStorage
#else
    internal class Client : PublicKeyStorage
#endif
    {
        // From the Server given unique ID
        public long UserID { get; }
        // Display Name for outher users
        public string UserName { get; }
        // Public and privat Key pair
        public RSA UserKey { get; }
        // List of all Textchats
        public List<TextChat> TextChats { get; }

        public Client(long userID, string userName, RSA userKey, Dictionary<long, RSA>? publicKeyDictionary = null, List<TextChat>? textChats = null)
            :base(publicKeyDictionary)
        {
            UserID              = userID;
            UserName            = userName  ?? throw new ArgumentNullException(nameof(userName));
            UserKey             = userKey   ?? throw new ArgumentNullException(nameof(userKey));
            TextChats           = textChats ?? new();
        }

        public override KeyStatus AddPublicKey(long targetID, RSA publicKey)
        {
            //Check if target is self
            if (targetID == UserID) { return KeyStatus.SELF; }
            return base.AddPublicKey(targetID, publicKey);
        }

    }
}
