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
        private int lastMessage;

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

        async Task Timeout(int sek)
        {
            await Task.Delay(sek * 1000);
            if (lastMessage < DateTime.Now.Millisecond - (sek * 1000))
            {

            }
            _ = Timeout(sek);
        }
    }


}
