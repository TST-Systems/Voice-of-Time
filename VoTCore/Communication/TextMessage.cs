using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoTCore.Package;

namespace VoTCore.Communication
{
    /// <summary>
    /// Basic text message
    /// </summary>
    public class TextMessage : Message, IVOTPBody
    {
        public TextMessage(short typeOfMessage, string messageString, long authorID, long dateOfCreation) 
            : base(typeOfMessage, messageString, authorID, dateOfCreation)
        {
        }

        public MessageType Type => MessageType.TEXT_MESSAGE;

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }
    }
}
