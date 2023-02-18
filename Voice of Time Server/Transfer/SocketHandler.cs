using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Voice_of_Time_Server.Shared;
using VoTCore;
using VoTCore.Communication.Extra;
using VoTCore.Package;
using VoTCore.Package.AbsData;
using VoTCore.Package.Header;
using VoTCore.Package.Interfaces;
using VoTCore.Package.SData;
using VoTCore.Secure;

/**
 * @author      - Timeplex
 * 
 * @created     - 20.01.2023
 * 
 * @last_change - 18.02.2023
 */
namespace Voice_of_Time_Server.Transfer
{
    internal class SocketHandler
    {
        private  readonly TcpClient socket;
        private  readonly NetworkStream stream;
        internal readonly Aes CommunicationKey = Aes.Create();

        internal RSA? UserPubKey;
        internal long UserID = -1;

        internal bool SecureCommunicationEnabled = false;
        internal bool CommunicationVerified      = false;

        internal bool RequestEncryption = false;
        internal bool RequestConnectionClose = false;

        internal bool EndConnection = false;

        private readonly byte[] TokenSOM = Encoding.UTF8.GetBytes(Constants.SOM);
        private readonly byte[] TokenEOM = Encoding.UTF8.GetBytes(Constants.EOM);
        private readonly byte[] TokenFIN = Encoding.UTF8.GetBytes(Constants.FIN);

        internal string Address { 
            get 
            {
                var value = "Unknown";
                if(socket.Client.RemoteEndPoint is IPEndPoint endpoint)
                {
                    value = endpoint.Address.ToString();
                }
                return value; 
            } 
        }

        private Thread? Reader;

        public SocketHandler(TcpClient socket)
        {
            this.socket = socket;
            stream = socket.GetStream();


            if (socket.Client.RemoteEndPoint is not IPEndPoint userEndPoint) throw new Exception("Invalied connection!");
            WriteInfo("User connected");

            StartReader();
        }


        private void StartReader()
        {
            Reader = new Thread(() =>
            {
                byte[] buffer = new byte[Constants.BUFFER_SIZE_BYTE];
                try
                {
                    while (!EndConnection)
                    {
                        bool messageComplete = false;

                        int bytesRead = stream.Read(buffer, 0, buffer.Length);

                        if (bytesRead == 0) throw new Exception("Server didn't send Data!");

                        var IncomingMessageInBytes = buffer[0..bytesRead];

                        if (bytesRead < Constants.FIN.Length) throw new Exception("Connection to slow!");

                        if (Enumerable.SequenceEqual(IncomingMessageInBytes[0..TokenFIN.Length], TokenFIN))
                        {
                            return;
                        }

                        if (!Enumerable.SequenceEqual(IncomingMessageInBytes[0..TokenSOM.Length], TokenSOM)) throw new Exception("Communication not valid!");
                        IncomingMessageInBytes = IncomingMessageInBytes[TokenSOM.Length..];

                        while (!messageComplete)
                        {
                            if (Enumerable.SequenceEqual(
                                IncomingMessageInBytes[(IncomingMessageInBytes.Length - TokenEOM.Length)..IncomingMessageInBytes.Length],
                                TokenEOM))
                            {
                                messageComplete = true;
                                IncomingMessageInBytes = IncomingMessageInBytes[..(IncomingMessageInBytes.Length - TokenEOM.Length)];
                                break;
                            }

                            buffer = new byte[Constants.BUFFER_SIZE_BYTE];
                            bytesRead = stream.Read(buffer, 0, buffer.Length);

                            IncomingMessageInBytes = IncomingMessageInBytes.Concat(buffer[0..bytesRead]).ToArray();
                        }

                        if (SecureCommunicationEnabled)
                        {
                            if (CommunicationKey is null) throw new Exception("Inteneral Error!");
                            IncomingMessageInBytes = CryproManager.AesDecyrpt(CommunicationKey, IncomingMessageInBytes);
                        }

                        var IncomingMessage = Encoding.UTF8.GetString(IncomingMessageInBytes);

                        // PROCESS
                        var process = new Thread(() => WriteMessage(ProccessResponse(IncomingMessage)));
                        process.Start();
                    }
                }
                catch (IOException soex)
                {
                    WriteInfo(soex.Message);
                }
                catch (Exception ex)
                {
                    WriteInfo("Connection Error:");
                    WriteInfo(ex.ToString());
                }
                finally
                {
                    WriteInfo("User disconnected");
                    EndConnection = true;
                    socket.Close();
                }
                return;
            });
            Reader.Name = Address + ":Reader";
            Reader.Start();
        }


        private void WriteMessage(string message)
        {
            try
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);

                if (SecureCommunicationEnabled)
                {
                    if (CommunicationKey is null) throw new ArgumentNullException(nameof(CommunicationKey));
                    messageBytes = CryproManager.AesEncyrpt(CommunicationKey, messageBytes);
                }

                var bytesToSend = TokenSOM.Concat(messageBytes).Concat(TokenEOM).ToArray();

                stream.Write(bytesToSend, 0, bytesToSend.Length);

                if (RequestEncryption)
                {
                    RequestEncryption = false;
                    SecureCommunicationEnabled = true;
                }
                if (RequestConnectionClose) EndConnection = true;
            }
            catch (IOException ex)
            {
                WriteInfo(ex.Message);
                EndConnection = true;
                socket.Close();
            }
            return;
        }

        public void WriteInfo(string message)
        {
            Console.WriteLine(Address + ": " + message);
        }


        protected virtual string ProccessResponse(string incomingMessage)
        {
            //Get VOTP
            VOTP package = new(incomingMessage);

            var header    = package.Header;
            var body      = package.Body;
            var packageID = package.PackageID;

            if (header is not HeaderReq reqHeader)
            {
                var _toSend = new VOTP(new HeaderAck(false), new SData_Exception(new Exception($"{header.GetType().Name} is not supported by the server!")));
                return _toSend.Serialize();
            }

            var requestType = reqHeader.Request;

            Thread.CurrentThread.Name = $"{UserID}->{requestType}";

            var executer = ServerData.GetExecuter(requestType);

            if(executer is null)
            {
                var _toSend = new VOTP(new HeaderAck(false), new SData_Exception(new Exception($"{requestType} is not supported by the server!")));
                return _toSend.Serialize();
            }

            if(UserID != reqHeader.SenderID)
            {
                var _toSend = new VOTP(new HeaderAck(false), new SData_Exception(new Exception("You are tring to be anouther user! Use -1 as long you are not verifiyed or registerd!")));
                return _toSend.Serialize();
            }

            if (executer.ExecuteOnlyIfVerified && !CommunicationVerified)
            {
                var _toSend = new VOTP(new HeaderAck(false), new SData_Exception(new Exception("You need to be verifiyed or registerd to use this feature!")));
                return _toSend.Serialize();
            }

            var (toSendHeader, toSendBody) = executer.ExecuteRequest(reqHeader, body, this) ?? throw new Exception();

            var toSend = new VOTP(toSendHeader, toSendBody)
            {
                PackageID = packageID,
            };

            return toSend.Serialize();
        }

    }
}
