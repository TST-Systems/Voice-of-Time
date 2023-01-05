using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Voice_of_Time.Transfer
{
    internal class SocketServer
    {

        private IPEndPoint IpEndPoint { get; set; }
        private Socket Listener { get; set; }

        private Func<SocketMessage, Task> Function { get; }

        public SocketServer (int port, Func<SocketMessage, Task> func)
        {
            IpEndPoint = new(IPAddress.Any, port);
            Listener   = new(IpEndPoint.AddressFamily,
                             SocketType.Stream,
                             ProtocolType.Tcp);
            Function = func;
            Console.WriteLine("Init");
        }

        public async Task StartListining()
        {
            Console.WriteLine("Start Listining");
            Listener.Bind(IpEndPoint);
            Listener.Listen(100);
            while (true)
            {
                Console.WriteLine("Start new listining");
                var message = ListenNext();
                SocketMessage msg = await message;
                _ = Function(msg);
            }
        }



        public async Task<SocketMessage> ListenNext()
        {
            var handler = await Listener.AcceptAsync();

            Console.WriteLine("Message Incoming");
            var buffer = new byte[33_554_432];
            var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, received);
            Console.WriteLine("Message had read!");

            return new(handler, response);
        }
    }

    internal readonly struct SocketMessage
    {
        public readonly Socket? Socket;
        public readonly string  Message;
        public SocketMessage(Socket? socket, string message)
        {
            Socket  = socket;
            Message = message;
        }
    }


}
