using Voice_of_Time.Transfer;

var clientSocket = new SocketClient("192.168.178.34", 15050);

await clientSocket.SetStreamAsync("Hey NA!");

Console.ReadLine();