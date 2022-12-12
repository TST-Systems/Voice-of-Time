using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoTCore.Communication
{
    /// <summary>
    /// Text based message for communication between clients
    /// </summary>
    public abstract class Message
    {
        /// <summary>
        /// Outdated?
        /// </summary>
        public short  TypeOfMessage  { get; }
        /// <summary>
        /// Text part of Message
        /// Always present even if message is an image
        /// </summary>
        public string MessageString  { get; }
        /// <summary>
        /// User ID of Author
        /// </summary>
        public long   AuthorID       { get; }
        /// <summary>
        /// Time of creation in milliseconds
        /// </summary>
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
