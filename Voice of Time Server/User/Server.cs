using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Voice_of_Time_Server.Config;
using VoTCore;
using VoTCore.Communication.Extra;
using VoTCore.Data;
using VoTCore.Secure;
using VoTCore.User;

/**
 * @author      - Timeplex
 * 
 * @created     - 24.12.2022
 * 
 * @last_change - 18.02.2023
 */
namespace Voice_of_Time_Server.User
{
    /// <summary>
    /// Server data storage
    /// </summary>
    internal class Server : IDisposable
    {
        /// <summary>
        /// Server UID
        /// </summary>
        public Guid ServerIdentity { get; }
        /// <summary>
        /// Server Config
        /// </summary>
        public ServerConfig Config { get; }
        /// <summary>
        /// Server Key pair
        /// </summary>
        public RSA ServerKey { get; }

        /// <summary>
        /// DB-Profile Name
        /// </summary>
        private string DBName { get; }
        /// <summary>
        /// ID-DB
        /// Stores the following:
        /// - IDs
        /// - Users
        /// - Chats
        /// - Channels
        /// - Member of Channels
        /// </summary>
        private readonly SqliteConnection DB;
        /// <summary>
        /// Stash-DB
        /// Link DB between file storage and user/chat/channel
        /// </summary>
        private readonly SqliteConnection StashDB;

        // Data Name Hasher
        private readonly SHA256 hasher = SHA256.Create();

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="serverIdentity">Server UID</param>
        /// <param name="serverKey">Server Key pair</param>
        /// <param name="config">Servert config</param>
        /// <param name="dbName">Profile-Name</param>
        public Server(Guid? serverIdentity = null, RSA? serverKey = null, ServerConfig? config = null, string dbName = "Server")
        {
            Config          = config         ?? new();
            ServerKey       = serverKey      ?? RSA.Create();
            ServerIdentity  = serverIdentity ?? Guid.NewGuid();

            DBName = dbName;
            
            // SQLite stuff :/
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());

            // Check if the DB musst be created
            var createDB = !File.Exists($"{DBName}.sqlite");
            // Create DB
            DB = new SqliteConnection($"Data Source={DBName}.sqlite");
            // Start DB
            StartDB(createDB);

            // (Create +) Open StashDB 
            Directory.CreateDirectory("Stash");
            StashDB = new SqliteConnection($@"Data Source=Stash\{DBName}.sqlite");
            StashDB.Open();
            Console.WriteLine("Stash started");
        }

        /// <summary>
        /// SB starter
        /// </summary>
        /// <param name="createDB">DB needs to be created</param>
        /// <exception cref="FileNotFoundException">Template not found</exception>
        private void StartDB(bool createDB)
        {
            try
            {
                DB.Open();
                Console.WriteLine("DB started");
                // Init DB
                if (createDB)
                {
                    // Load Template & Create it
                    var createScriptPath = @"SQLite\CREATE.sql";
                    if (!File.Exists(createScriptPath)) throw new FileNotFoundException(createScriptPath);

                    var createScript = File.ReadAllText(createScriptPath);
                    var createCommand = new SqliteCommand(createScript, DB);

                    createCommand.ExecuteNonQuery();
                    Console.WriteLine("DB Created");
                }
                // Run every start commands (pragmas)
                var initScriptPath = @"SQLite\INIT.sql";
                if (!File.Exists(initScriptPath)) throw new FileNotFoundException(initScriptPath);

                var initScript = File.ReadAllText(initScriptPath);
                var initCommand = new SqliteCommand(initScript, DB);

                initCommand.ExecuteNonQuery();
                Console.WriteLine("DB inited");

            }
            catch(SqliteException ex)
            {
                Console.WriteLine("DB error!");
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Register a new User to the system
        /// </summary>
        /// <param name="userPubKey">Public Key of user</param>
        /// <param name="username">Username of user</param>
        /// <returns>User ID of new user</returns>
        internal long AddUser(RSA userPubKey, string username)
        {
            Random rdm = new();
            long userID;
            // Get an free ID
            do 
            { 
                userID = rdm.Next(101, int.MaxValue); 
            }
            while (!IDIsFree(userID));

            TryRegisterUser(new(userID, username, userPubKey));
            SetupStash(userID);

            return userID;
        }

        /// <summary>
        /// Register a new Chat to the system
        /// </summary>
        /// <param name="creatorID">User ID of the creator of the chat</param>
        /// <returns>Chat ID of new chat</returns>
        internal long AddChat(long creatorID)
        {
            Random rdm = new();

            long chatID;
            do
            {
                chatID = rdm.NextInt64(((long)int.MaxValue) + 1, long.MaxValue);
            }
            while (!IDIsFree(chatID));

            TryRegisterChat(chatID, creatorID);
            SetupStash(chatID);

            return chatID;
        }

        #region DBCommands
        /// <summary>
        /// Check if a given ID is already occupied
        /// </summary>
        /// <param name="id">ID to check</param>
        /// <returns>ID if free to use</returns>
        /// <exception cref="Exception"></exception>
        internal bool IDIsFree (long id)
        {
            using var cmd = new SqliteCommand("SELECT COUNT(*) FROM IDS WHERE id = @id", DB);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                throw new Exception("Somthing went wrong!");
            }
            var count = reader.GetInt32(0);
            if(count > 1)
            {
                throw new Exception("Table is broken!");
            }
            return count == 0;
        }

        /// <summary>
        /// Add a user into the DB
        /// </summary>
        /// <param name="client">Public user data</param>
        internal void RegisterUser(PublicClient client)
        {
            using var cmd = new SqliteCommand("INSERT INTO USERS (id, username, publickey) VALUES (@id, @username, @publickey)", DB);
            cmd.Parameters.AddWithValue("@id", client.UserID);
            cmd.Parameters.AddWithValue("@username", client.Username);
            cmd.Parameters.AddWithValue("@publickey", JsonSerializer.Serialize(client.Key));

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Wrapper for <see cref="RegisterUser"/> if the ID is already occupied
        /// </summary>
        /// <param name="client">Public user data</param>
        /// <returns>Client could be added</returns>
        internal bool TryRegisterUser(PublicClient client)
        {
            try
            {
                RegisterUser(client);
            }
            catch (SqliteException ex) 
            {
                Console.WriteLine("DB error: " + ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add a chat to the system
        /// </summary>
        /// <param name="chatID">ID of the chat</param>
        /// <param name="creatorID">ID of the creator</param>
        internal void RegisterChat(long chatID, long creatorID)
        {
            using (var cmd = new SqliteCommand("INSERT INTO CHATS (id) VALUES (@id)", DB))
            {
                cmd.Parameters.AddWithValue("@id", chatID);
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new SqliteCommand("INSERT INTO CHAT_MEMBERS (chat_id, user_id, state) VALUES (@chat_id, @user_id, @state)", DB))
            {
                cmd.Parameters.AddWithValue("@chat_id", chatID);
                cmd.Parameters.AddWithValue("@user_id", creatorID);
                cmd.Parameters.AddWithValue("@state",   (int)(ChatUserState.ADMIN | ChatUserState.MEMBER));
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Wrapper for <see cref="RegisterChat"/> if the ID is already occupied
        /// </summary>
        /// <param name="chatID">ID of the chat</param>
        /// <param name="creatorID">ID of the creator</param>
        /// <returns>Chat could be added</returns>
        internal bool TryRegisterChat(long chatID, long creatorID)
        {
            try
            {
                RegisterChat(chatID, creatorID);
            }
            catch (SqliteException ex)
            {
                Console.WriteLine("DB error: " + ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get User data if user exists
        /// </summary>
        /// <param name="userID">ID of user</param>
        /// <returns>User data or null</returns>
        internal PublicClient? GetUser(long userID)
        {
            using var cmd = new SqliteCommand("SELECT id, username, publickey FROM USERS WHERE id = @id", DB);
            cmd.Parameters.AddWithValue("@id", userID);

            using var reader = cmd.ExecuteReader();

            if(!reader.Read()) return null;
            
            var id        = reader.GetInt64(0);
            var username  = reader.GetString(1);
            var serPubKey = reader.GetString(2);
            var PublicKey = JsonSerializer.Deserialize<PublicRSA>(serPubKey);

            if (PublicKey is null) return null;

            return new(id, username, PublicKey);
        }

        /// <summary>
        /// Get a List of IDs of users
        /// </summary>
        /// <returns>List of all user IDs</returns>
        internal long[] GetUserIDs()
        {
            List<long> ids = new();

            using var cmd    = new SqliteCommand("SELECT id FROM USERS", DB);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ids.Add(reader.GetInt64(0));
            }
            return ids.ToArray();
        }
        
        /// <summary>
        /// Check if a user is known by the server
        /// </summary>
        /// <param name="userID">Id of designated user</param>
        /// <returns>User exists</returns>
        /// <exception cref="Exception"></exception>
        internal bool UserExists(long userID)
        {
            using var cmd = new SqliteCommand("SELECT COUNT(*) FROM USERS WHERE id = @id", DB);
            cmd.Parameters.AddWithValue("@id", userID);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                throw new Exception("Somthing went wrong!");
            }
            var count = reader.GetInt32(0);
            if (count > 1)
            {
                throw new Exception("Table is broken!");
            }
            return count == 1;
        }

        /// <summary>
        /// Change the username of a user
        /// </summary>
        /// <param name="userID">ID of targeted user</param>
        /// <param name="userName">New username</param>
        internal void ChangeUserUsername(long userID, string userName)
        {
            using var cmd = new SqliteCommand("UPDATE USERS SET username = @username WHERE id = @id", DB);
            cmd.Parameters.AddWithValue("@username", userName);
            cmd.Parameters.AddWithValue("@id",       userID);
            cmd.ExecuteNonQuery();
            // TODO: Return if succesfull
        }

        /// <summary>
        /// Change the public Key of a user
        /// </summary>
        /// <param name="userID">ID of user</param>
        /// <param name="userPubKey">New public key</param>
        internal void ChangeUserKey(long userID, RSA userPubKey)
        {
            using var cmd = new SqliteCommand("UPDATE OR FAIL users SET PublicKey = @PublicKey WHERE id = @id", DB);
            cmd.Parameters.AddWithValue("@PublicKey", JsonSerializer.Serialize(new PublicRSA(userPubKey)));
            cmd.Parameters.AddWithValue("@id",        userID);
            cmd.ExecuteNonQuery();
            // TODO: Return if succesfull
        }

        /// <summary>
        /// Get a list of all chatmember + state of a chat
        /// </summary>
        /// <param name="chatID">ID of chat</param>
        /// <returns>List of all chatmember + state</returns>
        internal List<(long userID, ChatUserState state)> GetChatMembers(long chatID)
        {
            List<(long, ChatUserState)> values = new();

            using var cmd = new SqliteCommand("SELECT user_id, state FROM CHATS WHERE chat_id = @chat_id", DB);
            cmd.Parameters.AddWithValue("@chat_id", chatID);
            using var reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                var userID = reader.GetInt64(0);
                var uState = (ChatUserState)reader.GetInt16(1);
                values.Add((userID, uState));
            }
            return values;
        }

        /// <summary>
        /// Check if a Chat is known by the server
        /// </summary>
        /// <param name="chatID">ID of targeted chat</param>
        /// <returns>Chat exists</returns>
        /// <exception cref="Exception"></exception>
        internal bool ChatExists(long chatID)
        {
            using var cmd = new SqliteCommand("SELECT COUNT(*) FROM CHATS WHERE id = @id", DB);
            cmd.Parameters.AddWithValue("@id", chatID);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                throw new Exception("Somthing went wrong!");
            }
            var count = reader.GetInt32(0);
            if (count > 1)
            {
                throw new Exception("Table is broken!");
            }
            return count == 1;
        }

        /// <summary>
        /// Get the state of a chatmember
        /// </summary>
        /// <param name="chatID">ID of chat</param>
        /// <param name="userID">ID of user</param>
        /// <returns>state</returns>
        internal ChatUserState GetChatMember(long chatID, long userID)
        {
            using var cmd = new SqliteCommand("SELECT state FROM CHAT_MEMBERS WHERE user_id = @user_id AND chat_id = @chat_id", DB);
            cmd.Parameters.AddWithValue("@user_id", userID);
            cmd.Parameters.AddWithValue("@chat_id", chatID);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return ChatUserState.NONE;
            }
            return (ChatUserState)reader.GetInt16(0);
        }

        /// <summary>
        /// Add a user to a chat with given state
        /// </summary>
        /// <param name="chatID">ID of chat</param>
        /// <param name="targetID">ID of user</param>
        /// <param name="state">state</param>
        internal void AddChatUser(long chatID, long targetID, ChatUserState state)
        {
            using var cmd = new SqliteCommand("INSERT OR REPLACE INTO CHAT_MEMBERS (chat_id, user_id, state) VALUES (@chat_id, @user_id, @state)", DB);
            cmd.Parameters.AddWithValue("@user_id", targetID);
            cmd.Parameters.AddWithValue("@chat_id", chatID);
            cmd.Parameters.AddWithValue("@state",   (int)state);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Change the state of a user. User need to already added to chat!
        /// </summary>
        /// <param name="chatID">ID of chat</param>
        /// <param name="targetID">ID of user</param>
        /// <param name="state">New state</param>
        internal void UpdateChatMemberState(long chatID, long targetID, ChatUserState state)
        {
            using var cmd = new SqliteCommand("UPDATE CHAT_MEMBERS SET state = @state WHERE chat_id = @chat_id AND user_id = @user_id", DB);
            cmd.Parameters.AddWithValue("@user_id", targetID);
            cmd.Parameters.AddWithValue("@chat_id", chatID);
            cmd.Parameters.AddWithValue("@state",   (int)state);
            cmd.ExecuteNonQuery();
        }
        #endregion


        #region StashDBCommands
        /// <summary>
        /// Create a new stash for a user/chat/channel.
        /// + Template for new Shards.
        /// </summary>
        /// <param name="ID">ID for stash (Should match a existing ID)</param>
        private void SetupStash(long ID)
        {
            using var cmd = new SqliteCommand(
                $"CREATE TABLE IF NOT EXISTS m{ID} (" +
                $"Receipt INTEGER, " +
                $"MessageHandling INTEGER NOT NULL," +
                $"Author INTEGER NOT NULL," +
                $"Created NUMERIC NOT NULL," +
                $"Expires NUMERIC NOT NULL," +
                $"StashFile TEXT NOT NULL," +
                $"PRIMARY KEY(Receipt));", 
                StashDB);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Add a message to a stash
        /// </summary>
        /// <param name="targetID">ID of tageted stash</param>
        /// <param name="authorID">ID of author (user)</param>
        /// <param name="message">Message as string</param>
        /// <param name="expires">Date of expiring</param>
        /// <param name="handling">Data handling guidelines</param>
        /// <returns>ReceipID</returns>
        /// <exception cref="Exception"></exception>
        public long StashMessage(long targetID, long authorID, string message, DateTime expires, DataHandling handling)
        {
            // Get a File name
            var input = Encoding.UTF8.GetBytes(message);
            var bName = hasher.ComputeHash(input);
            var sName = Convert.ToBase64String(bName);
            // Replace non usable symbol
            sName = sName.Replace('/', '-');

            // Repeat until a non used name is found
            while (File.Exists(@$"Stash\{sName}"))
            {
                var _input = Encoding.UTF8.GetBytes(sName);
                var _bName = hasher.ComputeHash(_input);
                sName = Convert.ToBase64String(_bName);
                sName = sName.Replace('/', '-');
            }

            // Write the message into the file
            File.WriteAllText(@$"Stash\{sName}", message);

            // Link it via the stash
            using (var cmd = new SqliteCommand($"INSERT INTO m{targetID} (messagehandling, author, created, expires, stashfile) " +
                "VALUES (@messagehandling, @author, @created, @expires, @stashfile)", StashDB))
            {
                cmd.Parameters.AddWithValue("@messagehandling", (int)handling);
                cmd.Parameters.AddWithValue("@author",          authorID);
                cmd.Parameters.AddWithValue("@created",         DateTime.Now);
                cmd.Parameters.AddWithValue("@expires",         expires);
                cmd.Parameters.AddWithValue("@stashfile",       sName);
                cmd.ExecuteNonQuery();
            }

            // TODO: Garantie für richtig ID. / lock stash for time of using that not the wrong ID will be highest

            // Get the receipID
            using (var cmd = new SqliteCommand($"SELECT MAX(Receipt) FROM m{targetID}", StashDB))
            {
                using var reader = cmd.ExecuteReader();
                if (!reader.Read()) throw new Exception("DB broken!");

                return reader.GetInt64(0);
            }
        }

        /// <summary>
        /// Get a message of a stash my it receiptID
        /// </summary>
        /// <param name="targetID">ID of stash</param>
        /// <param name="receiptID">ID of receipt</param>
        /// <returns>Message if exists</returns>
        public StashMessage? StashMessageGet(long targetID, long receiptID)
        {
            using var cmd = new SqliteCommand($"SELECT Receipt, MessageHandling, Author, Created, Expires, StashFile FROM m{targetID} WHERE receipt = @receipt", StashDB);
            cmd.Parameters.AddWithValue("@receipt", receiptID);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            var receipt         = reader.GetInt64   (0);
            var messageHandling = (DataHandling)
                                  reader.GetInt16   (1);
            var authorID        = reader.GetInt64   (2);
            var created         = reader.GetDateTime(3);
            var expires         = reader.GetDateTime(4);
            var messageLocation = reader.GetString  (5);

            var message         = "";

            var filePath = @$"Stash\{messageLocation}";
            if (File.Exists(filePath)) 
            {
                message = File.ReadAllText(filePath);
            }

            // If data is expired remove
            if (expires <= DateTime.Now)
            {
                StashMessageRemove(targetID, receiptID);
                return null;
            }

            // If data should instantlie be removed after reading, remove it
            if(messageHandling == DataHandling.REMOVE_AFTER_GET)
            {
                // TODO: Check if User / Chat and remove if user
            }

            return new(receipt, messageHandling, authorID, targetID, created, expires, message); ;
        }

        /// <summary>
        /// Get all IDs of messages stored in a stash
        /// </summary>
        /// <param name="targetID">ID of stash</param>
        /// <returns>List of IDs contained in the stash</returns>
        public long[] StashMessageList(long targetID) 
        { 
            List<long> list = new();
            using var cmd = new SqliteCommand($"SELECT Receipt FROM m{targetID}", StashDB);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(reader.GetInt64(0));
            }
            return list.ToArray();
        }

        /// <summary>
        /// Remove a message from a stash
        /// </summary>
        /// <param name="targetID">ID of stash</param>
        /// <param name="receiptID">ID of message</param>
        /// <returns>Message could be deleted</returns>
        public bool StashMessageRemove(long targetID, long receiptID)
        {
            // Remove the file first
            using (var cmd = new SqliteCommand($"SELECT StashFile FROM m{targetID} WHERE receipt = @receipt", StashDB))
            {
                cmd.Parameters.AddWithValue("@receipt", receiptID);
                using var reader = cmd.ExecuteReader();
                if(!reader.Read()) return false;

                var messageLocation = reader.GetString(0);

                var filePath = @$"Stash\{messageLocation}";
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            // Remove the link
            using (var cmd = new SqliteCommand($"DELETE FROM m{targetID} WHERE receipt = @receipt", StashDB))
            {
                cmd.Parameters.AddWithValue("@receipt", receiptID);
                cmd.ExecuteNonQuery();
            }
            return true;
        }
        #endregion


        public void Dispose()
        {
            DB.Close();
            StashDB.Close();

            DB.Dispose();
            StashDB.Dispose();

            ServerKey.Dispose();
        }

    }
}
