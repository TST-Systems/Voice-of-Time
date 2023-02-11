using System.Security.Cryptography;
using Voice_of_Time_Server.Config;
using VoTCore.User;

/**
 * @author      - Timeplex
 * 
 * @created     - 24.12.2022
 * 
 * @last_change - 01.02.2023
 */
namespace Voice_of_Time_Server.User
{
    internal class Server
    {
        public Guid ServerIdentity { get; }

        public ServerConfig Config { get; }

        public Dictionary<long, PublicClient> UserDB { get; }
        public Dictionary<long, List<long>>   ChatDB { get; }

        public RSA ServerKey { get; }

        public Server(Guid? serverIdentity = null, RSA? serverKey = null, ServerConfig? config = null, Dictionary<long, PublicClient>? userDB = null, Dictionary<long, List<long>> chatDB = null)
        {
            Config          = config ?? new();
            UserDB          = userDB ?? new();
            ServerKey       = serverKey ?? RSA.Create();
            ServerIdentity  = serverIdentity ?? Guid.NewGuid();
            ChatDB          = chatDB;
        }

        internal long AddUser(RSA userPubKey, string username)
        {
            Random rdm = new();
            long userID;
            do
            {
                userID = rdm.Next(1, 100); // <- Just for Test Reasons // rdm.Next() 
            }
            while (!UserDB.ContainsKey(userID) && !ChatDB.ContainsKey(userID));

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
            while (!UserDB.ContainsKey(chatID) && !ChatDB.ContainsKey(chatID));

            ChatDB[chatID] = new(new long[] { creator });

            return chatID;
        }
}
