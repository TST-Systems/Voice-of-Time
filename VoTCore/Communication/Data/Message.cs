using System.Runtime.Serialization;
using System.Text.Json.Serialization;

/**
 * @author      - Timeplex, SalzstangeManga
 * 
 * @created     - 23.01.2023
 * 
 * @last_change - 05.02.2023
 */
namespace VoTCore.Communication.Data
{
    /// <summary>
    /// Text based message for communication between clients
    /// </summary>
    [Serializable]
    [KnownType(typeof(BodyType))]
    [KnownType(typeof(DateTime))]
    [KnownType(typeof(MessageStatus))]
    [KnownType(typeof(FileMessage))]
    [KnownType(typeof(TextMessage))]
    public abstract class Message : ISerializable
    {
        /// <summary>
        /// Type of Message
        /// Used for de-/serialasation
        /// </summary>
        [JsonIgnore]
        public BodyType Type { get; }
        /// <summary>
        /// Text part of Message
        /// Always present even if message is an image
        /// </summary>
        public string MessageString { get; }
        /// <summary>
        /// User ID of Author
        /// </summary>
        public long AuthorID { get; }
        /// <summary>
        /// Time of creation in milliseconds
        /// </summary>
        public DateTime DateOfCreation { get; }
        /// <summary>
        /// Message status
        /// </summary>
        public MessageStatus Status { get; }

        /// <summary>
        /// Constructor for DataContract
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// <exception cref="Exception"></exception>
        protected Message(SerializationInfo info, StreamingContext context)
        {
            Type           = (BodyType)(info.GetValue(nameof(Type), typeof(BodyType)) 
                ?? throw new Exception("Message coudn't be laoded!"));

            MessageString  = info.GetString(nameof(MessageString)) 
                ?? throw new Exception("Message coudn't be laoded!");

            AuthorID       = info.GetInt64(nameof(AuthorID));

            DateOfCreation = info.GetDateTime(nameof(DateOfCreation));

            Status         = (MessageStatus)(info.GetValue(nameof(Status), typeof(MessageStatus)) 
                ?? throw new Exception("Message coudn't be laoded!"));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="messageString">Text message</param>
        /// <param name="authorID">ID of author</param>
        /// <param name="dateOfCreation">Date and Time of creation</param>
        /// <param name="type">Type of Message</param>
        protected Message(string messageString, long authorID, DateTime dateOfCreation, BodyType type)
        {
            MessageString  = messageString;
            AuthorID       = authorID;
            DateOfCreation = dateOfCreation;
            Type           = type;
            Status         = new();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj is not Message their) return false;

            if (MessageString != their.MessageString) return false;
            if (AuthorID != their.AuthorID) return false;
            if (DateOfCreation != their.DateOfCreation) return false;

            return true;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(MessageString),  MessageString);
            info.AddValue(nameof(AuthorID),       AuthorID);
            info.AddValue(nameof(DateOfCreation), DateOfCreation);
            info.AddValue(nameof(Status),         Status);
            info.AddValue(nameof(Type),           Type);
        }
    }
}
