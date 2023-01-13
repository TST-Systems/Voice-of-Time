using VoTCore.Communication;
using VoTCore.Package.Header;

namespace VoTCore
{
    public static class Constants
    {
        public readonly static Dictionary<Int16, Type> HeaderTypes = new()
        {
            { 1, typeof(HeaderStd) },
            { 2, typeof(HeaderReq) },
        };

        public readonly static Dictionary<BodyType, Type> BodyTypes   = new()
        {
            { BodyType.TEXT_MESSAGE, typeof(TextMessage) },
            { BodyType.FILE_MESSAGE, typeof(FileMessage) },
        };

        // Transmission buffer size
        public const int BUFFER_SIZE_BYTE = 1024;

        // Transmission symbols
        public const char EOM = (char) 3; // ASCI: ETX
        public const char FIN = (char) 4; // ASCI: EOT
        public const char ACK = (char) 6; // ASCI: ACK

    }

    /// <summary>
    /// All possible Types of Bodys
    /// </summary>
    public enum BodyType : ushort
    {
        // Messages (Client >-> Client)
        MESSAGE      = 0x00,
        // 0x01 - 0x1f
        TEXT_MESSAGE = 0x01,
        FILE_MESSAGE = 0x02,

        // Reserved
        // 0x21 - 0x3f

        // Simple Data
        SDATA        = 0x40,
        // 0x41 - 0x5f
        INT_SDATA    = 0x41,
        STRING_SDATA = 0x42,
        DOUBLE_SDATA = 0x43,

        // Reserved
        // 0x61 - 0x7f

        // Reserved
        // 0x81 - 0x9f

        // Reserved
        // 0xa1 - 0xbf

        // Reserved
        // 0xc1 - 0xdf

        // Reserved
        // 0xe1 - 0xfe

        NONE = 0xff

        // User space
        // 0x01_01 - 0xff_ff
    }

    public enum RequestType
    {
        IDENTITY,
    }
}
