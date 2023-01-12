using System.Text;
using Voice_of_Time_Server.Transfer;

var serverSocket = new SocketServer(15050, (msg) => returnMessage(msg));
_ = serverSocket.StartListining();

Console.ReadLine();




string returnMessage(string msg)
{
    var anserw = "";
    for (var i = msg.Length - 1; i >= 0; i--)
    {
        anserw += msg[i];
    }
    return anserw;
}
