using Voice_of_Time.Cmd;
//✔️❌

await new CommandHandler().Enable();

/*
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
*/