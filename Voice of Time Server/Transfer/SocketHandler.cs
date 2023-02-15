using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Voice_of_Time_Server.Shared;
using VoTCore;
using VoTCore.Communication.Extra;
using VoTCore.Package;
using VoTCore.Package.AbsData;
using VoTCore.Package.AData;
using VoTCore.Package.Header;
using VoTCore.Package.Interfaces;
using VoTCore.Package.SData;
using VoTCore.Package.SecData;
using VoTCore.Secure;

/**
 * @author      - Timeplex
 * 
 * @created     - 20.01.2023
 * 
 * @last_change - 12.02.2023
 */
namespace Voice_of_Time_Server.Transfer
{
    internal class SocketHandler
    {
        private readonly TcpClient socket;
        private readonly NetworkStream stream;
        private readonly Aes CommunicationKey = Aes.Create();

        private long UserID = -1;
        private bool SecureCommunicationEnabled = false;
        private bool requestEncryption = false;

        private RSA? UserPubKey;
        private bool CommunicationVerified = false;
        private bool requestConnectionClose = false;

        private bool EndConnection = false;

        private readonly byte[] TokenSOM = Encoding.UTF8.GetBytes(Constants.SOM);
        private readonly byte[] TokenEOM = Encoding.UTF8.GetBytes(Constants.EOM);
        private readonly byte[] TokenFIN = Encoding.UTF8.GetBytes(Constants.FIN);

        private string Address { 
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
            Console.WriteLine(userEndPoint.Address.ToString() + ": User connected");

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
                    Console.WriteLine(Address + ": " + soex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connection Error:");
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    Console.WriteLine(Address + ": User disconnected");
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

                if (requestEncryption)
                {
                    requestEncryption = false;
                    SecureCommunicationEnabled = true;
                }
                if (requestConnectionClose) EndConnection = true;
            }
            catch (IOException ex)
            {
                Console.WriteLine(Address + ": " + ex.Message);
                EndConnection = true;
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
                    response = ProccessRequest(req, package.Body, package.PackageID);
                    break;
            }



            return response;
        }

        private string ProccessRequest(HeaderReq header, IVOTPBody? body, long packageID)
        {
            IVOTPHeader? sendHeader = null;
            IVOTPBody? sendBody     = null;

            var typeOfRequest = header.Request;

            // Prechecks
            var userID = header.SenderID;
            if(userID != UserID && header.Request != RequestType.VERIFY)
            {
                sendHeader             = new HeaderAck(false);
                sendBody               = new SData_String("Wrong UserID! Disconecting!");
                requestConnectionClose = true;
                goto END;
            }
            //

            switch (typeOfRequest)
            {
                case RequestType.IDENTITY:
                    sendHeader = new HeaderAck(true);
                    sendBody = new SData_Guid(ServerData.server.ServerIdentity);
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
                        ServerData.server.ChangeUserKey(UserID, UserPubKey);
                    }

                    sendHeader = new HeaderAck(true);
                    sendBody = new SecData_Key_RSA(ServerData.server.ServerKey, 0);

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

                    if(!ServerData.server.UserExists(header.SenderID))
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody   = new SData_String("User unknown! Register first!");
                        break;
                    }

                    UserID = header.SenderID;

                    var client = ServerData.server.GetUser(UserID) ?? throw new Exception("User is known & unknown at the same time :/");
                    UserPubKey = client.PublicKey.PublicKey;


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
                        if (header.SenderID <= 0 || !ServerData.server.UserExists(header.SenderID))
                        {
                            sendHeader = new HeaderAck(false);
                            sendBody   = new SData_String("Public key unknown! Please exhange your Key first!");
                            break;
                        }
                        var _client = ServerData.server.GetUser(UserID) ?? throw new Exception("User is known & unknown at the same time :/");
                        UserPubKey  = _client.PublicKey.PublicKey;
                    }

                    // Because if he doesn't understand the key, he has to close the connection.
                    if(typeOfRequest == RequestType.VERIFY) CommunicationVerified = true; 

                    requestEncryption = true;

                    var preBody = new SecData_Key_Aes(CommunicationKey, 0);
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
                    var uid = ServerData.server.AddUser(UserPubKey, "");

                    CommunicationVerified = true;
                    UserID                = uid;

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

                    ServerData.server.ChangeUserUsername(UserID, strBody.Data);

                    sendHeader = new HeaderAck(true);
                    break;
                case RequestType.GET_PUBLIC_USER:
                    if (!CommunicationVerified)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String("You need to be Verified to use this function!");
                        break;
                    }
                    if (body is not SData_Long longBody)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String($"Wrong Body! Need to be a {nameof(SData_Long)}");
                        break;
                    }
                    if (!ServerData.server.UserExists(longBody.Data))
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String($"User with the ID: {longBody.Data} is unknown!");
                        break;
                    }
                    sendHeader = new HeaderAck(true);
                    sendBody   = ServerData.server.GetUser(longBody.Data);
                    break;
                case RequestType.GET_USERID_LIST:
                    if (!CommunicationVerified)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String("You need to be Verified to use this function!");
                        break;
                    }
                    sendHeader = new HeaderAck(true);
                    sendBody = new AData_Long(ServerData.server.GetUserIDs());
                    break;
                case RequestType.REGISTER_PRIVAT_CHAT:
                    if (!CommunicationVerified)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String("You need to be Verified to use this function!");
                        break;
                    }

                    var chatID = ServerData.server.AddChat(UserID);

                    sendHeader = new HeaderAck(true);
                    sendBody   = new SData_Long(chatID);
                    break;
                case RequestType.INVITE_USER_PRIVATCHAT:
                    if (!CommunicationVerified)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String("You need to be Verified to use this function!");
                        break;
                    }                    
                    if (body is not AbsData_Invite invite)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String($"Wrong Body! Need to be a {nameof(AbsData_Invite)}");
                        break;
                    }
                    if (UserID != invite.SourceID)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String($"UserID:{UserID} is not the same as SourceID:{invite.SourceID}");
                        break;
                    }
                    if (!ServerData.server.UserExists(invite.TargetID))
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String($"UserID:{invite.TargetID} is not a User of this Server!");
                        break;
                    }
                    if (!ServerData.server.ChatExists(invite.ChatID))
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String($"Chat with the ID:{invite.ChatID} is unknwon!");
                        break;
                    }
                    ChatUserState UserChatState = ServerData.server.GetChatMember(invite.ChatID, UserID);
                    ChatUserState TargetChatState = ServerData.server.GetChatMember(invite.ChatID, invite.TargetID);
                    if (TargetChatState != ChatUserState.NONE) // TODO: Detailed checks
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String($"Taget user is alrady part of the Chat!");
                        break;
                    }
                    if (UserChatState == ChatUserState.NONE || (UserChatState & ChatUserState.BLOCKED) != 0)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String($"You are not a member of this chat!");
                        break;
                    }
                    if ((UserChatState & ChatUserState.ADMIN) != 0 && (UserChatState & ChatUserState.MODERATOR) != 0)
                    {
                        sendHeader = new HeaderAck(false);
                        sendBody = new SData_String($"You don't have the nessesary rights to do that!");
                        break;
                    }
                    ServerData.server.AddChatUser(invite.ChatID, invite.TargetID, ChatUserState.INVITED);
                    sendHeader = new HeaderAck(true);
                    break;
            }

            END:

            if (sendHeader is null) throw new Exception("Unknown request!");

            VOTP sendPackage = new(sendHeader, sendBody)
            {
                PackageID = packageID
            };
            return sendPackage.Serialize();
        }
    }
}
