using System.Security.Cryptography;
using Voice_of_Time_Server.Config;
using VoTCore.Communication.Extra;
using VoTCore.User;

/**
 * @author      - Timeplex
 * 
 * @created     - 24.12.2022
 * 
 * @last_change - 11.02.2023
 */
namespace Voice_of_Time_Server.User
{
    internal class Server
    {
        public Guid ServerIdentity { get; }

        public ServerConfig Config { get; }

        public Dictionary<long, PublicClient> UserDB { get; }
        public Dictionary<long, List<(long, ChatUserState)>> ChatDB { get; }

        public RSA ServerKey { get; }

        public Server(Guid? serverIdentity = null, RSA? serverKey = null, ServerConfig? config = null, Dictionary<long, PublicClient>? userDB = null, Dictionary<long, List<(long, ChatUserState)>>? chatDB = null)
        {
            Config          = config ?? new();
            UserDB          = userDB ?? new();
            ChatDB          = chatDB ?? new();
            ServerKey       = serverKey      ?? RSA.Create();
            ServerIdentity  = serverIdentity ?? Guid.NewGuid();
        }

        internal long AddUser(RSA userPubKey, string username)
        {
            Random rdm = new();
            long userID;
            do
            {
#if DEBUG
                userID = rdm.Next(1, 100);
#else
                userID = rdm.Next(101, int.MaxValue); // Reserverd IDs: 0-1-100
#endif
            }
            while (UserDB.ContainsKey(userID) || ChatDB.ContainsKey(userID));

            UserDB[userID] = new(userID, username, new(userPubKey));

            return userID;
        }

        internal long AddChat(long creator)
        {
            Random rdm = new();

            long chatID;
            do
            {
                chatID = rdm.NextInt64(((long)int.MaxValue) + 1, long.MaxValue);
            }
            while (UserDB.ContainsKey(chatID) || ChatDB.ContainsKey(chatID));

            ChatDB[chatID] = new()
            {
                (creator, ChatUserState.ADMIN | ChatUserState.MEMBER)
            };

            return chatID;
        }
    }
}
