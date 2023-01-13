using System.Text.Json.Serialization;

namespace VoTCore.Communication
{
    /// <summary>
    /// Text based message for communication between clients
    /// </summary>
    public abstract class Message
    {
        /// <summary>
        /// Type of Message
        /// Used for de-/serialasation
        /// </summary>
        [JsonIgnore]
        public MessageType Type { get; }
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
        
        protected Message(string messageString, long authorID, long dateOfCreation, MessageType type)
        {
            this.MessageString  = messageString;
            this.AuthorID       = authorID;
            this.DateOfCreation = dateOfCreation;
            this.Type           = type;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj is not Message their) return false;

            if (this.MessageString  != their.MessageString)  return false;
            if (this.AuthorID       != their.AuthorID)       return false;
            if (this.DateOfCreation != their.DateOfCreation) return false;

            return true;
        }
    }
}
