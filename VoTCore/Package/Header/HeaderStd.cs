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
    public class HeaderStd : IVOTPHeader
    {
        public HeaderStd(long senderID, long receiverID, byte messageType)
        {
            SenderID      = senderID;
            ReceiverID    = receiverID;
            MessageType   = messageType;
        }

        [JsonIgnore]
        public short Version      { get; } = 1;

        public long SenderID      { get; }

        public long ReceiverID    { get; }

        public byte MessageType   { get; }

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
