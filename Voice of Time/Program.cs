using Voice_of_Time.Transfer;

var clientSocket = new SocketClient("127.0.0.1", 15050);

await clientSocket.SetStreamAsync("Hey NA!");

Console.ReadLine();