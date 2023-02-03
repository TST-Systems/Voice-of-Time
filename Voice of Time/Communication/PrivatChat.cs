using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using VoTCore.Communication;

/**
 * @author      - Timeplex
 * 
 * @created     - 30.01.2023
 * 
 * @last_change - 03.02.2023
 */
namespace Voice_of_Time.Communication
{
    [Serializable]
    internal class PrivatChat : TextChat, ISerializable
    {
        /// <summary>
        /// A List of all Participants, including self. 
        /// Can be used to determind the reciver of a message for a speific chat
        /// </summary>
        internal List<long> Participants { get => new(participants); }
        private readonly List<long> participants;
        /// <summary>      
        /// ID under wich the server can recognize this chat
        /// </summary>
        internal long ChatID { get; }
        /// <summary>
        /// Name displayed in GUI or CMD as chat name/title
        /// </summary>
        internal string Title { get; set; }
        /// <summary>
        /// Communication Key for messages
        /// </summary>
        internal Aes GroupKey { get; }

        protected PrivatChat(SerializationInfo info, StreamingContext context) : base(info, context) 
        {
            var _participants = info.GetValue(nameof(Participants), typeof(List<long>));
            if(_participants is null) participants = new List<long>();
            else participants = (List<long>)_participants;

            ChatID = info.GetInt64(nameof(ChatID));

            Title  = info.GetString(nameof(Title)) ?? throw new Exception("Title coudn't be loaded!");

            var KeyUTF8 = info.GetString(nameof(GroupKey.Key)) ?? throw new Exception("Key coudn't be loaded!");
            var IVUTF8  = info.GetString(nameof(GroupKey.IV))  ?? throw new Exception("IV coudn't be loaded!"); ;

            var Key     = Encoding.UTF8.GetBytes(KeyUTF8);
            var IV      = Encoding.UTF8.GetBytes(IVUTF8);

            GroupKey = Aes.Create();
            GroupKey.Key = Key;
            GroupKey.IV = IV;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="participants"></param>
        /// <param name="chatID"></param>
        /// <param name="title"></param>
        internal PrivatChat(Aes? groupkey, List<long>? participants, long chatID, string title)
        {
            this.participants = participants ?? new();
            ChatID            = chatID;
            Title             = title;
            GroupKey          = groupkey ?? Aes.Create();
        }

        internal bool AddUser(long userID)
        {
            if (participants.Contains(userID)) return false;

            participants.Add(userID);

            participants.Sort();

            return true;
        }

        internal bool RemoveUser(long userID)
        {
            return participants.Remove(userID);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(Participants), Participants);

            info.AddValue(nameof(ChatID), ChatID);

            info.AddValue(nameof(Title), Title);

            var Key = Encoding.UTF8.GetString(GroupKey.Key);
            var IV  = Encoding.UTF8.GetString(GroupKey.IV);

            info.AddValue(nameof(GroupKey.Key), Key);
            info.AddValue(nameof(GroupKey.IV),  IV);
        }
    }
}
