using System.Security.Cryptography;
using VoTCore.Communication;
using VoTCore.Secure;

/**
 * @author      - Timeplex
 * 
 * @created     - 20.12.2022
 * 
 * @last_change - 20.01.2023
 */
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
        public string Username { get; }
        // Public and privat Key pair
        public RSA UserKey { get; }
        // List of all Textchats
        public List<TextChat> TextChats { get; }

        public Client(long userID, string username, RSA userKey, Dictionary<long, RSA>? publicKeyDictionary = null, List<TextChat>? textChats = null)
            : base(publicKeyDictionary)
        {
            UserID    = userID;
            Username  = username  ?? throw new ArgumentNullException(nameof(username));
            UserKey   = userKey   ?? throw new ArgumentNullException(nameof(userKey));
            TextChats = textChats ?? new();
        }

        public override KeyStatus AddPublicKey(long targetID, RSA publicKey)
        {
            //Check if target is self
            if (targetID == UserID) { return KeyStatus.SELF; }
            return base.AddPublicKey(targetID, publicKey);
        }

    }
}
