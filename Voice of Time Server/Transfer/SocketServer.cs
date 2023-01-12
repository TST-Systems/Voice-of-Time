using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using VoTCore;
using VoTCore.Communication;

namespace Voice_of_Time_Server.Transfer
{
    internal class SocketServer
    {
        private IPEndPoint IpEndPoint { get; set; }
        private Socket Listener { get; set; }

        private Func<string, string> Function { get; }

        public SocketServer(int port, Func<string, string> func)
        {
            IpEndPoint = new(IPAddress.Any, port);
            Listener = new(IpEndPoint.AddressFamily,
                             SocketType.Stream,
                             ProtocolType.Tcp);
            Function = func;
        }

        public async Task StartListining()
        {
            Listener.Bind(IpEndPoint);
            Listener.Listen(1000);
            while (true)
            {
                var handler = await Listener.AcceptAsync();
                _ = HandleConnection(handler);
            }
        }

        public async Task HandleConnection(Socket handler)
        {
            bool EndConnection = false;
            var userEndPoint = handler.RemoteEndPoint as IPEndPoint;
            if (userEndPoint == null) throw new Exception("User has valied connection!");
            Console.WriteLine(userEndPoint.Address.ToString() + ": User connected");
            try
            {
                while (!EndConnection)
                {
                    bool messageComplete = false;
                    string IncomingMessage = "";
                    // RECIVE
                    while (!messageComplete)
                    {
                        var buffer = new byte[Constants.BUFFER_SIZE_BYTE];
                        var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                        var response = Encoding.UTF8.GetString(buffer, 0, received);

                        if (response.Length == 1 && response.StartsWith(Constants.FIN))
                        {
                            EndConnection = true;
                            break;
                        }

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
                        }
                        */
                        IncomingMessage += response;
                    }
                    // PROCESS
                    var answer = Function(IncomingMessage);
                    // SEND
                    answer += Constants.EOM;
                    var messageBytes = Encoding.UTF8.GetBytes(answer);
                    var code = await handler.SendAsync(messageBytes, SocketFlags.None);
                }
            }catch(SocketException soex)
            {
                Console.WriteLine(userEndPoint.Address.ToString() + ": " + soex.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Connection Error:");
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.WriteLine(userEndPoint.Address.ToString() + ": User disconnected");
                handler.Close();
            }
            return;
        }
    }


}
