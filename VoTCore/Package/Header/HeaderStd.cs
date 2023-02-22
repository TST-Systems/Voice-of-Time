using System.Text.Json.Serialization;
using VoTCore.Package.Interfaces;

/**
 * @author      - Timeplex
 * 
 * @created     - 13.01.2023
 * 
 * @last_change - 20.01.2023
 */
namespace VoTCore.Package.Header
{
    /// <summary>
    /// Header for CLient->Client communication
    /// </summary>
    public class HeaderStd : IVOTPHeader
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="senderID">ID of sender</param>
        /// <param name="receiverID">ID of reciver</param>
        /// <param name="messageType">Type of message</param>
        public HeaderStd(long senderID, long receiverID, byte messageType)
        {
            SenderID      = senderID;
            ReceiverID    = receiverID;
            MessageType   = messageType;
        }

        [JsonIgnore]
        public short Version      { get; } = 1;

        /// <summary>
        /// ID of sender
        /// </summary>
        public long SenderID      { get; }
        /// <summary>
        /// ID of reciver
        /// </summary>
        public long ReceiverID    { get; }
        /// <summary>
        /// Type of message
        /// </summary>
        public byte MessageType   { get; } // TODO: Make a enum for it

        public override bool Equals(object? obj)
        {
            if (obj is null)                          return false;

            if (obj is not HeaderStd their)           return false;

            if (Version       != their.Version)       return false;
            if (ReceiverID    != their.ReceiverID)    return false;
            if (SenderID      != their.SenderID)      return false;
            if (MessageType   != their.MessageType)   return false;

            return true;
        }

        public override int GetHashCode()
        {
            return (int)(Version * SenderID * ReceiverID * MessageType);
        }
    }
}
