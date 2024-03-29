﻿using System.Runtime.Serialization;
using VoTCore.Package.Interfaces;

/**
 * @author      - Timeplex
 * 
 * @created     - 23.01.2023
 * 
 * @last_change - 05.02.2023
 */
namespace VoTCore.Communication.Data
{
    /// <summary>
    /// Message with a file attached
    /// </summary>
    [Serializable]
    public class FileMessage : Message, IVOTPBody, ISerializable
    {
        /// <summary>
        /// File to send
        /// </summary>
        public string? File { get; } // TODO: 

        const BodyType TYPE = BodyType.MESSAGE_FILE;

        /// <summary>
        /// Constructor for DataContract
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public FileMessage(SerializationInfo info, StreamingContext context) : base(info, context) 
        { 
            File = info.GetString(nameof(File));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="messageString">Text message as string</param>
        /// <param name="authorID">ID of the author</param>
        /// <param name="dateOfCreation">Date and Time of creation</param>
        /// <param name="file">File</param>
        public FileMessage(string messageString, long authorID, DateTime dateOfCreation, string file)
            : base(messageString, authorID, dateOfCreation, TYPE)
        {
            File = file;
        }

        public override bool Equals(object? obj)
        {
            if (!base.Equals(obj)) return false;

            if (obj is not FileMessage their) return false;

            if (File == null && their.File == null) return true;
            if (File == null) return false;


            if (File.Equals(their.File)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(File), File);
            base.GetObjectData(info, context);
        }
    }
}
