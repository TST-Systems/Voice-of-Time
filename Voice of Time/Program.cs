using Voice_of_Time.Transfer;

CSocketHold? socket = null;

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    await processCommand(input);
}

async Task processCommand(string? str)
{
    if (str is null or "") {
        Console.WriteLine("Command List: > help"); 
        return; 
    }

    var commandSplit = str.Split(' ');

    var command = commandSplit[0];

    switch (command.ToLower())
    {
        case "help":
            showHelp();
            break;
        case "connect":
            if(commandSplit.Length > 1)
            {
                await connect(commandSplit[1], commandSplit.Length > 2 ? Int32.Parse(commandSplit[2]) : 15050);
            }
            break;
        case "disconnect":
            disconnect();
            break;
    }
}

void disconnect()
{
    if(socket is null)
    {
        Console.WriteLine("Connection coudn't be closed! No connection active!");
        return;
    }
    var succsess = socket.Disconect();
    if (succsess)
    {
        Console.WriteLine("Successfully disconnected!");
    }else
    {
        Console.WriteLine("Error!");
    }

}

void showHelp()
{
    Console.WriteLine("*************** List of all commands ***************");
    Console.WriteLine("connect [ip] {[port]} - connect to a server         ");
    Console.WriteLine("disconnect            - disconnect from the server  ");
    Console.WriteLine("send [message]        - send a message to the server");
}

async Task connect(string host, int port = 15050)
{
    socket      = new(host, port);
    Console.WriteLine("Trying to connect to: " + $"{host}:{port}");
    var success = await socket.AutoStart();
    Console.WriteLine($"Connection was established: {success}");
}