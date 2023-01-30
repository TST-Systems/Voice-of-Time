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
        private bool requestEncryption = false;

        private RSA? UserPubKey;
        private bool CommunicationVerified  = false;
        private bool requestConnectionClose = false;

        public SocketHandler(Socket socket)
        {
            this.socket = socket;
        }

        internal protected virtual async Task HandleConnection()
        {
            bool EndConnection = false;
            byte[] tokenSOM = Encoding.UTF8.GetBytes(Constants.SOM);
            byte[] tokenEOM = Encoding.UTF8.GetBytes(Constants.EOM);
            byte[] tokenFIN = Encoding.UTF8.GetBytes(Constants.FIN);

            if (socket.RemoteEndPoint is not IPEndPoint userEndPoint) throw new Exception("Invalied connection!");
            Console.WriteLine(userEndPoint.Address.ToString() + ": User connected");

            try
            {
                while (!EndConnection)
                {
                    //RECIVE
                    bool messageComplete = false;

                    var buffer = new byte[Constants.BUFFER_SIZE_BYTE];
                    var received = await socket.ReceiveAsync(buffer, SocketFlags.None);

                    if (received == 0) throw new Exception("No data Transfer!");

                    var IncomingMessageInBytes = buffer[0..received];

                    if (received < Constants.FIN.Length) throw new Exception("Connection to slow!");

                    if (Enumerable.SequenceEqual(IncomingMessageInBytes[0..tokenFIN.Length], tokenFIN))
                    {
                        EndConnection = true;
                        return;
                    }

                    if (!Enumerable.SequenceEqual(IncomingMessageInBytes[0..tokenSOM.Length], tokenSOM)) throw new Exception("Communication not valid!");
                    IncomingMessageInBytes = IncomingMessageInBytes[tokenSOM.Length..];

                    while (!messageComplete)
                    {
                        if (Enumerable.SequenceEqual(
                            IncomingMessageInBytes[(IncomingMessageInBytes.Length - tokenEOM.Length)..IncomingMessageInBytes.Length],
                            tokenEOM))
                        {
                            messageComplete = true;
                            IncomingMessageInBytes = IncomingMessageInBytes[..(IncomingMessageInBytes.Length - tokenEOM.Length)];
                            break;
                        }

                        buffer = new byte[Constants.BUFFER_SIZE_BYTE];
                        received = await socket.ReceiveAsync(buffer, SocketFlags.None);

                        IncomingMessageInBytes = IncomingMessageInBytes.Concat(buffer[0..received]).ToArray();
                    }

                    if (SecureCommunicationEnabled)
                    {
                        using var encryptor = ConnectionKey.CreateDecryptor();
                        using MemoryStream memoryStream = new();
                        using CryptoStream cryptostream = new(memoryStream, encryptor, CryptoStreamMode.Write);

                        cryptostream.Write(IncomingMessageInBytes, 0, IncomingMessageInBytes.Length);
                        cryptostream.FlushFinalBlock();

                        IncomingMessageInBytes = memoryStream.ToArray();

                        cryptostream.Close();
                        memoryStream.Close();
                    }
                    var IncomingMessage = Encoding.UTF8.GetString(IncomingMessageInBytes);

                    // PROCESS
                    var answer = ProccessResponse(IncomingMessage);

                    // SEND
                    byte[] messageBytes = Encoding.UTF8.GetBytes(answer);

                    if (SecureCommunicationEnabled)
                    {
                        using var encryptor = ConnectionKey.CreateEncryptor();
                        using MemoryStream memoryStream = new();
                        using CryptoStream cryptostream = new(memoryStream, encryptor, CryptoStreamMode.Write);

                        cryptostream.Write(messageBytes, 0, messageBytes.Length);
                        cryptostream.FlushFinalBlock();

                        messageBytes = memoryStream.ToArray();

                        cryptostream.Close();
                        memoryStream.Close();
                    }

                    var bytesToSend = tokenSOM.Concat(messageBytes).Concat(tokenEOM).ToArray();

                    var code = await socket.SendAsync(bytesToSend, SocketFlags.None);
                    if (requestEncryption)
                    {
                        requestEncryption = false;
                        SecureCommunicationEnabled = true;
                    }
                    if (requestConnectionClose) EndConnection = true;
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
                case RequestType.VERIFY:
                    if (UserID > 0)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody   = new SData_String("You are already verifiyed! Server closes the connection!");
                        requestConnectionClose = true;
                        break;
                    }

                    if(header.SenderID <= 0)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody   = new SData_String("Use your UserID to verify!");
                        break;
                    }

                    if(!ServerInfo.server.PublicKeyDictionaryCopy.ContainsKey(header.SenderID))
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody   = new SData_String("User unknown! Register first!");
                        break;
                    }

                    UserID = header.SenderID;

                    UserPubKey = ServerInfo.server.PublicKeyDictionaryCopy[UserID];


                    goto COMM_KEY;
                case RequestType.COMM_KEY:
                COMM_KEY:

                    if (CommunicationVerified)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody   = new SData_String("You are already verifiyed! Server closes the connection!");
                        requestConnectionClose = true;
                        break;
                    }

                    if (UserPubKey is null)
                    {
                        if (header.SenderID <= 0 || !ServerInfo.server.PublicKeyDictionaryCopy.ContainsKey(header.SenderID))
                        {
                            sendHeader = new HeaderAck(false);
                            sendBody   = new SData_String("Public key unknown! Please exhange your Key first!");
                            break;
                        }
                        UserPubKey = ServerInfo.server.PublicKeyDictionaryCopy[header.SenderID];
                    }

                    // Because if he doesn't understand the key, he has to close the connection.
                    if(typeOfRequest == RequestType.VERIFY) CommunicationVerified = true; 

                    requestEncryption = true;

                    var preBody = new SecData_Key_Aes(ConnectionKey, 0);
                    preBody.EncryptData(UserPubKey, UserID);

                    sendHeader  = new HeaderAck(true);
                    sendBody = preBody;
                    break;
                case RequestType.REGISTRATION: //<- Can be attacked very easily
                    if(UserID > 0)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody  = new SData_String("You are already logged in!");
                        break;
                    }
                    if (!SecureCommunicationEnabled || UserPubKey is null)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String("You need to secure the communication first!");
                        break;
                    }
                    var uid = ServerInfo.server.AddUser(UserPubKey);

                    CommunicationVerified = true;
                    UserID                = uid;

                    sendHeader = new HeaderAck(true);
                    sendBody  = new SData_Long(uid);
                    break;
                case RequestType.SET_USERNAME:
                    if (!CommunicationVerified)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String("You need to secure the communication first!");
                        break;
                    }
                    if (UserID <= 0)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String("You are not known!");
                        break;
                    }

                    if (body is not SData_String strBody)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String("No new username!");
                        break;
                    }
                    if (strBody.Data is null)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String("No new username!");
                        break;
                    }

                    ServerInfo.server.UserDB[UserID].UserName = strBody.Data;

                    sendHeader = new HeaderAck(true);
                    break;
            }

            if (sendHeader is null) throw new Exception("Unknown request!");

            VOTP sendPackage = new(sendHeader, sendBody);
            return sendPackage.Serialize();
        }
    }
}
