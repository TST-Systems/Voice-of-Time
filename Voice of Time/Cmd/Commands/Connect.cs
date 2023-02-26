using Voice_of_Time.Transfer;
using VoTCore.Controll;
using System.Security.Cryptography;
using Voice_of_Time.Shared;
using Voice_of_Time.User;
using Voice_of_Time.Shared.Functions;

/**
 * @author      - Timeplex
 * 
 * @created     - 27.01.2023
 * 
 * @last_change - 12.02.2023
 */
namespace Voice_of_Time.Cmd.Commands
{
    /// <summary>
    /// Command to connect to a server and automicly log in or register
    /// </summary>
    internal class Connect : ICommandHelp, IConsoleCommandAsync
    {
        #region Public info
        public string[] CommandHelp => throw new NotImplementedException(); // TODO

        public string Command => "connect";

        private readonly string[] aliases = { "c", "con" };
        public string[] Aliases     => aliases;

        public string Usage         => "connect <IP/Domain> [Port def:15050]";
        #endregion

        async public Task<bool> ExecuteCommand(string command, string[] args)
        {
            try
            {
                if (args.Length < 1) return false;

                // Connection to server
                int port = args.Length > 1 ? Int32.Parse(args[1]) : 15050;
                Console.Write($"Connection to Server: {args[0]}:{port} ...");
                ClientSocket socket = new(args[0], port);
                socket.StartHandler();
                Console.WriteLine("done");

                // Getting server Identity
                Console.Write("Getting ServerID...");
                Guid serverID  = await Requests.RequestServerID(socket);
                Console.WriteLine("done");
                Console.WriteLine($"ServerID: {serverID}");

                //Checking if a connection is already open
                var connectionOpen = ClientData.GetConnection(serverID);
                if(connectionOpen != null)
                {
                    Console.WriteLine("A Connection is already open!");
                    Console.Write("Closing the new connection...");
                    socket.Dispose();
                    Console.WriteLine("done");
                    return true;
                }

                // Checking if server is known
                Client? client = ClientData.GetServerCient(serverID);
                if (client == null) // If not Register
                {
                    Console.WriteLine("Server unknown. Trying to Register");

                    // Generate a new RSA key pair
                    Console.Write("Generating Key... ");
                    var clientKey = RSA.Create();
                    Console.WriteLine("done");

                    // Execute a key exchange
                    Console.Write("Exchange public key with Server...");
                    var serverKey = await Requests.KeyExchangeWithServer(socket, clientKey);
                    Console.WriteLine("done");

                    // Open a Secure communication
                    Console.Write("Open a safe communication...");
                    await Requests.OpenSecureCommunication(socket, clientKey);
                    Console.WriteLine("done");

                    // Register User on Server
                    Console.Write("Getting UserID...");
                    var userID = await Requests.RegisterClient(socket);
                    Console.WriteLine("done");

                    // Getting username
                    Console.WriteLine();
                    Console.WriteLine("Please Enter your username:");
                    string? username;
                    do
                    {
                        Console.Write("Username: ");
                        username = Console.ReadLine();
                    } while (username is null or "");


                    // Setting username
                    Console.Write("Setting username...");
                    await Requests.SetUsername(socket, username, userID);
                    Console.WriteLine("done");

                    // Saving Data of connection
                    Console.WriteLine("Setting up Profile:");
                    Console.WriteLine($"Username  : {username}");
                    Console.WriteLine($"UserID    : {userID}  ");
                    Console.WriteLine($"UserKey   : [privat]  ");
                    Console.Write($"ServerKey : ");
                    var sKey = serverKey.ExportParameters(false).Modulus;
                    if (sKey is null) Console.WriteLine("Error");
                    else Console.WriteLine(Convert.ToBase64String(sKey));

                    client = new(userID, username, clientKey);
                    client.UserDB.Add(0, new(0, "Server", serverKey));

                    ClientData.AddServerClient(client, serverID);
                }
                else // Else verify
                {
                    Console.WriteLine("Server known...Trying to log in");
                    Console.Write("Verifiying...");
                    var userIsKnown = await Requests.ValidateSelf(socket, client);
                    if (!userIsKnown)
                    {
                        Console.WriteLine("error");
                        Console.WriteLine("You are not known by the Server!");
                        socket.Dispose();
                        return false;
                    }
                    Console.WriteLine("done");
                    Console.Write("Testing encryption...");
                    await Requests.TestConnection(socket, serverID, client.UserID);
                    Console.WriteLine("done");
                }
                // Register the connection
                ClientData.AddConnection(serverID, socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("An Error ocured!");
                Console.WriteLine(ex.Message);
                return false;
            }

            ClientData.SaveData();

            return true;
        }
    }
}
