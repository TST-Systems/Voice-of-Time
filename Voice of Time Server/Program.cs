using System.Text;
using Voice_of_Time_Server.Transfer;

var serverSocket = new SocketServer(15050, (msg) => returnMessage(msg));
_ = serverSocket.StartListining();

Console.ReadLine();




string returnMessage(SocketMessage msg)
{
    var anserw = "";
    var Socket  = msg.Socket;
    var Message = msg.Message;
    if (Socket is null) throw new Exception("No Socket!");
    for (var i = Message.Length - 1; i >= 0; i--)
    {
        anserw += Message[i];
    }
    return anserw;
}
