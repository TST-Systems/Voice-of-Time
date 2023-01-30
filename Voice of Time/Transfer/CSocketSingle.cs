using System.Net;
using System.Net.Sockets;
using System.Text;

/**
 * @author      - Timeplex
 * 
 * @created     - 09.01.2023
 * 
 * @last_change - 11.01.2023
 */
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

        [Obsolete("Outdated! Please use CSocketHold.EnqueItem instead.")]
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
            bool messageComplete = false;
            string IncomingMessage = "";
            // RECIVE
            while (!messageComplete)
            {
                var buffer = new byte[Constants.BUFFER_SIZE_BYTE];
                var received = await client.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);

                var indexOfEOM = response.IndexOf(Constants.EOM);
                if (indexOfEOM > -1)
                {
                    messageComplete = true;
                    response = response.Remove(indexOfEOM);
                }
                /*
                else if (received < Constants.BUFFER_SIZE_BYTE)
                {
                    throw new Exception("End of message was not resived!");
                }*/

                IncomingMessage += response;
            }
            var fin_byte = Encoding.UTF8.GetBytes(Constants.FIN.ToString());
            var fin_code = await client.SendAsync(fin_byte, SocketFlags.None);
            client.Close();
            return IncomingMessage;
        }

    }
}
