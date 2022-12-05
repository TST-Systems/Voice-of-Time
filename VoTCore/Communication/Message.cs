using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoTCore.Communication
{
    public abstract class Message
    {
        public short  TypeOfMessage  { get; }
        public string MessageString  { get; }
        public long   AuthorID       { get; }
        public long   DateOfCreation { get; }
        
        protected Message(short typeOfMessage, string messageString, long authorID, long dateOfCreation)
        {
            this.TypeOfMessage  = typeOfMessage;
            this.MessageString  = messageString;
            this.AuthorID       = authorID;
            this.DateOfCreation = dateOfCreation;
        }
    }
}
