using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoTCore.Package;

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
        
        public MessageStatus Status{ get; }

        protected Message(short typeOfMessage, string messageString, long authorID, long dateOfCreation)
        {
            this.TypeOfMessage  = typeOfMessage;
            this.MessageString  = messageString;
            this.AuthorID       = authorID;
            this.DateOfCreation = dateOfCreation;
            Status = new();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj is not Message their) return false;

            if (this.TypeOfMessage  != their.TypeOfMessage)  return false;
            if (this.MessageString  != their.MessageString)  return false;
            if (this.AuthorID       != their.AuthorID)       return false;
            if (this.DateOfCreation != their.DateOfCreation) return false;

            return true;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
