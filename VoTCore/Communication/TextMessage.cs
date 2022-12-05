using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoTCore.Communication
{
    public class TextMessage : Message
    {
        public TextMessage(short typeOfMessage, string messageString, long authorID, long dateOfCreation) 
            : base(typeOfMessage, messageString, authorID, dateOfCreation)
        {
        }        
    }
}
