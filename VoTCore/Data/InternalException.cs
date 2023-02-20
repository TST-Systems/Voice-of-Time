using System.Text.Json.Serialization;

namespace VoTCore.Data
{
    public class InternalException
    {
        public string Message { get; }
        public InternalExceptionCode Code { get; }

        [JsonConstructor]
        public InternalException(string message, InternalExceptionCode code)
        {
            Message = message;
            Code = code;
        }
    }
}
