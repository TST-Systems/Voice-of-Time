using System.Security.Cryptography;
using VoTCore.Secure;

namespace Voice_of_Time_Server
{
    internal class Server : PublicKeyStorage
    {
        public ServerConfig Config { get; }

        public Dictionary<long, UserInfo> UserDB { get; }

    }
}
