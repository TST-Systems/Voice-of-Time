using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoTCore.Package;

namespace VoTCore.Communication
{
    public class FileMessage : Message, IVOTPBody
    {
        public FileStream? File { get; }

        public FileMessage(short typeOfMessage, string messageString, long authorID, long dateOfCreation, FileStream file) 
            : base(typeOfMessage, messageString, authorID, dateOfCreation)
        {
            this.File = file;
        }
        
        public MessageType Type => MessageType.TEXT_MESSAGE | MessageType.MEDIA_MESSAGE;
    }
}
