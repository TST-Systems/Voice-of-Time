using System.Security.Cryptography;
using VoTCore.Communication;
using VoTCore.Secure;
using VoTCore.User;

/**
 * @author      - Timeplex
 * 
 * @created     - 20.12.2022
 * 
 * @last_change - 01.02.2023
 */
namespace Voice_of_Time
{
    /// <summary>
    /// Server bound Client Identity
    /// </summary>
#if DEBUG
    public class Client 
#else
    internal class Client
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
        // List of all Known Users
        public Dictionary<long, PublicClient> UserDB { get; }


        public Client(long userID, string username, RSA userKey, List<TextChat>? textChats = null, Dictionary<long, PublicClient>? userDB = null)
        {
            UserID    = userID;
            Username  = username  ?? throw new ArgumentNullException(nameof(username));
            UserKey   = userKey   ?? throw new ArgumentNullException(nameof(userKey));
            TextChats = textChats ?? new();
            UserDB    = userDB    ?? new();
        }

    }
}
