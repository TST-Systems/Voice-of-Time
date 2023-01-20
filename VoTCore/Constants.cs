using VoTCore.Communication;
using VoTCore.Package;

namespace VoTCore
{
    public static class Constants
    {
        public readonly static Dictionary<Int16, Type> HeaderTypes = new()
        {
            { 1, typeof(VOTPHeaderV1) },
        };

        public readonly static Dictionary<MessageType, Type> BodyTypes = new()
        {
            { MessageType.TEXT_MESSAGE,                             typeof(TextMessage) },
            { MessageType.TEXT_MESSAGE | MessageType.MEDIA_MESSAGE, typeof(FileMessage) },
        };

        // Transmission buffer size
        public const int BUFFER_SIZE_BYTE = 1024;

        // Transmission symbols
        public const char SOM = (char) 2;   // ASCI: STX
        public const char EOM = (char) 3;   // ASCI: ETX
        public const char FIN = (char) 4;   // ASCI: EOT
        public const char ACK = (char) 6;   // ASCI: ACK


    }

    public enum MessageType
    {
        NONE = 0,
        TEXT_MESSAGE = 1 << 0,
        MEDIA_MESSAGE = 1 << 1,

    }


}
