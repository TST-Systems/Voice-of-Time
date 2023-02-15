using Voice_of_Time_Server.Shared;
using Voice_of_Time_Server.Transfer;

ServerData.Initialize();

var serverSocket = new SocketServer(15050);
_ = serverSocket.StartListining();

Console.ReadLine();
