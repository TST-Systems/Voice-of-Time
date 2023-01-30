using System.Security.Cryptography;
using VoTCore.Secure;

/**
 * @author      - Timeplex
 * 
 * @created     - 24.12.2022
 * 
 * @last_change - 20.01.2023
 */
namespace Voice_of_Time_Server
{
    internal class Server : PublicKeyStorage
    {
        public Guid ServerIdentity { get; }

        public ServerConfig Config { get; }

        public Dictionary<long, UserInfo> UserDB { get; }

        public RSA ServerKey { get; }

        public Server(Guid? serverIdentity = null, RSA? serverKey = null, Dictionary<long, RSA>? publicKeyDictionary = null, ServerConfig? config = null, Dictionary<long, UserInfo>? userDB = null)
            : base(publicKeyDictionary)
        {
            Config         = config         ?? new();
            UserDB         = userDB         ?? new();
            ServerKey      = serverKey      ?? RSA.Create();
            ServerIdentity = serverIdentity ?? Guid.NewGuid();
        }

        internal long AddUser(RSA userPubKey)
        {
            Random rdm = new();
            long userID;
            do
            {
                userID = rdm.NextInt64();
            }
            while (userID <= 0 && !UserDB.ContainsKey(userID));

            base.AddPublicKey(userID, userPubKey);
            UserDB[userID] = new("Guest");

            return userID;
        }
    }
}
