using Voice_of_Time.Cmd;
//✔️❌

// Currently only starting Console command input
await new CommandHandler().Enable();

/*
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
*/