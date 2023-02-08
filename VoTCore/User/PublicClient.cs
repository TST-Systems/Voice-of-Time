using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using VoTCore.Secure;

/**
 * @author      - Timeplex
 * 
 * @created     - 01.02.2023
 * 
 * @last_change - 03.02.2023
 */
namespace VoTCore.User
{
    [Serializable]
    [KnownType(typeof(PublicRSA))]
    public class PublicClient : ISerializable
    {
        public long       ID         { get; set; }
        public string?    Username   { get; set; }
        public PublicRSA? PublicKey  { get; set; }

        protected PublicClient(SerializationInfo info, StreamingContext context)
        {
            ID = info.GetInt64(nameof(ID));

            var username = info.GetString(nameof(Username));
            if(username is not null or "") Username = username;

            PublicKey = (PublicRSA?) info.GetValue(nameof(PublicKey), typeof(PublicRSA));
        }

        [JsonConstructor]
        protected PublicClient(long iD, string username, System.Security.Cryptography.RSA rSA)
        {
            ID = iD;
            Username = username;
        }

        public PublicClient(long iD, string username, PublicRSA publicKey)
        {
            ID        = iD;
            Username  = username;
            PublicKey = publicKey;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(ID), ID);

            if(Username is null)
            {
                info.AddValue(nameof(Username), "");
            }
            else
            {
                info.AddValue(nameof(Username), Username);
            }

            if(PublicKey is null)
            {
                info.AddValue(nameof(PublicKey), null);
            }
            else
            {
                info.AddValue(nameof(PublicKey), PublicKey);
            }
        }
    }
}
