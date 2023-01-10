using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Voice_of_Time.Transfer
{
    internal class CSocketSingle
    {
        private protected IPHostEntry IpHostInfo { get; set; }
        private protected IPAddress   IpAddress  { get; set; }
        private protected IPEndPoint  IpEndPoint { get; set; }

        internal CSocketSingle(string address, int port)
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

            message += Constants.EOM;

            await client.ConnectAsync(IpEndPoint);
            // Send message.
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var code = await client.SendAsync(messageBytes, SocketFlags.None);
            // Recive answer
            var buffer = new byte[Constants.BUFFER_SIZE_BYTE];
            var received = await client.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, received);
            var fin_byte = Encoding.UTF8.GetBytes(Constants.FIN.ToString());
            var fin_code = await client.SendAsync(fin_byte, SocketFlags.None);
            client.Close();
            return response + response;
        }

    }
}
