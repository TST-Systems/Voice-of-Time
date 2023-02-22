using System.Text.Json.Serialization;

namespace VoTCore.Data
{
    /// <summary>
    /// Soft exeption class for thrown or hanbdling exeptions over distance like server->client client->server
    /// </summary>
    public class InternalException
    {
        /// <summary>
        /// Messge for explainations
        /// </summary>
        public string Message { get; }
        /// <summary>
        /// An error code
        /// </summary>
        public InternalExceptionCode Code { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="message">Discription</param>
        /// <param name="code">Error code</param>
        [JsonConstructor]
        public InternalException(string message, InternalExceptionCode code)
        {
            Message = message;
            Code = code;
        }
    }
}
