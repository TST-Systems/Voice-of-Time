using System.Text.Json.Serialization;
using VoTCore.Package.Interfaces;

/**
 * @author      - Timeplex
 * 
 * @created     - 13.01.2023
 * 
 * @last_change - 20.01.2023
 */
namespace VoTCore.Package.Header
{
    /// <summary>
    /// Header for user -> server communication for requesting keys, regestration and other usecases
    /// </summary>
    public class HeaderReq : IVOTPHeader
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="senderID">ID of sender</param>
        /// <param name="request">Type of request</param>
        /// <param name="targetID">Currently unused</param>
        public HeaderReq(long senderID, RequestType request, long? targetID = null)
        {
            SenderID = senderID;
            Request  = request;
            TargetID = targetID;
        }

        [JsonIgnore]
        public short Version { get; } = 2;

        /// <summary>
        /// ID of sender
        /// </summary>
        public long  SenderID { get; }

        public long? TargetID { get; }
        /// <summary>
        /// Type of request
        /// </summary>
        public RequestType Request { get; }


        public override bool Equals(object? obj)
        {
            if (obj is null)                return false;

            if (obj is not HeaderReq their) return false;

            if (Version  != their.Version)  return false;
            if (SenderID != their.SenderID) return false;
            if (Request  != their.Request)  return false;

            return true;
        }

        public override int GetHashCode()
        {
            return (int)(Version * SenderID * ((long)Request));
        }
    }
}