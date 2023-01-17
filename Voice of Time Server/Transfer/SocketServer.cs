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



                    //-----------------------------------------------------------------------------

                    var bufferSOM = new byte[Constants.BUFFER_SIZE_BYTE];
                    var receivedSOM = await handler.ReceiveAsync(bufferSOM, SocketFlags.None);
                    var responseSOM = Encoding.UTF8.GetString(bufferSOM, 0, receivedSOM);

                    if (responseSOM.Length == 1 && responseSOM.StartsWith(Constants.FIN))
                    {
                        EndConnection = true;
                        break;
                    }

                    var indexOfSOM = responseSOM.IndexOf(Constants.SOM);
                    if (indexOfSOM < 0)
                    {
                        handler.Close(); // + Fehler werfen
                        return;
                    }
                    responseSOM = responseSOM.Remove(indexOfSOM, 1);


                    var indexOfEOM = responseSOM.IndexOf(Constants.EOM);
                    if (indexOfEOM > -1)
                    {
                        messageComplete = true;
                        responseSOM = responseSOM.Remove(indexOfEOM);
                    }
                    IncomingMessage += responseSOM;



                    while (!messageComplete)
                    {
                        var buffer = new byte[Constants.BUFFER_SIZE_BYTE];
                        var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                        var response = Encoding.UTF8.GetString(buffer, 0, received);

                        indexOfEOM = response.IndexOf(Constants.EOM);
                        if (indexOfEOM > -1)
                        {
                            messageComplete = true;
                            response = response.Remove(indexOfEOM);
                        }
                        IncomingMessage += response;
                    }

  
                    
                    // PROCESS
                    var answer = Function(IncomingMessage);
                    // SEND
                    var messageBytes = Encoding.UTF8.GetBytes(Constants.SOM + answer + Constants.EOM);
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
