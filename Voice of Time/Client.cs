using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using VoTCore.Communication;
using VoTCore.Secure;
using VoTCore.User;

/**
 * @author      - Timeplex
 * 
 * @created     - 20.12.2022
 * 
 * @last_change - 03.02.2023
 */
namespace Voice_of_Time
{
    /// <summary>
    /// Server bound Client Identity
    /// </summary>
    [Serializable]
    [KnownType(typeof(List<TextChat>))]
    [KnownType(typeof(PublicClient))]
    [KnownType(typeof(Dictionary<long, PublicClient>))]
#if DEBUG
    public class Client : ISerializable
#else
    internal class Client : ISerializable
#endif
    {
        // From the Server given unique ID
        public long UserID { get; }
        // Display Name for outher users
        public string Username { get; }
        // Public and privat Key pair
        public RSA UserKey { get; }
        // List of all Textchats
        public List<TextChat> TextChats { get; } // TODO: Dynamic serialiastion!
        // List of all Known Users
        public Dictionary<long, PublicClient> UserDB { get; }


        [JsonConstructor]
        protected Client(SerializationInfo info, StreamingContext context)
        {
            UserID       = info.GetInt64(nameof(UserID));

            Username     = info.GetString(nameof(Username)) ?? throw new Exception(nameof(Username) + " coudn't be loaded!");

            var keyAsXML = info.GetString(nameof(UserKey))  ?? throw new Exception(nameof(UserKey) +  " coudn't be loaded!");
            UserKey      = RSA.Create();
            UserKey.FromXmlString(keyAsXML);

            TextChats    = (List<TextChat>?)                info.GetValue(nameof(TextChats), typeof(List<TextChat>))              ?? new();

            UserDB       = (Dictionary<long, PublicClient>?)info.GetValue(nameof(UserDB), typeof(Dictionary<long, PublicClient>)) ?? new();
        }

        public Client(long userID, string username, RSA userKey, List<TextChat>? textChats = null, Dictionary<long, PublicClient>? userDB = null)
        {
            UserID    = userID;
            Username  = username  ?? throw new ArgumentNullException(nameof(username));
            UserKey   = userKey   ?? throw new ArgumentNullException(nameof(userKey));
            TextChats = textChats ?? new();
            UserDB    = userDB    ?? new();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var KeyAsXML = UserKey.ToXmlString(true);

            info.AddValue(nameof(UserID),    UserID);
            info.AddValue(nameof(Username),  Username);
            info.AddValue(nameof(UserKey),   KeyAsXML);
            info.AddValue(nameof(TextChats), TextChats);
            info.AddValue(nameof(UserDB),    UserDB);

        }
    }
}
