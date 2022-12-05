using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoTCore.Communication
{
    public class MultimediaMessage : FileMessage
    {
        public MultimediaMessage(short typeOfMessage, string messageString, long authorID, long dateOfCreation, FileStream file) 
            : base(typeOfMessage, messageString, authorID, dateOfCreation, file)
        {
        }
    }
}
