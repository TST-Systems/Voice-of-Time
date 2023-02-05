using System.Runtime.Serialization;
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
    [Serializable]
    [KnownType(typeof(List<Message>))]
    [KnownType(typeof(PrivatChat))]
    public class TextChat : ISerializable
    {
        private readonly List<Message> messages;

        protected TextChat(SerializationInfo info, StreamingContext context)
        {
            var _messages = info.GetValue(nameof(messages), typeof(List<Message>));
            if (_messages is null) { messages = new List<Message>(); return; }
            messages = (List<Message>)_messages;
        }

        public TextChat()
        {
            messages = new();
        }

        public TextChat(List<Message> messages)
        {
            this.messages = messages;
        }

        public void AddMessage(Message message)
        {
            messages.Add(message);
        }

        public List<Message> GetMessages()
        {
            return messages;
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(messages), messages);
        }
    }
}