using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoTCore.Package;

namespace VoTCore.Communication
{
    /// <summary>
    /// Message with a file attached
    /// </summary>
    public class MediaMessage : Message, IVOTPBody
    {

        public FileStream? File { get; }

        public MediaMessage(short typeOfMessage, string messageString, long authorID, long dateOfCreation, FileStream file) 
            : base(typeOfMessage, messageString, authorID, dateOfCreation)
        {
            this.File = file;
        }
        
        public MessageType Type => MessageType.TEXT_MESSAGE | MessageType.MEDIA_MESSAGE;
    }
}
