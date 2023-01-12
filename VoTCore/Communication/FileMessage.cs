using VoTCore.Package;

namespace VoTCore.Communication
{
    /// <summary>
    /// Message with a file attached
    /// </summary>
    public class FileMessage : Message, IVOTPBody
    {

        public FileStream? File { get; }

        public FileMessage(short typeOfMessage, string messageString, long authorID, long dateOfCreation, FileStream file) 
            : base(typeOfMessage, messageString, authorID, dateOfCreation)
        {
            this.File = file;
        }
        
        public MessageType Type => MessageType.TEXT_MESSAGE | MessageType.MEDIA_MESSAGE;

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
