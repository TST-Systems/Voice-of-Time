using Voice_of_Time;
using Voice_of_Time.Transfer;
using VoTCore.Communication;
using VoTCore.Package;
using VoTCore.Package.Header;

Dictionary<Guid, Client> keyValuePairs= new();

CSocketHold? socket = null;

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
            if (commandSplit.Length > 1)
            {
                await connect(commandSplit[1], commandSplit.Length > 2 ? Int32.Parse(commandSplit[2]) : 15050);
            }
            break;
        case "disconnect":
            disconnect();
            break;
        case "send":
            if (commandSplit.Length < 3) break;
            string message = TrimMessage(commandSplit);
            await send(int.Parse(commandSplit[1]), message);
            break;
    }
}

string TrimMessage(string?[] s)
{
    if (s.Length < 2) return "";
    string solution = "";
    for(int i = 2; i < s.Length; i++)
    {
        solution += s[i] + " ";
    }
    solution = solution.Remove(solution.Length - 1);
    return solution;
}


async Task send(int v, string message)
{
    if (socket is null) return;
    Console.WriteLine($"{message} -> {v}");
    VOTP package = new(new HeaderStd(-1, v, 0, 0), new TextMessage(message, -1, DateTime.Now.Ticks));
    var serial = package.Serialize();
    Console.WriteLine(await socket.EnqueueItem(serial));
}

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
    if (socket is not null) {Console.WriteLine("Connection already open! Reconnection..."); disconnect(); }
    socket = new(host, port);
    Console.WriteLine("Trying to connect to: " + $"{host}:{port}");
    var success = await socket.AutoStart();
    Console.WriteLine($"Connection was established: {success}");
    Guid serverID = requestServerID();
}

Guid requestServerID()
{


    return Guid.NewGuid();
}