using Voice_of_Time.Transfer;
/*

int done = 0;
object synDone = new();
int err = 0;
object synErr = new();

Random rnd = new();
int proc = 0;

*/
var clientSocket = new CSocketHold("84.144.245.119", 15050);

var a = await clientSocket.AutoStart();
Console.WriteLine(a);
Console.WriteLine(clientSocket.CurrentState);

//var id1 = clientSocket.EnqueueItem("1 .rN", (msg) => { Console.WriteLine(msg); return Task.CompletedTask; });
//var id2 = clientSocket.EnqueueItem("2 .rN", async (msg) => { await Task.Delay(1000); Console.WriteLine(msg); });
//var id3 = clientSocket.EnqueueItem("3 .rN", (msg) => { Console.WriteLine(msg); return Task.CompletedTask; });

Console.ReadLine();

Console.WriteLine(await clientSocket.EnqueueItem("4 .rN"));

Console.ReadLine();

clientSocket.Dispose();



//List<List<bool>> WaitXTSuccess = new();

/*

string rdmString = "";

for(int i = 0; i < 6_000; i++)
{
    rdmString += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
}



await doStuff2(rdmString);


*/


/*
for(int j = 10; j > 0; j--)
{
    var list = new List<bool>();
    WaitXTSuccess.Add(list); 
    Console.WriteLine("*********************************");
    Console.WriteLine("Delay = " + (10 * j));
    for (int i = 0; i < 250; i++)
    {
        _ = doStuff(i, list);
        await Task.Delay(10 * j);
    }
}
*/
/*
for(long waitTick = 500_000; waitTick >= 0; waitTick -= 50_000)
{
    Console.WriteLine("Ticktime: " + waitTick);
    Task action = Task.CompletedTask;
    var IT = 0;
    for (int i = 0; i < 1000; i++)
    {
        IT = (proc + 1 - done - err);
        action = doStuff2(i);
        proc = i;
        await Task.Delay(new TimeSpan((long)waitTick));
        print();
    }
    await action;
    print();
    done = 0;
    err = 0;
    Console.WriteLine();
    Console.WriteLine("IT: " + IT);
}
*/

/*
Func<Task> loopPrint = async () => {
    while (true)
    {
        print();
        await Task.Delay(1000/144);
    }
};

_ = loopPrint();
*/
/*
async Task doStuff(int i, List<bool> list)
{
    list.Add(false);
    try
    {
        var echo = await clientSocket.StreamAsync(i.ToString());
        Console.WriteLine(echo + " - Done!");
        list[i] = true;
    }
    catch (Exception ex) { Console.WriteLine(i + ": " + ex.Message); };
}*/
/*
void print()
{
    Console.Write("\rC: " + (proc + 1) + " | D: " + done + " | F: " + err + " | IT: " + (proc + 1 - done - err) + "");
}

async Task doStuff2(string i)
{
    try
    {
        var echo = clientSocket.StreamAsync(i);
        lock (synDone)
        {
            done++;
        }
        Console.WriteLine(echo.Result);
    }
    catch (Exception)
    {
        lock (synErr)
        {
            err++;
        }
    };
}
*/
Console.ReadLine();