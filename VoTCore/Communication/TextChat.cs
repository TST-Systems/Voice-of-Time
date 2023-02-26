using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using VoTCore.Communication.Data;

/**
 * @author      - Timeplex
 * 
 * @created     - 23.01.2023
 * 
 * @last_change - 05.02.2023
 */
namespace VoTCore.Communication
{
    /// <summary>
    /// Base class for all Textbased chats
    /// </summary>
    [Serializable]
    [KnownType(typeof(List<Message>))]
    [KnownType(typeof(PrivatChat))]
    public class TextChat : ISerializable
    {
        /// <summary>
        /// List of all messages
        /// </summary>
        [JsonIgnore]
        private readonly List<Message> messages;

        /// <summary>
        /// Constructor for DataContract
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected TextChat(SerializationInfo info, StreamingContext context)
        {
            var _messages = info.GetValue(nameof(messages), typeof(List<Message>));
            if (_messages is null) { messages = new List<Message>(); return; }
            messages = (List<Message>)_messages;
        }

        /// <summary>
        /// Constructor for a empty chat
        /// </summary>
        public TextChat()
        {
            messages = new();
        }

        /// <summary>
        /// Constructor for loading in messages
        /// </summary>
        /// <param name="messages">List of messages</param>
        public TextChat(List<Message> messages)
        {
            this.messages = messages;
        }

        /// <summary>
        /// Add a message to the chat + automaticly sorts it in by Date
        /// </summary>
        /// <param name="message">Mesage to add</param>
        public void AddMessage(Message message)
        {
            messages.Add(message);
            Sort();
        }

        /// <summary>
        /// Get a list of all Messages sorted
        /// </summary>
        /// <returns></returns>
        public List<Message> GetMessages()
        {
            return messages;
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(messages), messages);
        }

        /// <summary>
        /// Sort the messages by Date
        /// </summary>
        public void Sort()
        {
            messages.Sort((e1, e2) => (int)(e1.DateOfCreation - e2.DateOfCreation).Ticks); //TODO: Better method
        }
    }
}