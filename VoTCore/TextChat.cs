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
        private readonly List<IMessage> messages;
        
        public TextChat()
        {
            messages = new();
        }

        public TextChat(List<IMessage> messages)
        {
            this.messages = messages;
        }

        public void AddMessage(IMessage message)
        {
            messages.Add(message);
        }

        public List<IMessage> GetMessages()
        {
            return messages;
        }
    }
}

