using System.Runtime.Serialization;
using System.Text.Json.Serialization;
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
    /// Basic text message
    /// </summary>
    [Serializable]
    public class TextMessage : Message, IVOTPBody, ISerializable
    {
        const BodyType TYPE = BodyType.MESSAGE_TEXT;

        public TextMessage(SerializationInfo info, StreamingContext context) : base(info, context) { }

        [JsonConstructor]
        public TextMessage(string messageString, long authorID, DateTime dateOfCreation)
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
