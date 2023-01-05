using System.Security.Cryptography;
using VoTCore.Secure;

namespace Voice_of_Time_Server
{
    internal class Server : PublicKeyStorage
    {
        public ServerConfig Config { get; }

        public Dictionary<long, UserInfo> UserDB { get; }

        public Server(Dictionary<long, RSA>? publicKeyDictionary = null, ServerConfig? config = null, Dictionary<long, UserInfo>? userDB = null)
            : base(publicKeyDictionary)
        {
            Config = config ?? new();
            UserDB = userDB ?? new();
        }

    }
}
