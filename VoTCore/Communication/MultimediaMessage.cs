using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoTCore.Package;

namespace VoTCore.Communication
{
    /// <summary>
    /// Extended FileMessage for Images and outher showable files
    /// </summary>
    public class MultimediaMessage : FileMessage
    {
        public MultimediaMessage(short typeOfMessage, string messageString, long authorID, long dateOfCreation, FileStream file) 
            : base(typeOfMessage, messageString, authorID, dateOfCreation, file)
        {
        }
        
    }
}
