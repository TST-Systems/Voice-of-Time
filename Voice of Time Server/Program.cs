using Voice_of_Time_Server.Shared;
using Voice_of_Time_Server.Transfer;
using Voice_of_Time_Server.User;

var server = new Server();
ServerData.server = server;

var serverSocket = new SocketServer(15050);
_ = serverSocket.StartListining();

Console.ReadLine();
