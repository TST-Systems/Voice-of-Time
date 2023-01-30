using System.Security.Cryptography;
using VoTCore.Communication;

/**
 * @author      - Timeplex
 * 
 * @created     - 30.01.2023
 * 
 * @last_change - 30.01.2023
 */
namespace Voice_of_Time.Communication
{
    internal class PrivatChat : TextChat
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
    }
}
