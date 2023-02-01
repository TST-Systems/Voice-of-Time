using VoTCore.Secure;

/**
 * @author      - Timeplex
 * 
 * @created     - 01.02.2023
 * 
 * @last_change - 01.02.2023
 */
namespace VoTCore.User
{
    public class PublicClient
    {
        public long       ID         { get; set; }
        public string?    Username   { get; set; }
        public PublicRSA? PublicKey  { get; set; }

        public PublicClient(long iD, string username, PublicRSA publicKey)
        {
            ID        = iD;
            Username  = username;
            PublicKey = publicKey;
        }
    }
}
