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
    /// <summary>
    /// <para>Only one Socket Server per server! At least on the same port ;)</para>
    /// Connection managment class for incoming connections
    /// </summary>
    internal class SocketServer
    {
        private readonly TcpListener Listener;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="port">Any port number witch is not already taken by any other process</param>
        public SocketServer(int port)
        {
            Listener = new(IPAddress.Any, port);
        }

        /// <summary>
        /// Start listing for new connections 
        /// </summary>
        public async Task StartListining()
        {
            Listener.Start();
            while (true)
            {
                // wait for new connection
                var socket = await Listener.AcceptTcpClientAsync();
                // start a new Handler for the connection
                _ = new SocketHandler(socket); // TODO: Start new Thread
            }
        }

        // TODO : Stop

        // TODO : Store all Handlers in a list or a Dictonary for server sided triggers
    }


}
