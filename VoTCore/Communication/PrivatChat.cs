using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using VoTCore.Package.Interfaces;

/**
 * @author      - Timeplex
 * 
 * @created     - 30.01.2023
 * 
 * @last_change - 06.02.2023
 */
namespace VoTCore.Communication
{
    [Serializable]
    [KnownType(typeof(List<long>))]
    public class PrivatChat : TextChat, ISerializable, IVOTPBody
    {
        /// <summary>
        /// A List of all Participants, including self. 
        /// Can be used to determind the reciver of a message for a speific chat
        /// </summary>
        public List<long> Participants { get => new(participants); }
        [JsonIgnore]
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
        public Aes GroupKey { get; }
        [JsonIgnore]
        public bool GroupKeyIsSet = false;

        [JsonIgnore]
        public BodyType Type => BodyType.PRIVAT_CHAT;

        [JsonConstructor]
        protected PrivatChat(List<long> Participants, long chatID, string title) 
        {
            participants = Participants;
            ChatID = chatID;
            Title = title;
            GroupKey = Aes.Create();
        }

        protected PrivatChat(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            var _participants = info.GetValue(nameof(Participants), typeof(List<long>));
            if(_participants is null) participants = new List<long>();
            else participants = (List<long>)_participants;

            ChatID = info.GetInt64(nameof(ChatID));

            Title  = info.GetString(nameof(Title)) ?? throw new Exception("Title coudn't be loaded!");

            var KeyUTF8 = info.GetString(nameof(GroupKey.Key)) ?? throw new Exception("Key coudn't be loaded!");
            var IVUTF8  = info.GetString(nameof(GroupKey.IV))  ?? throw new Exception("IV coudn't be loaded!"); ;

            var Key     = Convert.FromBase64String(KeyUTF8);
            var IV      = Convert.FromBase64String(IVUTF8);

            GroupKey = Aes.Create();
            GroupKey.Key = Key;
            GroupKey.IV = IV;

            GroupKeyIsSet = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="participants"></param>
        /// <param name="chatID"></param>
        /// <param name="title"></param>
        public PrivatChat(Aes? groupkey, List<long>? participants, long chatID, string title)
        {
            this.participants = participants ?? new();
            ChatID            = chatID;
            Title             = title;
            GroupKey          = groupkey ?? Aes.Create();

            GroupKeyIsSet = true;
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

            var Key = Convert.ToBase64String(GroupKey.Key);
            var IV  = Convert.ToBase64String(GroupKey.IV);

            info.AddValue(nameof(GroupKey.Key), Key);
            info.AddValue(nameof(GroupKey.IV),  IV);

            base.GetObjectData(info, context);
        }
    }
}
