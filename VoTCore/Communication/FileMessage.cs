namespace VoTCore.Communication
{
    public class FileMessage : IMessage
    {
        private readonly FileStream? file;
        private readonly short       typeOfMessage;
        private readonly string      messageString;
        private readonly long        authorID;
        private readonly long        dateOfCreation;

        public FileMessage(FileStream? file, short typeOfMessage, string textMessage, long authorID, long dateOfCreation)
        {
            this.file           = file;
            this.typeOfMessage  = typeOfMessage;
            this.messageString  = textMessage;
            this.authorID       = authorID;
            this.dateOfCreation = dateOfCreation;
        }

        public short TypeOfMessage {
            get { return typeOfMessage; }
        }

        public string MessageString
        {
            get { return messageString; }
        }
    

        public long AuthorID
        {
            get { return authorID; }
        }

        public long DateOfCreation
        {
            get { return dateOfCreation; }
        }

        public FileStream? File
        {
            get { return file; }
        }
    }
}
