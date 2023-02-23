using System.Runtime.Serialization;
using Voice_of_Time.Transfer;
using Voice_of_Time.User;
using VoTCore.Controll;
using VoTCore.Exeptions;

/**
 * @author      - Timeplex
 * 
 * @created     - 27.01.2023
 * 
 * @last_change - 12.02.2023
 */
namespace Voice_of_Time.Shared
{
    /// <summary>
    /// Static class which holds all nessaray Data for all classes
    /// </summary>
    [Serializable]
    public static class ClientData
    {
        /// <summary>
        /// Register of all users for each Server
        /// </summary>
        private static readonly Dictionary<Guid, Client> UserRegister = new();
        /// <summary>
        /// Register of all activ connectrions to a server
        /// </summary>
        private static readonly Dictionary<Guid, ClientSocket> ConnectionRegister = new();
        /// <summary>
        /// Register of all registerd command-executer with the main command
        /// </summary>
        private static readonly Dictionary<string, IConsoleCommand> CommandRegister = new();
        /// <summary>
        /// Register of all alieses of a command
        /// </summary>
        private static readonly Dictionary<string, string> AliasesRegister = new();
        /// <summary>
        /// Current activ Connction ID 
        /// </summary>
        private static Guid? currentConnection = null; // TODO: Better naming
        /// <summary>
        /// Current activ Connction ID 
        /// </summary>
        internal static Guid? CurrentConnection { get => currentConnection; }

        // TODO: 'REAL' CurrentConnection

        /// <summary>
        /// Client of the current activ connection or null
        /// </summary>
        internal static Client? CurrentClient
        {
            get
            {
                if (CurrentConnection != null)
                    return UserRegister[(Guid)CurrentConnection];
                return null;
            }
        }

        /// <summary>
        /// Folder in which the saves will be saved and loaded
        /// </summary>
        public static readonly string SaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Voice_Of_Time");

        /// <summary>
        /// WORKAROUND. Get all nessary parts of the current connection
        /// </summary>
        /// <returns>All importend parts of the current Connection</returns>
        /// <exception cref="Exception">No connection is currently active</exception>
        internal static (ClientSocket, Client, Guid) GetCurrentConnection()
        {            
            var serverID = currentConnection       ?? throw new Exception("No acctiv connection!");
            var socket   = GetConnection(serverID) ?? throw new Exception("Internal Error: No Connection to current Server!");
            var client   = CurrentClient           ?? throw new Exception("Internal Errir: No Client for current Servert!");

            return (socket, client, serverID);
        }

        #region CommandRegister
        /// <summary>
        /// List of all allowed Charactars of with a command and its aliases can consist of
        /// </summary>
        private const string AllowedChars = "abcdefghijklmnopqrstuvwxyz0123456789_";
        /// <summary>
        /// <para>Register a new command to the system.</para>
        /// <br>You shoud only set one Command to one executer or the stored data could be lost if the same command is called from another aliases of itself!</br>
        /// <br>Use aliases instead!</br>
        /// </summary>
        /// <param name="executer">A new instance of a IConsoleCommand</param>
        /// <returns></returns>
        /// <exception cref="EntryAlreadyExistsExeption">When command is already present</exception>
        /// <exception cref="UnauthorizedCharExeption">When a command contains a unauthoried char</exception>
        public static void RegisterCommand(IConsoleCommand executer)
        {
            // For later equals ignor case checking
            var command = executer.Command;
            var aliases = executer.Aliases;
            // Convert to case insensitiv
            command = command.ToLower();
            aliases = aliases.Select(x => x.ToLower()).ToArray();
            // Check for unauthericest chars
            if (!command.All(c => AllowedChars.Contains(c)))
            {
                throw new UnauthorizedCharExeption($"\"{command}\" has a not allowed char in it!\nAllowed Chars: \"{AllowedChars}\"");
            }
            foreach (var alias in aliases)
            {
                if (!alias.All(c => AllowedChars.Contains(c)))
                {
                    throw new UnauthorizedCharExeption($"\"{alias}\" has a not allowed char in it!\nAllowed Chars: \"{AllowedChars}\"");
                }
            }
            // Add command to the aliases for easyer use later
            aliases = aliases.Append(command).ToArray();
            // Remove all duplicats
            aliases = aliases.Distinct().ToArray();
            // Check if their is any colidataion
            lock (CommandRegister)
            {
                if (CommandRegister.ContainsKey(command))
                {
                    throw new EntryAlreadyExistsExeption($"Command already known: {command}");
                }
            }
            lock (AliasesRegister)
            {
                foreach (var alias in aliases)
                {
                    if (AliasesRegister.ContainsKey(alias))
                    {
                        throw new EntryAlreadyExistsExeption($"Command already known: {alias}");
                    }
                }
            }
            // Add Command to register
            lock (CommandRegister)
            {
                CommandRegister.Add(command, executer);
            }
            lock (AliasesRegister)
            {
                foreach (var alias in aliases)
                {
                    AliasesRegister.Add(alias, command);
                }
            }
        }

        /// <summary>
        /// <para>Try catch wrapper for <see cref="RegisterCommand"/></para>
        /// Only catches errors thrown from RegisterCommand
        /// </summary>
        /// <param name="executer">A new instance of a IConsoleCommand</param>
        /// <returns>Command was registered</returns>
        public static bool TryRegisterCommand(IConsoleCommand executer)
        {
            try
            {
                RegisterCommand(executer);
            }
            catch (EntryAlreadyExistsExeption) { return false; }
            catch (UnauthorizedCharExeption) { return false; }
            return true;
        }

        /// <summary>
        /// Get a command-executer bound to a command
        /// </summary>
        /// <param name="command">command or alias of command-executer</param>
        /// <returns>command-executer if exists</returns>
        public static IConsoleCommand? GetCommandExecuter(string command)
        {
            command = command.ToLower();
            var realCommand = AliasesRegister.GetValueOrDefault(command);
            if (realCommand is null) return null;
            return CommandRegister[realCommand];
        }

        /// <summary>
        /// Get the top command call of a command over its aliases or the command
        /// </summary>
        /// <param name="command">alias or the command</param>
        /// <returns>top command call</returns>
        public static string? GetAliaseParent(string command)
        {
            command = command.ToLower();
            return AliasesRegister.GetValueOrDefault(command); ;
        }

        /// <summary>
        /// Get a list of all command-execuzter
        /// </summary>
        /// <returns>list of all command-execuzter</returns>
        public static List<IConsoleCommand> GetAllComands()
        {
            return CommandRegister.Values.ToList();
        }
        #endregion

        #region UserRegister
        /// <summary>
        /// Get a Client instance of a spezific server if exists
        /// </summary>
        /// <param name="serverID">UID of server</param>
        /// <returns>Client if exists</returns>
        internal static Client? GetServerCient(Guid serverID)
        {
            // Check if server is known
            if (!UserRegister.ContainsKey(serverID))
            {
                // if not known try to load from save files
                var serverInstace = LoadData(serverID);
                if (serverInstace is not null)
                {
                    UserRegister[serverID] = serverInstace;
                }
            }
            return UserRegister.GetValueOrDefault(serverID);
        }

        /// <summary>
        /// Add a client to the ClientRegister
        /// </summary>
        /// <param name="client">Client of server</param>
        /// <param name="serverID">UID of server</param>
        /// <exception cref="EntryAlreadyExistsExeption">If a client is already present for the server</exception>
        internal static void AddServerClient(Client client, Guid serverID)
        {
            if (UserRegister.ContainsKey(serverID))
            {
                throw new EntryAlreadyExistsExeption();
            }

            UserRegister.Add(serverID, client);
        }

        /// <summary>
        /// Try-catch wrapper for <see cref="AddServerClient"/>
        /// </summary>
        /// <param name="client">Client of server</param>
        /// <param name="serverID">UID of server</param>
        /// <returns>Client was added</returns>
        internal static bool TryAddServerClient(Client client, Guid serverID)
        {
            try
            {
                AddServerClient(client, serverID);
            }
            catch (EntryAlreadyExistsExeption) { return false; }
            return true;
        }
        #endregion

        #region ConnectionRegister
        /// <summary>
        /// Add a connection to the connection register
        /// </summary>
        /// <param name="serverID">UID of server</param>
        /// <param name="socket">Scoket of connection</param>
        /// <param name="isCurrentConnection">Set the new connection as current connection</param>
        /// <exception cref="EntryAlreadyExistsExeption">Connection is already present</exception>
        internal static void AddConnection(Guid serverID, ClientSocket socket, bool isCurrentConnection = true)
        {
            if (ConnectionRegister.ContainsKey(serverID)) throw new EntryAlreadyExistsExeption();
            ConnectionRegister.Add(serverID, socket);

            if (isCurrentConnection)
            {
                currentConnection = serverID;
            }
        }

        /// <summary>
        /// Get a connection, if active, over its server ID
        /// </summary>
        /// <param name="serverID">UID of server</param>
        /// <returns>Connection to server</returns>
        internal static ClientSocket? GetConnection(Guid serverID)
        {
            return ConnectionRegister.GetValueOrDefault(serverID);
        }

        /// <summary>
        /// Get a list of all currently open Connections
        /// </summary>
        /// <returns>list of all currently open Connections</returns>
        internal static List<ClientSocket> GetAllConnectionSockets()
        {
            return ConnectionRegister.Values.ToList();
        }
        
        /// <summary>
        /// Get all Server IDs of all open connections
        /// </summary>
        /// <returns>list of all open server IDs</returns>
        internal static List<Guid> GetAllConnectionIDs()
        {
            return ConnectionRegister.Keys.ToList();
        }

        /// <summary>
        /// Close a connection to a spezific server
        /// </summary>
        /// <param name="serverID">UID of server</param>
        internal static void CloseConnection(Guid serverID)
        {
            if (!ConnectionRegister.ContainsKey(serverID)) return;

            var connection = ConnectionRegister[serverID];
            connection.Dispose();
            ConnectionRegister.Remove(serverID);

            if (currentConnection.Equals(serverID))
            {
                currentConnection = null;
            }
        }

        /// <summary>
        /// Get a copy of the connection register
        /// </summary>
        /// <returns>copy of the connection register</returns>
        internal static Dictionary<Guid, ClientSocket> GetConnectionRegisterCopy()
        {
            return new Dictionary<Guid, ClientSocket>(ConnectionRegister);
        }

        /// <summary>
        /// Make anouther connection to the current connection
        /// </summary>
        /// <param name="serverID">UID of server</param>
        /// <returns>Current connection was changed</returns>
        internal static bool SelectConnection(Guid serverID)
        {
            var entryExists = ConnectionRegister.ContainsKey(serverID);
            if (entryExists) currentConnection = serverID;
            return entryExists;
        }
        #endregion

        #region Save & Load
        /// <summary>
        /// Save all savaable Data to files
        /// </summary>
        public static void SaveData() // TODO: Encryption
        {
            // Create the folder if not exists
            Directory.CreateDirectory(SaveFolder);

            foreach (var server in UserRegister)
            {
                var saveFile = Path.Combine(SaveFolder, server.Key.ToString());

                // Move the current savefile of the client to a backup to avoid data smashing and as possible short backup
                if(File.Exists(saveFile))
                {
                    if(File.Exists(saveFile + ".BAK"))
                    {
                        File.Delete(saveFile + ".BAK");
                    }
                    File.Move(saveFile, saveFile + ".BAK");
                }

                using FileStream stream = File.OpenWrite(saveFile);

                DataContractSerializer formatter = new(typeof(Client));

                formatter.WriteObject(stream, server.Value);

                stream.Close();
            }
        }

        /// <summary>
        /// Load the client of a spezific Server if exists
        /// </summary>
        /// <param name="serverID">UID of Server</param>
        /// <returns>client if exists</returns>
        private static Client? LoadData(Guid serverID) //TODO: Decryption
        {
            var saveFile = Path.Combine(SaveFolder, serverID.ToString());

            if (!File.Exists(saveFile)) return null;

            using FileStream stream = File.OpenRead(saveFile);

            DataContractSerializer formatter = new(typeof(Client));

            Client? client = (Client?)formatter.ReadObject(stream);

            stream.Close();

            return client;
        }
        #endregion
    }
}
