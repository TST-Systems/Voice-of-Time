using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoTCore.Communication;

namespace VoTCore
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

