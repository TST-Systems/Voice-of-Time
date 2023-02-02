using System.Security.Cryptography;
using VoTCore.Secure;
using VoTCore.User;

/**
 * @author      - Timeplex
 * 
 * @created     - 24.12.2022
 * 
 * @last_change - 01.02.2023
 */
namespace Voice_of_Time_Server
{
    internal class Server
    {
        public Guid ServerIdentity { get; }

        public ServerConfig Config { get; }

        public Dictionary<long, PublicClient> UserDB { get; }

        public RSA ServerKey { get; }

        public Server(Guid? serverIdentity = null, RSA? serverKey = null, ServerConfig? config = null, Dictionary<long, PublicClient>? userDB = null)
        {
            Config         = config         ?? new();
            UserDB         = userDB         ?? new();
            ServerKey      = serverKey      ?? RSA.Create();
            ServerIdentity = serverIdentity ?? Guid.NewGuid();
        }

        internal long AddUser(RSA userPubKey, string username)
        {
            Random rdm = new();
            long userID;
            do
            {
                userID = rdm.NextInt64();
            }
            while (userID <= 0 && !UserDB.ContainsKey(userID));

            UserDB[userID] = new(userID, username, new(userPubKey));

            return userID;
        }
    }
}
