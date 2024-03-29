﻿using VoTCore.Communication;
using VoTCore.Communication.Data;
using VoTCore.Package.AbsData;
using VoTCore.Package.AData;
using VoTCore.Package.Header;
using VoTCore.Package.SData;
using VoTCore.Package.SecData;
using VoTCore.Package.StashData;
using VoTCore.User;
/**
 * @author      - Timeplex, SalzstangeManga
 * 
 * @created     - 12.12.2022
 * 
 * @last_change - 17.02.2023
 */
namespace VoTCore
{
    /// <summary>
    /// Summary of all constants used thought all parts (Client/Server)
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Dict of all Headers
        /// </summary>
        public readonly static Dictionary<Int16, Type> HeaderTypes = new()
        {
            { 1, typeof(HeaderStd) },
            { 2, typeof(HeaderReq) },
            { 3, typeof(HeaderAck) },
        };

        /// <summary>
        /// Dict of all Bodys
        /// </summary>
        public readonly static Dictionary<BodyType, Type> BodyTypes = new()
        {
            { BodyType.MESSAGE_TEXT,            typeof(TextMessage)             },
            { BodyType.MESSAGE_FILE,            typeof(FileMessage)             },

            { BodyType.STASHDATA,               typeof(StashData)               },
            { BodyType.STASHDATA_ADD,           typeof(StashData_Add)           },

            { BodyType.SDATA_INT,               typeof(SData_Int)               },
            { BodyType.SDATA_LONG,              typeof(SData_Long)              },
            { BodyType.SDATA_GUID,              typeof(SData_Guid)              },
            { BodyType.SDATA_STRING,            typeof(SData_String)            },
            { BodyType.SDATA_INTERNALEXCEPTION, typeof(SData_InternalException) },

            { BodyType.ADATA_LONG,              typeof(AData_Long)              },

            { BodyType.ABSDATA_INVITE,          typeof(AbsData_Invite)          },
            { BodyType.ABSDATA_INVITE_ACCEPT,   typeof(AbsData_InviteAccept)    },
            { BodyType.ABSDATA_RECEIPT,         typeof(AbsData_Receipt)         },

            { BodyType.SECDATA_KEY_RSA,         typeof(SecData_Key_RSA)         },
            { BodyType.SECDATA_KEY_AES,         typeof(SecData_Key_Aes)         },

            { BodyType.PUBLIC_CLIENT,           typeof(PublicClient)            },
            { BodyType.PRIVAT_CHAT,             typeof(PrivatChat)              },
        };

        // Transmission buffer size
        public const int BUFFER_SIZE_BYTE = 1024;

        // Transmission symbols
        public const string SOM = "\u0002\u0002\u0002";   // ASCI: STX
        public const string EOM = "\u0003\u0003\u0003";   // ASCI: ETX
        public const string FIN = "\u0004";               // ASCI: EOT
        public const string ACK = "\u0005";               // ASCI: ACK // Unused

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

        // Stash Data
        STASHDATA     = 0x20,
        // 0x21 - 0x3f
        STASHDATA_ADD = 0x21,

        // Single Data
        SDATA           = 0x40,
        // 0x41 - 0x5f
        SDATA_INT       = 0x41,
        SDATA_LONG      = 0x42,
        SDATA_DOUBLE    = 0x43,
        SDATA_STRING    = 0x44,
        SDATA_GUID      = 0x50,
        SDATA_INTERNALEXCEPTION = 0x51,

        // Array Data
        ADATA       = 0x60,
        // 0x61 - 0x7f
        ADATA_LONG  = 0x62,

        // Abstract Data
        ABSDATA               = 0x80,
        // 0x81 - 0x9f
        ABSDATA_INVITE        = 0x81,
        ABSDATA_RECEIPT       = 0x82,
        ABSDATA_INVITE_ACCEPT = 0x83,

        // Secure Data
        SECDATA         = 0xa0,
        // 0xa1 - 0xbf
        SECDATA_KEY_RSA = 0xa1, 
        SECDATA_KEY_AES = 0x1b,
      //SECDATA_PUBLIC_CLIENT_SHARE = 0x1c,

        // ETC
        // 0xc1 - 0xdf
        PRIVAT_CHAT   = 0xc1,
        PUBLIC_CLIENT = 0xc2,


        // Reserved
        // 0xe1 - 0xfe

        NONE = 0xff,

        // User space
        // 0x01_01 - 0xff_ff
    }

    /// <summary>
    /// All types of request send to server
    /// </summary>
    public enum RequestType
    {
        // Server requests
        SERVER_GET_IDENTITY,
        SERVER_PUBLIC_KEY_EXCHANGE,
        // User requests
        USER_REGISTRATION,
        USER_VERIFY,
        USER_SET_USERNAME,
        // Communication Requests
        COMMUNICATION_GET_KEY_AND_SECURE,
        // Public User requests
        PUBLIC_USER_GET,
        PUBLIC_USER_GET_ID_LIST,
        // Privat chat requests
        PRIVAT_CHAT_REGISTER,
        PRIVAT_CHAT_INVITE_USER,
        PRIVAT_CHAT_INVITE_ACCEPT,
        // Stash requests
        STASH_ADD,
        STASH_GET, 
        STASH_LIST,
        STASH_DELETE
    }

    /// <summary>
    /// All data handling instructions
    /// </summary>
    public enum DataHandling : byte
    {
        NONE,
        REMOVE_AFTER_GET,
        REMOVE_AFTER_GET_ACK,
        REMOVE_AFTER_GET_READ,
    }

    /// <summary>
    /// Types of status that a receipt can have on the client side
    /// </summary>
    public enum ReceiptStatus : byte
    {
        TO_REQUEST,
        REC_AND_ACC,
        SEND,
        IGNORE,
    }

    /// <summary>
    /// All codes of "soft" errors that can orruce
    /// </summary>
    public enum InternalExceptionCode
    {
        UNKNOWN,
        WRONG_BODY_TYPE,
        SOURCE_UNEQUAL_USER,
        USER_DOES_NOT_EXISTS,
        CHAT_DOES_NOT_EXISTS,
        CHAT_NOT_INVITED,
        CHAT_NOT_MEMBER,
        CHAT_ALREADY_MEMBER,
        CHAT_NO_PERMISSIONS,
        COMMUNICATION_ALREADY_SECURE,
        COMMUNICATION_NO_PUBLIC_KEY,
        USER_INVALID,
        ID_DOES_NOT_EXISTS,
        STASH_NO_MESSAGE_UNDER_ID,
        STASH_NO_PERMISSIONS,
        COMMUNICATION_ALREADY_VERIFIED,
        COMMUNICATION_NOT_SECURE,
        UNKNOWN_HEADER_TYPE,
        UNKNOWN_REQUEST_TYPE,
        WRONG_SENDER,
        COMMUNICATION_NOT_VERIFIED,
    }
}
