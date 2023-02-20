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
    [Serializable]
    public static class ClientData
    {
        private static readonly Dictionary<Guid, Client> UserRegister = new();


        private static readonly Dictionary<Guid, ClientSocket> ConnectionRegister = new();

        private static readonly Dictionary<string, IConsoleCommand> CommandRegister = new();
        private static readonly Dictionary<string, string> AliasesRegister = new();

        private static Guid? currentConnection = null;
        internal static Guid? CurrentConnection { get => currentConnection; }

        internal static Client? CurrentClient
        {
            get
            {
                if (CurrentConnection != null)
                    return UserRegister[(Guid)CurrentConnection];
                return null;
            }
        }

        public static readonly string SaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Voice_Of_Time");

        internal static (ClientSocket, Client, Guid) GetCurrentConnection()
        {            
            var serverID = currentConnection       ?? throw new Exception("No acctiv connection!");
            var socket   = GetConnection(serverID) ?? throw new Exception("Internal Error: No Connection to current Server!");
            var client   = CurrentClient           ?? throw new Exception("Internal Errir: No Client for current Servert!");

            return (socket, client, serverID);
        }



        #region CommandRegister
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

        public static IConsoleCommand? GetCommandExecuter(string command)
        {
            command = command.ToLower();
            var realCommand = AliasesRegister.GetValueOrDefault(command);
            if (realCommand is null) return null;
            return CommandRegister[realCommand];
        }

        public static string? GetAliaseParent(string command)
        {
            command = command.ToLower();
            return AliasesRegister.GetValueOrDefault(command); ;
        }

        public static List<IConsoleCommand> GetAllComands()
        {
            return CommandRegister.Values.ToList();
        }
        #endregion

        #region UserRegister
        internal static Client? GetServerCient(Guid serverID)
        {
            if (!UserRegister.ContainsKey(serverID))
            {
                var serverInstace = LoadData(serverID);
                if (serverInstace is not null)
                {
                    UserRegister[serverID] = serverInstace;
                }
            }
            return UserRegister.GetValueOrDefault(serverID);
        }

        internal static void AddServerClient(Client client, Guid serverID)
        {
            if (UserRegister.ContainsKey(serverID))
            {
                throw new EntryAlreadyExistsExeption();
            }

            UserRegister.Add(serverID, client);
        }

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
        internal static void AddConnection(Guid serverID, ClientSocket socket, bool isCurrentConnection = true)
        {
            if (ConnectionRegister.ContainsKey(serverID)) throw new EntryAlreadyExistsExeption();
            ConnectionRegister.Add(serverID, socket);

            if (isCurrentConnection)
            {
                currentConnection = serverID;
            }
        }

        internal static ClientSocket? GetConnection(Guid serverID)
        {
            return ConnectionRegister.GetValueOrDefault(serverID);
        }

        internal static List<ClientSocket> GetAllConnectionSockets()
        {
            return ConnectionRegister.Values.ToList();
        }

        internal static List<Guid> GetAllConnectionIDs()
        {
            return ConnectionRegister.Keys.ToList();
        }

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

        internal static Dictionary<Guid, ClientSocket> GetConnectionRegisterCopy()
        {
            return new Dictionary<Guid, ClientSocket>(ConnectionRegister);
        }

        internal static bool SelectConnection(Guid serverID)
        {
            var entryExists = ConnectionRegister.ContainsKey(serverID);
            if (entryExists) currentConnection = serverID;
            return entryExists;
        }
        #endregion

        #region Save & Load
        public static void SaveData()
        {
            Directory.CreateDirectory(SaveFolder);

            foreach (var server in UserRegister)
            {
                var saveFile = Path.Combine(SaveFolder, server.Key.ToString());

                using FileStream stream = File.OpenWrite(saveFile);

                DataContractSerializer formatter = new(typeof(Client));

                formatter.WriteObject(stream, server.Value);

                stream.Close();
            }
        }

        private static Client? LoadData(Guid serverID)
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