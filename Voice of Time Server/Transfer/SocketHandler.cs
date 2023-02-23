using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Voice_of_Time_Server.Shared;
using VoTCore;
using VoTCore.Package;
using VoTCore.Package.Header;
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
    /// <summary>
    /// <para>Per-connection connection handling.</para>
    /// Addidtional this can be used as Connection data meta 
    /// </summary>
    internal class SocketHandler
    {
        /// <summary>
        /// Socket to client
        /// </summary>
        private  readonly TcpClient socket;
        /// <summary>
        /// Data stream over socket
        /// </summary>
        private  readonly NetworkStream stream;
        /// <summary>
        /// Communication key. Only provided by the server for communication stuff
        /// </summary>
        internal readonly Aes CommunicationKey = Aes.Create();

        /// <summary>
        /// Public Key of User. Provided by <see cref="RequestExecuter.UserVerify"/> or <see cref="RequestExecuter.ServerPublicKeyExchange"/>
        /// </summary>
        internal RSA? UserPubKey;
        /// <summary>
        /// UserID of User. Provided by <see cref="RequestExecuter.UserRegister"/> or <see cref="RequestExecuter.UserVerify"/>
        /// </summary>
        internal long UserID = -1;

        /// <summary>
        /// All incomging and outgoing bytes will be decrypoted and encrypted
        /// </summary>
        internal bool SecureCommunicationEnabled = false;
        /// <summary>
        /// User Identity is proven (more or less)
        /// </summary>
        internal bool CommunicationVerified      = false;

        /// <summary>
        /// If set, communication will be decrypoted and encrypted after next package send
        /// </summary>
        internal bool RequestEncryption = false;      // TODO: Make Full-Duplex secure
        /// <summary>
        /// If set, communication will be closed after the next package send
        /// </summary>
        internal bool RequestConnectionClose = false; // TODO: Make Full-Duplex secure

        /// <summary>
        /// All processes will be stopt
        /// </summary>
        internal bool EndConnection = false;

        // Shared tokens for Reader and Writer
        private readonly byte[] TokenSOM = Encoding.UTF8.GetBytes(Constants.SOM);
        private readonly byte[] TokenEOM = Encoding.UTF8.GetBytes(Constants.EOM);
        private readonly byte[] TokenFIN = Encoding.UTF8.GetBytes(Constants.FIN);

        /// <summary>
        /// IP-Address of client
        /// </summary>
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

        /// <summary>
        /// Message reader
        /// </summary>
        private Thread? Reader;

        /// <summary>
        /// Default constructor. Will automaticly set up it self
        /// </summary>
        /// <param name="socket">Connection to client</param>
        /// <exception cref="Exception">Somthing wrong with the socket</exception>
        public SocketHandler(TcpClient socket)
        {
            this.socket = socket;
            stream = socket.GetStream();


            if (socket.Client.RemoteEndPoint is not IPEndPoint userEndPoint) throw new Exception("Invalied connection!");
            WriteInfo("User connected");

            StartReader();
        }

        /// <summary>
        /// Start the reader thread to read incoming bytes on the connection.
        /// </summary>
        /// 
        /// <exception cref="Exception"></exception>
        private void StartReader()
        {
            Reader = new Thread(() =>
            {
                // Reusable buffer for incoming messages
                byte[] buffer = new byte[Constants.BUFFER_SIZE_BYTE];
                try
                {
                    while (!EndConnection)
                    {
                        bool messageComplete = false; // shows if incoming bytes had and End Of Message token
                        // Read first part of message
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);

                        if (bytesRead == 0) throw new Exception("Server didn't send Data!");
                        // Cut out the relevant Bytes from buffer
                        var IncomingMessageInBytes = buffer[0..bytesRead];
                        // Check if size of bytesRead is reasonable (Check if message is at least as long as the shortest token)
                        if (bytesRead < Constants.FIN.Length) throw new Exception("Connection to slow!");
                        // Check if client requested a end of connection
                        if (Enumerable.SequenceEqual(IncomingMessageInBytes[0..TokenFIN.Length], TokenFIN))
                        {
                            return;
                        }
                        // Check if Message started with a Start Of Message token
                        if (!Enumerable.SequenceEqual(IncomingMessageInBytes[0..TokenSOM.Length], TokenSOM)) throw new Exception("Communication not valid!");
                        // If yes cut away the token part
                        IncomingMessageInBytes = IncomingMessageInBytes[TokenSOM.Length..];

                        while (!messageComplete)
                        {
                            // Check if message is complet with the End Of Message token at the End of the message
                            if (Enumerable.SequenceEqual(
                                IncomingMessageInBytes[(IncomingMessageInBytes.Length - TokenEOM.Length)..IncomingMessageInBytes.Length],
                                TokenEOM))
                            {
                                // If yes end the read loop and cut away the EOM part
                                messageComplete = true;
                                IncomingMessageInBytes = IncomingMessageInBytes[..(IncomingMessageInBytes.Length - TokenEOM.Length)];
                                break;
                            }

                            // If message is not complet recive the rest of the message
                            buffer = new byte[Constants.BUFFER_SIZE_BYTE];
                            bytesRead = stream.Read(buffer, 0, buffer.Length);

                            // Combine the current message with the new recived bytes
                            IncomingMessageInBytes = IncomingMessageInBytes.Concat(buffer[0..bytesRead]).ToArray();
                        }

                        // Decrypted message if secured communication is enabled
                        if (SecureCommunicationEnabled)
                        {
                            if (CommunicationKey is null) throw new Exception("Inteneral Error!");
                            IncomingMessageInBytes = CryproManager.AesDecyrpt(CommunicationKey, IncomingMessageInBytes);
                        }

                        // Convert the bytes to a usable format
                        var IncomingMessage = Encoding.UTF8.GetString(IncomingMessageInBytes);

                        // process incoming message
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
            })
            {
                // Setting the name for the Thread
                Name = Address + ":Reader"
            };

            Reader.Start();
        }
        
        /// <summary>
        /// Write a message to the client
        /// </summary>
        /// <param name="message">Message as string</param>
        /// <exception cref="ArgumentNullException"></exception>
        private void WriteMessage(string message)
        {
            try
            {
                // TODO: Block until writer is free again
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);

                // Encrypted message if activated
                if (SecureCommunicationEnabled)
                {
                    if (CommunicationKey is null) throw new ArgumentNullException(nameof(CommunicationKey));
                    messageBytes = CryproManager.AesEncyrpt(CommunicationKey, messageBytes);
                }

                // stick EOM and SOM on it
                var bytesToSend = TokenSOM.Concat(messageBytes).Concat(TokenEOM).ToArray();

                stream.Write(bytesToSend, 0, bytesToSend.Length);

                // Activate de/encryption if requetsted
                if (RequestEncryption)
                {
                    RequestEncryption = false;
                    SecureCommunicationEnabled = true;
                }
                // Close the connection if requested
                if (RequestConnectionClose) EndConnection = true; // TODO: Refit for Full-Duplex
            }
            catch (IOException ex)
            {
                WriteInfo(ex.Message);
                EndConnection = true;
                socket.Close();
            }
            return;
        }

        /// <summary>
        /// Write out a message as socket to console
        /// </summary>
        /// <param name="message">Message to display</param>
        public void WriteInfo(string message) //TODO: Difrent states: INFO, DEBUG, ERROR, WARNING and user can choose what level of detail he wants to see
        {
            Console.WriteLine(Address + ": " + message);
        }

        /// <summary>
        /// Process a respnse generated by <see cref="Reader"/>
        /// </summary>
        /// <param name="incomingMessage">Message as UTF8 string</param>
        /// <returns>Response as UTF8 string</returns>
        /// <exception cref="Exception"></exception>
        protected virtual string ProccessResponse(string incomingMessage)
        {
            //Get VOTP
            VOTP package = new(incomingMessage);

            // Split it in its parts
            var header    = package.Header;
            var body      = package.Body;
            var packageID = package.PackageID;

            // Check if header is of any type which can be processed
            if (header is not HeaderReq reqHeader)
            {
                var _toSend = new VOTP(new HeaderAck(false), new SData_InternalException(InternalExceptionCode.UNKNOWN_HEADER_TYPE, $"{header.GetType().Name} is not supported by the server!"));
                return _toSend.Serialize();
            }

            // Get the type of requets
            var requestType = reqHeader.Request;

            // Set the name of the Tread for easyer DEBUG
            Thread.CurrentThread.Name = $"{UserID}->{requestType}";

            // Get the exuter matching the request type or give client an error if request is not known
            var executer = ServerData.GetExecuter(requestType);
            if(executer is null)
            {
                var _toSend = new VOTP(new HeaderAck(false), new SData_InternalException(InternalExceptionCode.UNKNOWN_REQUEST_TYPE, $"{requestType} is not supported by the server!"));
                return _toSend.Serialize();
            }

            // CHeck if sender matches the user of the socket (-1 anonym)
            if(UserID != reqHeader.SenderID)
            {
                var _toSend = new VOTP(new HeaderAck(false), new SData_InternalException(InternalExceptionCode.WRONG_SENDER, "You are tring to be anouther user! Use -1 as long you are not verifiyed or registerd!"));
                return _toSend.Serialize();
            }

            // Check if the executer can only be executed if the connection is verfiy and if the connection is verifyed
            if (executer.ExecuteOnlyIfVerified && !CommunicationVerified)
            {
                var _toSend = new VOTP(new HeaderAck(false), new SData_InternalException(InternalExceptionCode.COMMUNICATION_NOT_VERIFIED, "You need to be verifiyed or registerd to use this feature!"));
                return _toSend.Serialize();
            }

            // Get the package to send and set the packageID to symbolise that this is the anser to a request
            var (toSendHeader, toSendBody) = executer.ExecuteRequest(reqHeader, body, this) ?? throw new Exception();
            var toSend = new VOTP(toSendHeader, toSendBody)
            {
                PackageID = packageID,
            };

            return toSend.Serialize();
        }

    }
}
