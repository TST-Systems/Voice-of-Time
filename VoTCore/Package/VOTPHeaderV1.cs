using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoTCore.Package
{
    public class VOTPHeaderV1 : IVOTPHeader
    {
        private readonly short version = 1;
        private readonly long  senderID;
        private readonly long  receiverID;
        private readonly byte  encrypionType;
        private readonly byte  messageType;

        public VOTPHeaderV1(long senderID, long receiverID, byte messageType, byte encrypionType)
        {
            this.senderID      = senderID;
            this.receiverID    = receiverID;
            this.messageType   = messageType;
            this.encrypionType = encrypionType;            
        }

        public short Version
        {
            get { return version; }
        }

        public long SenderID
        {
            get { return senderID; }
        }

        public long ReceiverID
        {
            get { return receiverID; }
        }

        public byte MessageType
        {
            get { return messageType; }
        }

        public byte EncrypionType
        {
            get { return encrypionType; }
        }
    }
}
