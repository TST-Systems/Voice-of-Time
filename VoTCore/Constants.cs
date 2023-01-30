using VoTCore.Communication.Data;
using VoTCore.Package.Header;
using VoTCore.Package.SData;
using VoTCore.Package.SecData;

namespace VoTCore
{
    public static class Constants
    {
        public readonly static Dictionary<Int16, Type> HeaderTypes = new()
        {
            { 1, typeof(HeaderStd) },
            { 2, typeof(HeaderReq) },
            { 3, typeof(HeaderAck) },
        };

        public readonly static Dictionary<BodyType, Type> BodyTypes = new()
        {
            { BodyType.MESSAGE_TEXT,    typeof(TextMessage)     },
            { BodyType.MESSAGE_FILE,    typeof(FileMessage)     },
            { BodyType.SDATA_INT,       typeof(SData_Int)       },
            { BodyType.SDATA_LONG,      typeof(SData_Long)      },
            { BodyType.SDATA_GUID,      typeof(SData_Guid)      },
            { BodyType.SDATA_STRING,    typeof(SData_String)    },
            { BodyType.SECDATA_KEY_RSA, typeof(SecData_Key_RSA) },
            { BodyType.SECDATA_KEY_AES, typeof(SecData_Key_Aes) },
        };

        // Transmission buffer size
        public const int BUFFER_SIZE_BYTE = 1024;

        // Transmission symbols
        public const string SOM = "\u0002\u0002\u0002";   // ASCI: STX
        public const string EOM = "\u0003\u0003\u0003";   // ASCI: ETX
        public const string FIN = "\u0004";   // ASCI: EOT
        public const string ACK = "\u0005";   // ASCI: ACK


    }

    /// <summary>
    /// All possible Types of Bodys
    /// </summary>
    public enum BodyType : ushort
    {
        // Messages (Client >-> Client)
        MESSAGE      = 0x00,
        // 0x01 - 0x1f
        MESSAGE_TEXT = 0x01,
        MESSAGE_FILE = 0x02,

        // Reserved
        // 0x21 - 0x3f

        // Single Data
        SDATA        = 0x40,
        // 0x41 - 0x5f
        SDATA_INT    = 0x41,
        SDATA_LONG   = 0x42,
        SDATA_DOUBLE = 0x43,
        SDATA_STRING = 0x44,
        SDATA_GUID   = 0x50,

        // Reserved
        // 0x61 - 0x7f

        // Reserved
        // 0x81 - 0x9f

        // Security Data
        SECDATA         = 0xa0,
        // 0xa1 - 0xbf
        SECDATA_KEY_RSA = 0xa1, 
        SECDATA_KEY_AES = 0x1b,

        // Reserved
        // 0xc1 - 0xdf

        // Reserved
        // 0xe1 - 0xfe

        NONE = 0xff,

        // User space
        // 0x01_01 - 0xff_ff
    }

    public enum RequestType
    {
        IDENTITY,
        KEY,
        KEY_EXCHANGE,
        REGISTRATION,
        COMM_KEY,
        SET_USERNAME,
        VERIFY,
    }


}
