using System.Text;
using Voice_of_Time.Transfer;

var serverSocket = new SocketServer(15050, (msg) => returnMessage(msg));
_ = serverSocket.StartListining();

Console.ReadLine();




async Task returnMessage(SocketMessage msg)
{
    var Socket  = msg.Socket;
    var Message = msg.Message;
    if (Socket is null) return;
    var echoBytes = Encoding.UTF8.GetBytes(Message);
    await Socket.SendAsync(echoBytes, 0);
}