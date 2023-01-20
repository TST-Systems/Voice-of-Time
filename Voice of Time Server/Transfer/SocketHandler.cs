using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using VoTCore;
using VoTCore.Package;
using VoTCore.Package.Header;
using VoTCore.Package.Interfaces;
using VoTCore.Package.SData;
using VoTCore.Package.SecData;

namespace Voice_of_Time_Server.Transfer
{
    internal class SocketHandler
    {
        private readonly Socket socket;
        private readonly Aes ConnectionKey = Aes.Create();

        private long UserID = -1;
        private bool SecureCommunicationEnabled = false;
        private bool requestEncryption          = false;

        private RSA? UserPubKey;
        private bool CommunicationVerified = false;

        public SocketHandler(Socket socket)
        {
            this.socket = socket;
        }

        internal protected virtual async Task HandleConnection()
        {
            bool EndConnection = false;
            if (socket.RemoteEndPoint is not IPEndPoint userEndPoint) throw new Exception("Invalied connection!");
            Console.WriteLine(userEndPoint.Address.ToString() + ": User connected");
            try
            {
                while (!EndConnection)
                {
                    //RECIVE
                    bool messageComplete = false;
                    string IncomingMessage = "";

                    var bufferSOM = new byte[Constants.BUFFER_SIZE_BYTE];
                    var receivedSOM = await socket.ReceiveAsync(bufferSOM, SocketFlags.None);
                    var responseSOM = Encoding.UTF8.GetString(bufferSOM, 0, receivedSOM);

                    if (responseSOM.StartsWith(Constants.FIN, StringComparison.Ordinal))
                    {
                        EndConnection = true;
                        return;
                    }

                    if (!responseSOM.StartsWith(Constants.SOM, StringComparison.Ordinal)) throw new Exception("Communication not valid!");
                    responseSOM = responseSOM.Remove(0, Constants.SOM.Length);


                    if (responseSOM.EndsWith(Constants.EOM, StringComparison.Ordinal))
                    {
                        messageComplete = true;
                        responseSOM = responseSOM.Remove(responseSOM.Length - Constants.EOM.Length);
                    }
                    IncomingMessage += responseSOM;



                    while (!messageComplete)
                    {
                        var buffer = new byte[Constants.BUFFER_SIZE_BYTE];
                        var received = await socket.ReceiveAsync(buffer, SocketFlags.None);
                        var response = Encoding.UTF8.GetString(buffer, 0, received);

                        if (response.EndsWith(Constants.EOM, StringComparison.Ordinal))
                        {
                            messageComplete = true;
                            response = response.Remove(response.Length - Constants.EOM.Length);
                        }
                        IncomingMessage += response;
                    }


                    if (SecureCommunicationEnabled)
                    {
                        using var encryptor = ConnectionKey.CreateEncryptor();
                        using MemoryStream memoryStream = new();
                        using CryptoStream cryptostream = new(memoryStream, encryptor, CryptoStreamMode.Read);

                        var IncomingMessageInBytes = Encoding.UTF8.GetBytes(IncomingMessage);

                        cryptostream.Read(IncomingMessageInBytes, 0, IncomingMessageInBytes.Length);
                        IncomingMessageInBytes = memoryStream.ToArray();

                        IncomingMessage = Encoding.UTF8.GetString(IncomingMessageInBytes);

                        cryptostream.Close();
                        memoryStream.Close();
                    }

                    // PROCESS
                    var answer = ProccessResponse(IncomingMessage);

                    // SEND
                    byte[] messageBytes = Encoding.UTF8.GetBytes(answer);
                    byte[] tokenSOM = Encoding.UTF8.GetBytes(Constants.SOM);
                    byte[] tokenEOM = Encoding.UTF8.GetBytes(Constants.EOM);

                    if (SecureCommunicationEnabled)
                    {
                        using var encryptor = ConnectionKey.CreateEncryptor();
                        using MemoryStream memoryStream = new();
                        using CryptoStream cryptostream = new(memoryStream, encryptor, CryptoStreamMode.Write);

                        cryptostream.Write(messageBytes, 0, messageBytes.Length);
                        messageBytes = memoryStream.ToArray();

                        cryptostream.Close();
                        memoryStream.Close();
                    }

                    var bytesToSend = tokenSOM.Concat(messageBytes).Concat(tokenEOM).ToArray();

                    var code = await socket.SendAsync(bytesToSend, SocketFlags.None);
                    if (requestEncryption)
                    {
                        requestEncryption          = false;
                        SecureCommunicationEnabled = true;
                    }
                }
            }
            catch (SocketException soex)
            {
                Console.WriteLine(userEndPoint.Address.ToString() + ": " + soex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection Error:");
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.WriteLine(userEndPoint.Address.ToString() + ": User disconnected");
                socket.Close();
            }
            return;
        }

        protected virtual string ProccessResponse(string incomingMessage)
        {
            var response = "";
            //Get VOTP
            VOTP package = new(incomingMessage);

            switch (package.Header)
            {
                case HeaderReq req:
                    response = ProccessRequest(req, package.Body);
                    break;
            }



            return response;
        }

        private string ProccessRequest(HeaderReq header, IVOTPBody? body)
        {
            IVOTPHeader? sendHeader = null;
            IVOTPBody? sendBody = null;

            var typeOfRequest = header.Request;

            switch (typeOfRequest)
            {
                case RequestType.IDENTITY:
                    sendHeader = new HeaderAck(true);
                    sendBody = new SData_Guid(ServerInfo.server.ServerIdentity);
                    break;
                case RequestType.KEY_EXCHANGE:
                    if (body is null)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String("Public key is only avaivaible for users with own Public key!");
                        break;
                    }

                    if (body is not SecData_Key_RSA rsaBody)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String("Server expectet a RSA key!");
                        break;
                    }

                    UserPubKey = rsaBody.GetKey();

                    if (header.SenderID > 0 && CommunicationVerified)
                    {
                        ServerInfo.server.AddPublicKey(header.SenderID, UserPubKey);
                    }

                    sendHeader = new HeaderAck(true);
                    sendBody = new SecData_Key_RSA(ServerInfo.server.ServerKey, 0);

                    break;
                case RequestType.COMM_KEY:
                    if (UserPubKey is null)
                    {
                        if (header.SenderID <= 0 || !ServerInfo.server.PublicKeyDictionaryCopy.ContainsKey(header.SenderID))
                        {
                            sendHeader = new HeaderAck(false);
                            sendBody = new SData_String("Public key unknown! Please exhange your Key first!");
                            break;
                        }
                        UserPubKey = ServerInfo.server.PublicKeyDictionaryCopy[header.SenderID];
                    }
                    sendHeader = new HeaderAck(true);
                    sendBody = new SecData_Key_Aes(ConnectionKey, 0);
                    break;
                case RequestType.REGISTRATION: //<- Can be attacked very easily
                    if (!SecureCommunicationEnabled || UserPubKey is null)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody   = new SData_String("You need to secure the communication first!");
                        break;
                    }
                    var uid = ServerInfo.server.AddUser(UserPubKey);

                    CommunicationVerified = true;

                    sendHeader = new HeaderAck(true);
                    sendBody   = new SData_Long(uid);
                    break;
                case RequestType.SET_USERNAME:
                    if (!CommunicationVerified)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String("You need to secure the communication first!");
                        break;
                    }
                    if (!ServerInfo.server.PublicKeyDictionaryCopy.ContainsKey(header.SenderID))
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody   = new SData_String("You are not known!");
                        break;
                    }

                    if (body is not SData_String strBody)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String("No new username!");
                        break;
                    }
                    if(strBody.Data is null)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String("No new username!");
                        break;
                    }

                    ServerInfo.server.UserDB[header.SenderID].UserName = strBody.Data;

                    sendHeader = new HeaderAck(true);
                    break;
            }

            if (sendHeader is null) throw new Exception("Unknown request!");

            VOTP sendPackage = new(sendHeader, sendBody);
            return sendPackage.Serialize();
        }
    }
}
