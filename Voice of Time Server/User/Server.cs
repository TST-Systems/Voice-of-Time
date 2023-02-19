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
    internal class Server : IDisposable
    {
        public Guid ServerIdentity { get; }

        public ServerConfig Config { get; }

        public RSA ServerKey { get; }


        private string DBName { get; }
        private readonly SqliteConnection DB;
        private readonly SqliteConnection StashDB;

        // Data Hasher
        private readonly SHA256 hasher = SHA256.Create();

        public Server(Guid? serverIdentity = null, RSA? serverKey = null, ServerConfig? config = null, string dbName = "Server")
        {
            Config          = config         ?? new();
            ServerKey       = serverKey      ?? RSA.Create();
            ServerIdentity  = serverIdentity ?? Guid.NewGuid();

            DBName = dbName;

            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());

            var createDB = !File.Exists($"{DBName}.sqlite");

            DB = new SqliteConnection($"Data Source={DBName}.sqlite");

            StartDB(createDB);

            Directory.CreateDirectory("Stash");
            StashDB = new SqliteConnection($@"Data Source=Stash\{DBName}.sqlite");
            StashDB.Open();
            Console.WriteLine("Stash started");
        }

        private void StartDB(bool createDB)
        {
            try
            {
                DB.Open();
                Console.WriteLine("DB started");
                if (createDB)
                {
                    var createScriptPath = @"SQLite\CREATE.sql";
                    if (!File.Exists(createScriptPath)) throw new FileNotFoundException(createScriptPath);

                    var createScript = File.ReadAllText(createScriptPath);
                    var createCommand = new SqliteCommand(createScript, DB);

                    createCommand.ExecuteNonQuery();
                    Console.WriteLine("DB Created");
                }

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

        internal long AddUser(RSA userPubKey, string username)
        {
            Random rdm = new();
            long userID;
            do 
            { 
                userID = rdm.Next(101, int.MaxValue); 
            }
            while (!IDIsFree(userID));

            TryRegisterUser(new(userID, username, userPubKey));
            SetupStash(userID);

            return userID;
        }

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

        internal void RegisterUser(PublicClient client)
        {
            using var cmd = new SqliteCommand("INSERT INTO USERS (id, username, publickey) VALUES (@id, @username, @publickey)", DB);
            cmd.Parameters.AddWithValue("@id", client.UserID);
            cmd.Parameters.AddWithValue("@username", client.Username);
            cmd.Parameters.AddWithValue("@publickey", JsonSerializer.Serialize(client.Key));

            cmd.ExecuteNonQuery();
        }

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
                cmd.Parameters.AddWithValue("@state",   (int)ChatUserState.ADMIN);
                cmd.ExecuteNonQuery();
            }
        }

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

        internal void ChangeUserUsername(long userID, string userName)
        {
            using var cmd = new SqliteCommand("UPDATE USERS SET username = @username WHERE id = @id", DB);
            cmd.Parameters.AddWithValue("@username", userName);
            cmd.Parameters.AddWithValue("@id",       userID);
            cmd.ExecuteNonQuery();
            // TODO: Return if succesfull
        }

        internal void ChangeUserKey(long userID, RSA userPubKey)
        {
            using var cmd = new SqliteCommand("UPDATE OR FAIL users SET PublicKey = @PublicKey WHERE id = @id");
            cmd.Parameters.AddWithValue("@PublicKey", JsonSerializer.Serialize(new PublicRSA(userPubKey)));
            cmd.Parameters.AddWithValue("@id",        userID);
            cmd.ExecuteNonQuery();
        }

        internal List<(long, ChatUserState)> GetChatMembers(long chatID)
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

        internal void AddChatUser(long chatID, long targetID, ChatUserState state)
        {
            using var cmd = new SqliteCommand("INSERT OR REPLACE INTO CHAT_MEMBERS (chat_id, user_id, state) VALUES (@chat_id, @user_id, @state)", DB);
            cmd.Parameters.AddWithValue("@user_id", targetID);
            cmd.Parameters.AddWithValue("@chat_id", chatID);
            cmd.Parameters.AddWithValue("@state",   (int)state);
            cmd.ExecuteNonQuery();
        }

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

        public long StashMessage(long targetID, long authorID, string message, DateTime expires, DataHandling handling)
        {
            // Get a File name
            var input = Encoding.UTF8.GetBytes(message);
            var bName = hasher.ComputeHash(input);
            var sName = Convert.ToBase64String(bName);

            sName = sName.Replace('/', '-');

            while (File.Exists(@$"Stash\{sName}"))
            {
                var _input = Encoding.UTF8.GetBytes(sName);
                var _bName = hasher.ComputeHash(_input);
                sName = Convert.ToBase64String(_bName);
                sName = sName.Replace('/', '-');
            }

            File.WriteAllText(@$"Stash\{sName}", message);

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

            using (var cmd = new SqliteCommand($"SELECT MAX(Receipt) FROM m{targetID}", StashDB))
            {
                using var reader = cmd.ExecuteReader();
                if (!reader.Read()) throw new Exception("DB broken!");

                return reader.GetInt64(0);
            }
        }

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

            if (expires <= DateTime.Now)
            {
                StashMessageRemove(targetID, receiptID);
                return null;
            }

            if(messageHandling == DataHandling.REMOVE_AFTER_GET)
            {
                // TODO: Check if User / Chat and remove if user
            }

            return new(receipt, messageHandling, authorID, targetID, created, expires, message); ;
        }

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

        public bool StashMessageRemove(long targetID, long receiptID)
        {
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
