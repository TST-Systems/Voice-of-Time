﻿using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using VoTCore.User;

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
        public long DateOfCreation { get; }

        public MessageStatus Status { get; }

        protected Message(SerializationInfo info, StreamingContext context)
        {
            Type           = (BodyType)(info.GetValue(nameof(Type), typeof(BodyType)) 
                ?? throw new Exception("Message coudn't be laoded!"));

            MessageString  = info.GetString(nameof(MessageString)) 
                ?? throw new Exception("Message coudn't be laoded!");

            AuthorID       = info.GetInt64(nameof(AuthorID));

            DateOfCreation = info.GetInt64(nameof(DateOfCreation));

            Status         = (MessageStatus)(info.GetValue(nameof(Status), typeof(MessageStatus)) 
                ?? throw new Exception("Message coudn't be laoded!"));
        }

        protected Message(string messageString, long authorID, long dateOfCreation, BodyType type)
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
