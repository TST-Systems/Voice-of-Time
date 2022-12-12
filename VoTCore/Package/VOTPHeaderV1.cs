using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoTCore.Package
{
    public class VOTPHeaderV1 : IVOTPHeader
    {
        public VOTPHeaderV1(long senderID, long receiverID, byte messageType, byte encrypionType)
        {
            this.SenderID      = senderID;
            this.ReceiverID    = receiverID;
            this.MessageType   = messageType;
            this.EncrypionType = encrypionType;            
        }

        public short Version { get; } = 1;

        public long SenderID { get; }

        public long ReceiverID { get; }

        public byte MessageType { get; }

        public byte EncrypionType { get; }

        public override bool Equals(object? obj)
        {
            if(obj == null) return false;

            if (obj is not VOTPHeaderV1 their) return false;

            if (this.Version       != their.Version)       return false;
            if (this.ReceiverID    != their.ReceiverID)    return false;
            if (this.SenderID      != their.SenderID)      return false;
            if (this.MessageType   != their.MessageType)   return false;
            if (this.EncrypionType != their.EncrypionType) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return (int)(Version * SenderID * ReceiverID * MessageType * EncrypionType);
        }
    }
}
