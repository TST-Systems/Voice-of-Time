using VoTCore.Package.Interfaces;

namespace VoTCore.Communication
{
    /// <summary>
    /// Message with a file attached
    /// </summary>
    public class FileMessage : Message, IVOTPBody
    {

        public FileStream? File { get; }

        const MessageType TYPE = MessageType.TEXT_MESSAGE | MessageType.MEDIA_MESSAGE;

        public FileMessage(string messageString, long authorID, long dateOfCreation, FileStream file) 
            : base(messageString, authorID, dateOfCreation, TYPE)
        {
            this.File = file;
        }

        public override bool Equals(object? obj)
        {
            if (!base.Equals(obj)) return false;

            if (obj is not FileMessage their) return false;

            if(this.File == null && their.File == null) return true;
            if(this.File == null) return false;


            if (this.File.Equals(their.File)) return false;

            return true;
        }
    }
}
