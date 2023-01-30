using System.Net;
using System.Net.Sockets;

/**
 * @author      - Timeplex, SalzstangeManga
 * 
 * @created     - 09.01.2023
 * 
 * @last_change - 20.01.2023
 */
namespace Voice_of_Time_Server.Transfer
{
    internal class SocketServer
    {
        private IPEndPoint IpEndPoint { get; set; }
        private Socket Listener { get; set; }

        public SocketServer(int port)
        {
            IpEndPoint = new(IPAddress.Any, port);
            Listener = new(IpEndPoint.AddressFamily,
                             SocketType.Stream,
                             ProtocolType.Tcp);
        }

        public async Task StartListining()
        {
            Listener.Bind(IpEndPoint);
            Listener.Listen(1000);
            while (true)
            {
                var socket = await Listener.AcceptAsync();
                var handler = new SocketHandler(socket);
                _ = handler.HandleConnection();
            }
        }
    }


}
