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
        public byte[] GroupKey { get; }

        [JsonIgnore]
        public BodyType Type => BodyType.PRIVAT_CHAT;

        private long? cryptedReciver;
        public long CryptedReciver { get { if (cryptedReciver is null) return -1; return (long)cryptedReciver; } }

        [JsonConstructor]
        public PrivatChat(List<long> participants, long chatID, string title, long cryptedReciver, byte[] groupkey) 
        {
            this.participants   = participants;
            ChatID              = chatID;
            Title               = title;
            GroupKey            = groupkey;
            this.cryptedReciver = cryptedReciver;
        }

        protected PrivatChat(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            var _participants = info.GetValue(nameof(Participants), typeof(List<long>));
            if(_participants is null) participants = new List<long>();
            else participants = (List<long>)_participants;

            ChatID = info.GetInt64(nameof(ChatID));

            Title  = info.GetString(nameof(Title)) ?? throw new Exception("Title coudn't be loaded!");

            var KeyUTF8 = info.GetString(nameof(GroupKey)) ?? throw new Exception("Key coudn't be loaded!");

            var Key     = Convert.FromBase64String(KeyUTF8);

            GroupKey = Key;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="participants"></param>
        /// <param name="chatID"></param>
        /// <param name="title"></param>
        public PrivatChat(long chatID, string title, List<long>? participants = null, byte[]? groupkey = null)
        {
            this.participants = participants ?? new();
            ChatID            = chatID;
            Title             = title;
            GroupKey          = groupkey ?? Aes.Create().Key;
        }

        public bool AddUser(long userID)
        {
            if (participants.Contains(userID)) return false;

            participants.Add(userID);

            participants.Sort();

            return true;
        }

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

            info.AddValue(nameof(GroupKey), Key);

            base.GetObjectData(info, context);
        }

        public void EncryptData(RSA key, long revicerID)
        {
            throw new NotImplementedException();
        }

        public void DecryptData(RSA key)
        {
            throw new NotImplementedException();
        }
    }
}
