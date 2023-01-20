using VoTCore.Package.Interfaces;

namespace VoTCore.Communication
{
    /// <summary>
    /// Basic text message
    /// </summary>
    public class TextMessage : Message, IVOTPBody
    {
        const BodyType TYPE = BodyType.MESSAGE_TEXT;

        public TextMessage(string messageString, long authorID, long dateOfCreation) 
            : base(messageString, authorID, dateOfCreation, TYPE)
        {
        }


        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
