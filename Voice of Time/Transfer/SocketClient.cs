using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Voice_of_Time.Transfer
{
    internal class SocketClient
    {
        //source: 
        //https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/sockets/tcp-classes
        //https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/sockets/socket-services

        public static async Task SetStreamAsync(String message)
        {

            IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("www.timeliners.org");
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            IPEndPoint ipEndPoint = new(ipAddress, 11_000);

            using Socket client = new(
            ipEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

            await client.ConnectAsync(ipEndPoint);
            while (true)
            {
                // Send message.
                var messageBytes = Encoding.UTF8.GetBytes(message);
                _ = await client.SendAsync(messageBytes, SocketFlags.None);

                // Receive ack.
                var buffer = new byte[1_024];
                var received = await client.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);
                if (response == "<|ACK|>")
                {
                    break;
                }
            }

            client.Shutdown(SocketShutdown.Both);
        }

    }
}
