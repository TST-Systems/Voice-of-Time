using System.Text.Json.Serialization;
using VoTCore.Package.Interfaces;

namespace VoTCore.Package.Header
{
    public class HeaderStd : IVOTPHeader
    {
        public HeaderStd(long senderID, long receiverID, byte messageType, byte encrypionType)
        {
            SenderID      = senderID;
            ReceiverID    = receiverID;
            MessageType   = messageType;
            EncrypionType = encrypionType;
        }

        [JsonIgnore]
        public short Version      { get; } = 1;

        public long SenderID      { get; }

        public long ReceiverID    { get; }

        public byte MessageType   { get; }

        public byte EncrypionType { get; }

        public override bool Equals(object? obj)
        {
            if (obj is null)                          return false;

            if (obj is not HeaderStd their)           return false;

            if (Version       != their.Version)       return false;
            if (ReceiverID    != their.ReceiverID)    return false;
            if (SenderID      != their.SenderID)      return false;
            if (MessageType   != their.MessageType)   return false;
            if (EncrypionType != their.EncrypionType) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return (int)(Version * SenderID * ReceiverID * MessageType * EncrypionType);
        }
    }
}
