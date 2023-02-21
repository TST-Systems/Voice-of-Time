using System.Net;
using System.Net.Sockets;

/**
 * @author      - Timeplex, SalzstangeManga
 * 
 * @created     - 09.01.2023
 * 
 * @last_change - 12.02.2023
 */
namespace Voice_of_Time_Server.Transfer
{
    internal class SocketServer
    {
        private readonly TcpListener Listener;

        public SocketServer(int port)
        {
            Listener = new(IPAddress.Any, port);
        }

        public async Task StartListining()
        {
            Listener.Start();
            while (true)
            {
                var socket = await Listener.AcceptTcpClientAsync();
                _ = new SocketHandler(socket);
            }
        }
    }


}
