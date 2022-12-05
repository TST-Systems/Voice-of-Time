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
    }
}
