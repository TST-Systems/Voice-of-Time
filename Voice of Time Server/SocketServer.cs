using System;
using System.Collections.Generic;
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
        private String? message;
        

        public static void Main()
        {
            var sockServer = new SocketServer();
            Task task1 = sockServer.ListenLine();

            while (true) ;
        }

        //Set and Geter method to change and output message 
        //If not required, it can also be deleted
        public String GetMessage()
        {
            return this.message;
        }

        private void SetMessage(String? currentMessage)
        {
             this.message = currentMessage;
        }

        public async Task ListenLine()
        {
            var endPoint = new IPEndPoint(IPAddress.Loopback, 11_000);
            using var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            using Socket listener = new(
            endPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

            listener.Bind(endPoint);
            listener.Listen();

            var handler = await listener.AcceptAsync();
            while (true)
            {
                // Receive message.
                var buffer = new byte[1_024];
                var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);


                if (response.Length > 0)
                {
                    SetMessage(response);

                    //Show current message Value on Console
                    //Console.WriteLine(GetMessage());

                    //infinite execute of ListenLine
                    ListenLine();
                    break;
                }

            }

            

        }



    }
}
