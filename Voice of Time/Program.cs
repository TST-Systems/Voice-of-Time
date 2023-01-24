using System.Security.Cryptography;
using System.Text;
using Voice_of_Time;
using Voice_of_Time.Transfer;
using VoTCore.Communication;
using VoTCore.Package;
using VoTCore.Package.Header;
using VoTCore.Package.SData;
using VoTCore.Package.SecData;
//✔️❌

Dictionary<Guid, Client> userRegister = new();

CSocketHold? socket = null;
Client? currentClient = null;

while (true)
{
    Console.Write(" > ");
    var input = Console.ReadLine();
    Console.WriteLine();
    await processCommand(input);
    Console.WriteLine();
}

async Task processCommand(string? str)
{
    if (str is null or "")
    {
        Console.WriteLine("Command List: > help");
        return;
    }

    var commandSplit = str.Split(' ');
    commandSplit = commandSplit.Where(x => x != "").ToArray();

    var command = commandSplit[0];

    switch (command.ToLower())
    {
        case "help":
            showHelp();
            break;
        case "connect":
        case "c":
            if (commandSplit.Length > 1)
            {
                await connect(commandSplit[1], commandSplit.Length > 2 ? Int32.Parse(commandSplit[2]) : 15050);
            }
            break;
        case "disconnect":
        case "d":
            disconnect();
            break;
        case "send":
            Console.WriteLine("Currently out of service!");
            break;
        //if (commandSplit.Length < 3) break;
        //string message = TrimMessage(commandSplit);
        //await send(int.Parse(commandSplit[1]), message);
        //break;
        case "exit":
        case "quit":
            if (socket is not null) disconnect();
            Environment.Exit(0);
            break;
    }
}

string TrimMessage(string?[] s)
{
    if (s.Length < 2) return "";
    string solution = "";
    for (int i = 2; i < s.Length; i++)
    {
        solution += s[i] + " ";
    }
    solution = solution.Remove(solution.Length - 1);
    return solution;
}

//async task send(int v, string message)
//{
//    if (socket is null) return;
//    console.writeline($"{message} -> {v}");
//    votp package = new(new headerstd(-1, v, 0, 0), new textmessage(message, -1, datetime.now.ticks));
//    var serial = package.serialize();
//    console.writeline(await socket.enqueueitem(serial));
//}

void disconnect()
{
    if (socket is null)
    {
        Console.WriteLine("Connection coudn't be closed! No connection active!");
        return;
    }
    var succsess = socket.Disconect();
    if (succsess)
    {
        Console.WriteLine("Successfully disconnected!");
    }
    else
    {
        Console.WriteLine("Error!");
    }
    socket.Dispose();
    socket = null;
}

void showHelp()
{
    Console.WriteLine("*************** List of all commands ***************");
    Console.WriteLine("connect [ip] {[port]} - connect to a server         ");
    Console.WriteLine("disconnect            - disconnect from the server  ");
    Console.WriteLine("send [to] [message]   - send a message to a target  ");
}

async Task connect(string host, int port = 15050)
{
    if (socket is not null) { Console.WriteLine("Connection already open! Reconnection..."); disconnect(); }
    socket = new(host, port);
    Console.WriteLine("Trying to connect to: " + $"{host}:{port}");
    var success = await socket.AutoStart();
    Console.WriteLine($"Connection was established: {success}");
    Console.Write("Getting Server Identity...");
    Guid serverID = await requestServerID();
    Console.WriteLine("done");
    Console.WriteLine($"ID: {serverID}");
    if (!userRegister.ContainsKey(serverID))
    {
        Console.WriteLine("Server unknown...Trying to Register");
        userRegister[serverID] = await RegisterClient();
        currentClient = userRegister[serverID];
    }
    else
    {
        Console.WriteLine("Server known...Trying to log in");
        currentClient = userRegister[serverID];
        Console.Write("Verifiying...");
        var userIsKnown = await ValidateSelf(currentClient);
        if (userIsKnown)
        {
            Console.WriteLine("done");
            Console.Write("Testing encryption...");
            await testConnection(serverID, currentClient.UserID);
            Console.WriteLine("done");
            return;
        }
        Console.WriteLine("error");
        Console.WriteLine("You are not known by the Server!");
        Console.Write("Try register? ([y]/n)");
        var anserw = Console.ReadLine();
        if (anserw is null) throw new Exception("Somthing went very wrong!");
        if (anserw.ToLower() == "n") { disconnect(); return; }
        _ = connect(host, port);
    }
}

async Task testConnection(Guid serverID, long userID)
{
    var newServerID = await requestServerID(userID);

    if (serverID.CompareTo(newServerID) != 0) throw new Exception("Connection Invalid!"); 
}

async Task<bool> ValidateSelf(Client c)
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

async Task<Client> RegisterClient()
{
    string? username;
    do
    {
        Console.Write("Username: ");
        username = Console.ReadLine();
    } while (username is null or "");

    Console.WriteLine("Trying to register");

    Console.Write("Generating Key... ");
    var clientKey = RSA.Create();
    Console.WriteLine("done");

    Console.Write("Exchange public key with Server...");
    var serverKey = await KeyExchangeWithServer(clientKey);
    Console.WriteLine("done");

    Console.Write("Open a safe communication...");
    await OpenSecureCommunication(clientKey);
    Console.WriteLine("done");

    Console.Write("Getting UserID...");
    var header = new HeaderReq(-1, RequestType.REGISTRATION);
    var toSend = new VOTP(header);
    var recive = await socket.EnqueueItem(toSend.Serialize());
    var result = new VOTP(recive);

    if (result.Header is not HeaderAck resHeader) throw new Exception("Header wasn't expected!");
    if (result.Body is not SData_Long resBody) throw new Exception("Body wasn't expected!");

    if (!resHeader.Successful) throw new Exception("Server didn't responded correctly");

    var userID = resBody.Data;
    Console.WriteLine("done");

    Console.Write("Setting username...");
    await SetUsername(username, userID);
    Console.WriteLine("done");

    Console.WriteLine("Setting up Profile:");
    Console.WriteLine($"Username  : {username}");
    Console.WriteLine($"UserID    : {userID}  ");
    Console.WriteLine($"UserKey   : [privat]  ");
    Console.Write($"ServerKey : ");
    var sKey = serverKey.ExportParameters(false).Modulus;
    if (sKey is null) Console.WriteLine("Error");
    else Console.WriteLine(Convert.ToBase64String(sKey));

    Client client = new(userID, username, clientKey);
    client.AddPublicKey(0, serverKey);

    return client;
}

async Task SetUsername(string username, long userID)
{
    var header = new HeaderReq(userID, RequestType.SET_USERNAME);
    var body = new SData_String(username);
    var toSend = new VOTP(header, body);
    var recive = await socket.EnqueueItem(toSend.Serialize());
    var result = new VOTP(recive);

    if (result.Header is not HeaderAck resHeader) throw new Exception("Header wasn't expected!");

    if (!resHeader.Successful) throw new Exception("Server didn't responded correctly");
}

async Task OpenSecureCommunication(RSA decryptionKey, long userID = -1)
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

async Task<RSA> KeyExchangeWithServer(RSA key, long userID = -1)
{
    var header = new HeaderReq(userID, RequestType.KEY_EXCHANGE, 0);
    var body = new SecData_Key_RSA(key, userID);
    var toSend = new VOTP(header, body);
    var recive = await socket.EnqueueItem(toSend.Serialize());
    var result = new VOTP(recive);

    if (result.Header is not HeaderAck resHeader) throw new Exception("Header wasn't expected!");
    if (result.Body is not SecData_Key_RSA resBody) throw new Exception("Body wasn't expected!");

    if (!resHeader.Successful) throw new Exception("Server didn't responded correctly");

    return resBody.GetKey();
}

async Task<Guid> requestServerID(long userID = -1)
{
    if (socket is null) throw new Exception("no connection open");

    var header = new HeaderReq(userID, RequestType.IDENTITY);
    var toSend = new VOTP(header).Serialize();
    var recived = await socket.EnqueueItem(toSend);
    var body = new VOTP(recived).Body;

    if (body is not SData_Guid sDGuid) throw new Exception("Sever didn't replyed correctly");

    return sDGuid.Data;
}