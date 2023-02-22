using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using VoTCore.Package.Interfaces;
using VoTCore.Secure.Iterfaces;

/**
 * @author      - Timeplex
 * 
 * @created     - 30.01.2023
 * 
 * @last_change - 08.02.2023
 */
namespace VoTCore.Communication
{
    [Serializable]
    [KnownType(typeof(List<long>))]
    public class PrivatChat : TextChat, ISerializable, IVOTPBody, IRSACrypt
    {
        /// <summary>
        /// A List of all Participants, including self. 
        /// Can be used to determind the reciver of a message for a speific chat
        /// </summary>
        public List<long> Participants { get => new(participants); }
        private readonly List<long> participants;
        /// <summary>      
        /// ID under wich the server can recognize this chat
        /// </summary>
        public long ChatID { get; }
        /// <summary>
        /// Name displayed in GUI or CMD as chat name/title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Communication Key for messages
        /// </summary>
        [JsonIgnore]
        public Aes Key 
        { 
            get
            {
                var key = Aes.Create();
                key.IV  = GroupIV;
                key.Key = GroupKey;
                return key;
            }
        }
        public byte[] GroupKey { get; set; } //TODO
        public byte[] GroupIV  { get; set; } //TODO: private schreibschutz von außen

        [JsonIgnore]
        public BodyType Type => BodyType.PRIVAT_CHAT;

        private long? cryptedReciver;
        public long CryptedReciver { get { if (cryptedReciver is null) return -1; return (long)cryptedReciver; } }

        /// <summary>
        /// Constructor for JSON
        /// </summary>
        /// <param name="participants"></param>
        /// <param name="chatID"></param>
        /// <param name="title"></param>
        /// <param name="cryptedReciver"></param>
        /// <param name="groupKey"></param>
        /// <param name="groupIV"></param>
        [JsonConstructor]
        public PrivatChat(List<long> participants, long chatID, string title, long cryptedReciver, byte[] groupKey, byte[] groupIV) 
        {
            this.participants   = participants;
            ChatID              = chatID;
            Title               = title;
            GroupKey            = groupKey;
            GroupIV             = groupIV;
            this.cryptedReciver = cryptedReciver;
        }

        /// <summary>
        /// Constructor for DataContract
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// <exception cref="Exception"></exception>
        protected PrivatChat(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            var _participants = info.GetValue(nameof(Participants), typeof(List<long>));
            if(_participants is null) participants = new List<long>();
            else participants = (List<long>)_participants;

            ChatID = info.GetInt64(nameof(ChatID));

            Title  = info.GetString(nameof(Title)) ?? throw new Exception("Title coudn't be loaded!");

            var KeyUTF8 = info.GetString(nameof(GroupKey)) ?? throw new Exception("Key coudn't be loaded!");
            var IVUTF8  = info.GetString(nameof(GroupIV)) ?? throw new Exception("Key coudn't be loaded!");

            var Key     = Convert.FromBase64String(KeyUTF8);
            var IV      = Convert.FromBase64String(IVUTF8);

            GroupKey = Key;
            GroupIV  = IV;
        }

        /// <summary>
        /// Default construcor
        /// </summary>
        /// <param name="participants">List of particepent IDs (own included)</param>
        /// <param name="chatID">ID of chat</param>
        /// <param name="title">Titel of chat</param>
        public PrivatChat(long chatID, string title, List<long>? participants = null, byte[]? groupkey = null, byte[]? groupIV = null)
        {
            this.participants = participants ?? new();
            ChatID            = chatID;
            Title             = title;
            var aes = Aes.Create();
            GroupKey          = groupkey ?? aes.Key;
            GroupIV           = groupIV  ?? aes.IV;
        }

        /// <summary>
        /// Copy constructror
        /// </summary>
        /// <param name="chat">PrivatChat to copy</param>
        public PrivatChat(PrivatChat chat)
        {
            participants   = chat.participants;
            ChatID         = chat.ChatID;
            Title          = chat.Title;
            GroupKey       = chat.GroupKey;
            GroupIV        = chat.GroupIV;
            cryptedReciver = chat.cryptedReciver;
        }

        /// <summary>
        /// Add a user to the participants list sorted
        /// </summary>
        /// <param name="userID">Id of user to add</param>
        /// <returns>User could be added</returns>
        public bool AddUser(long userID)
        {
            if (participants.Contains(userID)) return false;

            participants.Add(userID);

            participants.Sort();

            return true;
        }

        /// <summary>
        /// Remove a user from the list of participants
        /// </summary>
        /// <param name="userID">ID of user to remove</param>
        /// <returns>User could be removed</returns>
        public bool RemoveUser(long userID)
        {
            return participants.Remove(userID);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Participants), Participants);

            info.AddValue(nameof(ChatID), ChatID);

            info.AddValue(nameof(Title), Title);

            var Key = Convert.ToBase64String(GroupKey);
            var IV  = Convert.ToBase64String(GroupIV);

            info.AddValue(nameof(GroupKey), Key);
            info.AddValue(nameof(GroupIV),  IV);

            base.GetObjectData(info, context);
        }

        public void EncryptData(RSA key, long revicerID)
        {
            if(cryptedReciver is not null)
            {
                return;
            }
            GroupKey = key.Encrypt(GroupKey, RSAEncryptionPadding.Pkcs1);
            GroupIV  = key.Encrypt(GroupIV,  RSAEncryptionPadding.Pkcs1);
            cryptedReciver = revicerID;
        }

        public void DecryptData(RSA key)
        {
            if (cryptedReciver is null)
            {
                return;
            }
            GroupKey = key.Decrypt(GroupKey, RSAEncryptionPadding.Pkcs1);
            GroupIV  = key.Decrypt(GroupIV, RSAEncryptionPadding.Pkcs1);
            cryptedReciver = null;
        }
    }
}
