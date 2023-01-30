using VoTCore.Package.Interfaces;

namespace VoTCore.Communication.Data
{
    /// <summary>
    /// Message with a file attached
    /// </summary>
    public class FileMessage : Message, IVOTPBody
    {

        public FileStream? File { get; }

        const BodyType TYPE = BodyType.MESSAGE_FILE;

        public FileMessage(string messageString, long authorID, long dateOfCreation, FileStream file)
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
    }
}
