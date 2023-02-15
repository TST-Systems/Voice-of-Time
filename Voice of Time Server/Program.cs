using Voice_of_Time_Server.Transfer;

var serverSocket = new SocketServer(15050);
_ = serverSocket.StartListining();

Console.ReadLine();
