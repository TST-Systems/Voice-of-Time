using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using VoTCore.Communication;
using VoTCore.Communication.Data;
using VoTCore.User;

/**
 * @author      - Timeplex
 * 
 * @created     - 20.12.2022
 * 
 * @last_change - 09.02.2023
 */
namespace Voice_of_Time.User
{
    /// <summary>
    /// Per-Server client data storage and data handling
    /// </summary>
    [Serializable]
    [KnownType(typeof(List<TextChat>))]
    [KnownType(typeof(PublicClient))]
    [KnownType(typeof(Dictionary<long, PublicClient>))]
    [KnownType(typeof(Dictionary<(long, long), ReceiptStatus>))]
    [KnownType(typeof(ReceiptStatus))]
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
        // List of all Known Public Users
        public Dictionary<long, PublicClient> UserDB { get; }

        // Receipt system
        public Dictionary<(long, long), ReceiptStatus> ReceiptStatusDictionary { get; }

        /// <summary>
        /// Constructor for XML construction
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// <exception cref="Exception"></exception>
        protected Client(SerializationInfo info, StreamingContext context)
        {
            UserID = info.GetInt64(nameof(UserID));

            Username = info.GetString(nameof(Username)) ?? throw new Exception(nameof(Username) + " coudn't be loaded!");

            var keyAsXML = info.GetString(nameof(UserKey)) ?? throw new Exception(nameof(UserKey) + " coudn't be loaded!");
            UserKey = RSA.Create();
            UserKey.FromXmlString(keyAsXML);

            TextChats = (List<TextChat>?)info.GetValue(nameof(TextChats), typeof(List<TextChat>)) ?? new();

            UserDB = (Dictionary<long, PublicClient>?)info.GetValue(nameof(UserDB), typeof(Dictionary<long, PublicClient>)) ?? new();

            ReceiptStatusDictionary = (Dictionary<(long, long), ReceiptStatus>?)
                info.GetValue(nameof(ReceiptStatusDictionary), typeof(Dictionary<(long, long), ReceiptStatus>)) ?? new();
        }

        /// <summary>
        /// Default constructor for Client
        /// </summary>
        /// <param name="userID">ID of user</param>
        /// <param name="username">Username of user</param>
        /// <param name="userKey">Public and Privat RSA key of user</param>
        /// <param name="textChats">List of all text-based chats</param>
        /// <param name="userDB">Dictonay of PublicClients</param>
        /// <param name="receiptStatusDictionary">Dictornary of Receipts and their current work status</param>
        /// <exception cref="ArgumentNullException"></exception>
        public Client(long userID, string username, RSA userKey, List<TextChat>? textChats = null, Dictionary<long, PublicClient>? userDB = null, Dictionary<(long, long), ReceiptStatus>? receiptStatusDictionary = null)
        {
            UserID    = userID;
            Username  = username  ?? throw new ArgumentNullException(nameof(username));
            UserKey   = userKey   ?? throw new ArgumentNullException(nameof(userKey));
            TextChats = textChats ?? new();
            UserDB    = userDB    ?? new();
            ReceiptStatusDictionary = receiptStatusDictionary ?? new();
        }

        /// <summary>
        /// Add, if not exists, a new public client to the UserDB
        /// </summary>
        /// <param name="publicClient">Public client to add</param>
        /// <returns>User was added AND publicClient is ok</returns>
        public bool AppendPublicClient(PublicClient publicClient)
        {
            var id = publicClient.UserID;
            // Check if user tries to use a non authorized id
            if (id < 0) return false;
            // Check if user is already known
            if (UserDB.ContainsKey(id)) return false;

            UserDB.Add(id, publicClient);
            return true;
        }

        /// <summary>
        /// Add or overide a public client to/on the UserDB
        /// </summary>
        /// <param name="publicClient">Public client to add</param>
        /// <returns>User was added AND publicClient is ok</returns>
        public bool AppendOrOverridePublicClint(PublicClient publicClient)
        {
            var id = publicClient.UserID;
            // Check if user tries to use a non authorized id
            if (id < 0) return false;
            UserDB[id] = publicClient;
            return true;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var KeyAsXML = UserKey.ToXmlString(true);

            info.AddValue(nameof(UserID), UserID);
            info.AddValue(nameof(Username), Username);
            info.AddValue(nameof(UserKey), KeyAsXML);

            info.AddValue(nameof(TextChats), TextChats);

            info.AddValue(nameof(UserDB), UserDB);

            info.AddValue(nameof(ReceiptStatusDictionary), ReceiptStatusDictionary);
        }
    }
}
