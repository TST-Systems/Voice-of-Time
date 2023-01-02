using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Voice_of_Time.Transfer
{
    public class SocketClient
    {
        public async Task SetStreamAsync(String message)
        {
            //source: 
            //https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/sockets/tcp-classes
            //https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/sockets/socket-services


            //IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("www.timeliners.org");
            
            IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("127.0.0.1"); //localhost
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            IPEndPoint ipEndPoint = new(ipAddress, 11_000);

            using Socket client = new(
            ipEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

            await client.ConnectAsync(ipEndPoint);

            // Send message.
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var _ = await client.SendAsync(messageBytes, SocketFlags.None);

            client.Shutdown(SocketShutdown.Both);
        }


    }
}
