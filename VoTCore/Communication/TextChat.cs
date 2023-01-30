using VoTCore.Communication.Data;

/**
 * @author      - Timeplex
 * 
 * @created     - 23.01.2023
 * 
 * @last_change - 23.01.2023
 */
namespace VoTCore.Communication
{
    public class TextChat
    {
        private readonly List<Message> messages;

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
    }
}