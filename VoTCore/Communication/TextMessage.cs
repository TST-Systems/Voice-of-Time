using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoTCore.Communication
{
    public class TextMessage : IMessage
    {
        private readonly short  typeOfMessage;
        private readonly string messageString;
        private readonly long   authorID;
        private readonly long   dateOfCreation;

        public TextMessage(short typeOfMessage, string textMessage, long authorID, long dateOfCreation)
        {
            this.typeOfMessage  = typeOfMessage;
            this.messageString  = textMessage;
            this.authorID       = authorID;
            this.dateOfCreation = dateOfCreation;
        }

        public short TypeOfMessage
        {
            get { return typeOfMessage; }
        }

        public string MessageString
        {
            get { return messageString; }
        }


        public long AuthorID
        {
            get { return authorID; }
        }

        public long DateOfCreation
        {
            get { return dateOfCreation; }
        }
    }
}
