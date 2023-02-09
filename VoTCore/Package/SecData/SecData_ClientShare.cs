using System.Text.Json.Serialization;
using VoTCore.Secure;
using VoTCore.User;

/**
 * @author      - Timeplex
 * 
 * @created     - 09.02.2023
 * 
 * @last_change - 09.02.2023
 */
namespace VoTCore.Package.SecData
{
    public class SecData_ClientShare : SecData_Key_RSA
    {
        public string Username {get;}

        [JsonIgnore]
        public override BodyType Type => BodyType.SECDATA_PUBLIC_CLIENT_SHARE; 

        public SecData_ClientShare(PublicClient publicClient) : this(publicClient.PublicKey ?? throw new Exception("Public Key can not be null!"), publicClient.ID, publicClient.Username ?? "") {}

        public SecData_ClientShare(PublicRSA key, long sourceID, string username) : base(key, sourceID)
        {
            Username = username;
        }

        [JsonConstructor]
        public SecData_ClientShare(string publicKeyAsXML, long sourceID, string username) : base(publicKeyAsXML, sourceID)
        {
            Username = username;
        }

        public PublicClient GetPublicClient()
        {
            return new(SourceID, Username, new(GetKey()));
        }
    }
}
