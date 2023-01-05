using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Voice_of_Time.Transfer
{
    internal class SocketClient
    {
        private IPHostEntry IpHostInfo { get; set; }
        private IPAddress   IpAddress  { get; set; }
        private IPEndPoint  IpEndPoint { get; set; }

        internal SocketClient(string address, int port)
        {
            IpHostInfo = Dns.GetHostEntry(address);
            IpAddress  = IpHostInfo.AddressList[0];
            IpEndPoint = new(IpAddress, port);
        }

        internal async Task<string?> StreamAsync(String message)
        {
            using Socket client = new(
            IpEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

            await client.ConnectAsync(IpEndPoint);

            // Send message.
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var code = await client.SendAsync(messageBytes, SocketFlags.None);
            client.Shutdown(SocketShutdown.Send);
            // Recive answer
            var buffer = new byte[33_554_432];
            var received = await client.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, received);
            client.Shutdown(SocketShutdown.Receive);

            return response;
        }


    }
}
