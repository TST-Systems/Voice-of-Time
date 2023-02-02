using Voice_of_Time.Transfer;
using VoTCore.Controll;
using VoTCore.Package.Header;
using VoTCore.Package.SData;
using VoTCore.Package;
using System.Security.Cryptography;
using VoTCore.Package.SecData;

/**
 * @author      - Timeplex
 * 
 * @created     - 27.01.2023
 * 
 * @last_change - 01.02.2023
 */
namespace Voice_of_Time.Cmd.Commands
{
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
                CSocketHold socket = new(args[0], port);
                await socket.AutoStart();
                Console.WriteLine("done");

                // Getting server Identity
                Console.Write("Getting ServerID...");
                Guid serverID  = await RequestServerID(socket);
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
                    var serverKey = await KeyExchangeWithServer(socket, clientKey);
                    Console.WriteLine("done");

                    // Open a Secure communication
                    Console.Write("Open a safe communication...");
                    await OpenSecureCommunication(socket, clientKey);
                    Console.WriteLine("done");

                    // Register User on Server
                    Console.Write("Getting UserID...");
                    var userID = await RegisterClient(socket);
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
                    await SetUsername(socket, username, userID);
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
                    client.UserDB.Add(0, new(0, "Server", new(serverKey)));

                    ClientData.AddServerClient(client, serverID);
                }
                else // Else verify
                {
                    Console.WriteLine("Server known...Trying to log in");
                    Console.Write("Verifiying...");
                    var userIsKnown = await ValidateSelf(socket, client);
                    if (!userIsKnown)
                    {
                        Console.WriteLine("error");
                        Console.WriteLine("You are not known by the Server!");
                        socket.Dispose();
                        return false;
                    }
                    Console.WriteLine("done");
                    Console.Write("Testing encryption...");
                    await TestConnection(socket, serverID, client.UserID);
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

            return true;
        }

        protected static async Task<Guid> RequestServerID(CSocketHold socket, long userID = -1)
        {
            if (socket is null) throw new Exception("no connection open");

            var header  = new HeaderReq(userID, RequestType.IDENTITY);
            var toSend  = new VOTP(header).Serialize();
            var recived = await socket.EnqueueItem(toSend);
            var body    = new VOTP(recived).Body;

            if (body is not SData_Guid sDGuid) throw new Exception("Sever didn't replyed correctly");

            return sDGuid.Data;
        }

        protected static async Task<RSA> KeyExchangeWithServer(CSocketHold socket, RSA key, long userID = -1)
        {
            var header = new HeaderReq(userID, RequestType.KEY_EXCHANGE, 0);
            var body   = new SecData_Key_RSA(key, userID);
            var toSend = new VOTP(header, body);
            var recive = await socket.EnqueueItem(toSend.Serialize());
            var result = new VOTP(recive);

            if (result.Header is not HeaderAck resHeader)   throw new Exception("Header wasn't expected!");
            if (result.Body is not SecData_Key_RSA resBody) throw new Exception("Body wasn't expected!");

            if (!resHeader.Successful) throw new Exception("Server didn't responded correctly");

            return resBody.GetKey();
        }

        protected static async Task OpenSecureCommunication(CSocketHold socket, RSA decryptionKey, long userID = -1)
        {
            var header = new HeaderReq(userID, RequestType.COMM_KEY);
            var toSend = new VOTP(header);
            var recive = await socket.EnqueueItem(toSend.Serialize());
            var result = new VOTP(recive);

            if (result.Header is not HeaderAck resHeader) throw new Exception("Header wasn't expected!");
            if (result.Body is not SecData_Key_Aes resBody) throw new Exception("Body wasn't expected!");

            if (!resHeader.Successful) throw new Exception("Server didn't responded correctly");

            resBody.DecryptData(decryptionKey);

            var key = resBody.GetKey();

            socket.SetCommunicationKey(key);
        }

        protected static async Task<long> RegisterClient(CSocketHold socket)
        {
            var header = new HeaderReq(-1, RequestType.REGISTRATION);
            var toSend = new VOTP(header);
            var recive = await socket.EnqueueItem(toSend.Serialize());
            var result = new VOTP(recive);

            if (result.Header is not HeaderAck resHeader) throw new Exception("Header wasn't expected!");
            if (result.Body is not SData_Long resBody) throw new Exception("Body wasn't expected!");

            if (!resHeader.Successful) throw new Exception("Server didn't responded correctly");

            return resBody.Data;
        }

        protected async static Task SetUsername(CSocketHold socket, string username, long userID)
        {
            var header = new HeaderReq(userID, RequestType.SET_USERNAME);
            var body = new SData_String(username);
            var toSend = new VOTP(header, body);
            var recive = await socket.EnqueueItem(toSend.Serialize());
            var result = new VOTP(recive);

            if (result.Header is not HeaderAck resHeader) throw new Exception("Header wasn't expected!");

            if (!resHeader.Successful) throw new Exception("Server didn't responded correctly");
        }

        protected static async Task TestConnection(CSocketHold socket, Guid serverID, long userID)
        {
            var newServerID = await RequestServerID(socket, userID);

            if (serverID.CompareTo(newServerID) != 0) throw new Exception("Connection Invalid!");
        }

        protected static async Task<bool> ValidateSelf(CSocketHold socket, Client c)
        {
            var head = new HeaderReq(c.UserID, RequestType.VERIFY);
            var body = new SData_Long(c.UserID);
            var package = new VOTP(head, body);

            var result = new VOTP(await socket.EnqueueItem(package.Serialize()));

            if (result.Header is not HeaderAck resHeader) throw new Exception("Header wasn't expected!");
            if (result.Body is not SecData_Key_Aes resBody) throw new Exception("Body wasn't expected!");

            if (!resHeader.Successful) return false;

            resBody.DecryptData(c.UserKey);

            var key = resBody.GetKey();

            socket.SetCommunicationKey(key);

            return true;
        }
    }
}
