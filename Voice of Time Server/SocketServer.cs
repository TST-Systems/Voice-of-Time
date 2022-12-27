using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Voice_of_Time.Transfer
{
    internal class SocketServer
    {
        public static async Task Main()
        {
            var endPoint = new IPEndPoint(IPAddress.Loopback, 5000);
            using var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            using Socket listener = new(
            endPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

            listener.Bind(endPoint);
            listener.Listen(100);

            var handler = await listener.AcceptAsync();
            while (true)
            {
                // Receive message.
                var buffer = new byte[1_024];
                var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);

                var eom = "<|EOM|>";
                if (response.IndexOf(eom) > -1 /* is end of message */)
                {
                    var ackMessage = "<|ACK|>";
                    var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
                    await handler.SendAsync(echoBytes, 0);
                    break;
                }
            }
        }



    }
}
