using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoTCore.Communication
{
    public class FileMessage : Message
    {
        public FileStream? File { get; }

        public FileMessage(short typeOfMessage, string messageString, long authorID, long dateOfCreation, FileStream file) 
            : base(typeOfMessage, messageString, authorID, dateOfCreation)
        {
            this.File = file;
        }
    }
}
