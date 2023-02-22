using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using VoTCore.Package.Interfaces;
using VoTCore.Secure;

/**
 * @author      - Timeplex
 * 
 * @created     - 01.02.2023
 * 
 * @last_change - 15.02.2023
 */
namespace VoTCore.User
{
    /// <summary>
    /// Class to hold any nessary informations about a user for outhers to get
    /// </summary>
    [DataContract]
    [KnownType(typeof(PublicRSA))]
    public class PublicClient : IVOTPBody
    {
        [DataMember]
        public long      UserID    { get; set; }
        [DataMember]
        public string    Username  { get; set; }
        [DataMember]
        public PublicRSA Key       { get; set; }

        [JsonIgnore]
        public BodyType Type => BodyType.PUBLIC_CLIENT;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="userID">ID of user</param>
        /// <param name="username">Username of user</param>
        /// <param name="key">Public key of user</param>
        [JsonConstructor]
        public PublicClient(long userID, string username, PublicRSA key)
        {
            UserID    = userID;
            Username  = username;
            Key = key;
        }

        /// <summary>
        /// Wrapper for <see cref="PublicClient(long, string , PublicRSA)"/>
        /// </summary>
        /// <param name="userID">ID of user</param>
        /// <param name="username">Username of user</param>
        /// <param name="key">RSA key</param>
        public PublicClient(long userID, string username, RSA key) : this(userID, username, new PublicRSA(key))
        {
        }

    }
}
