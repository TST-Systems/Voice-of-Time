using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;
using VoTCore.Secure;
using VoTCore.Package;

/**
 * @author      - Timeplex, SalzstangeManga
 * 
 * @created     - 09.01.2023
 * 
 * @last_change - 18.02.2023
 */
namespace Voice_of_Time.Transfer
{
    internal class ClientSocket : IDisposable
    {
        /// <summary>
        /// Data type to store requests for the server and the procedur to call after a anserw was given
        /// </summary>
        /// <param name="Message">Message to send</param>
        /// <param name="CallBack">Function to call with when task is done</param>
        /// <param name="ID">ID to track task</param>
        private record QueueItem
        (
            string Message,
            Func<string?, Task> CallBack,
            long ID
        );

        /// <summary>
        /// Client Server Connection
        /// </summary>
        private TcpClient?     Client;
        /// <summary>
        /// Data stream of <see cref="Client"/>
        /// </summary>
        private NetworkStream? Stream;

        /// <summary>
        /// Queue of open Tasks
        /// </summary>
        private readonly Queue<QueueItem> Queue = new();
        /// <summary>
        /// Blockade to stop multiple read and write operations on the queue
        /// </summary>
        private readonly SemaphoreSlim QueueBlock = new(1, 1);
        /// <summary>
        /// Blockade for handling if ther is a new item in queue
        /// </summary>
        private readonly SemaphoreSlim itemInQueue = new(0, 1);

        /// <summary>
        /// <para>A helping register witch holds the Callback Funcktions of <see cref="QueueItem"/>.</para>
        /// This is used since the use of a Full-Duplex connection, witch no longer garantues that a anserw to a request is the next thing to expect.
        /// </summary>
        private readonly Dictionary<long, Func<string?, Task>> CallBackRegister = new();


        /// <summary>
        /// ID for new items
        /// </summary>
        private long IDNew = 0;
        /// <summary>
        /// Signal to show that the current handler has to stop
        /// </summary>
        private bool isCancelled = false;

        /// <summary>
        /// The key for encryption of communication with the server
        /// </summary>
        private Aes? CommunicationKey           = null;
        /// <summary>
        /// Shows if the communication will now be encrypted and decrypted
        /// </summary>
        private bool secureCommunicationEnabled = false;

        public bool CommunicationKeyIsSet      { get => CommunicationKey != null; }
        public bool SecureCommunicationEnabled { get => secureCommunicationEnabled; }

        /// <summary>
        /// Address of connection
        /// </summary>
        public readonly string Address;
        /// <summary>
        /// Port of connection. Default: 15050
        /// </summary>
        public readonly int    Port;

        /// <summary>
        /// A socket for client server communication with the ability to connect once, send endlessly
        /// </summary>
        /// <param name="address">Ip addess / Domain of the server</param>
        /// <param name="port">Port of the server</param>
        internal ClientSocket(string address, int port)
        {
            Address = address;
            Port = port;
        }

        ~ClientSocket()
        {
            Dispose();
        }

        /// <summary>
        /// Set and optional enable the secure communication with the server
        /// </summary>
        /// <param name="key">Aes key for the secure channel</param>
        /// <param name="enable">Enable secure communication? Default: true</param>
        internal void SetCommunicationKey (Aes key, bool enable = true)
        {
            CommunicationKey           = key;
            secureCommunicationEnabled = enable;
        }

        /// <summary>
        /// Remove the Aes Key and deactivate secure communication
        /// </summary>
        internal void RemoveCommunicationKey()
        {
            CommunicationKey           = null;
            secureCommunicationEnabled = false;
        }

        /// <summary>
        /// Turn secure communication on or off if a Communication key is set.
        /// </summary>
        /// <param name="enable">Enable secure communication?</param>
        internal void SwitchSecureCommunicationState(bool enable)
        {
            if (CommunicationKey is null) return;
            secureCommunicationEnabled = enable;
        }

        /// <summary>
        /// try connecting to set Address:Port
        /// </summary>
        /// <returns>Connection could be established</returns>
        internal bool Connect()
        {
            if (Client is not null) return false;
            Client = new(Address, Port);
            return true;
        }

        /// <summary>
        /// Breaks up Connection with Server if connected
        /// </summary>
        /// <returns>Connection could be closed</returns>
        internal bool Disconect()
        {
            // Check if Connection backend is still present or Disposed. This should never be true.
            if (Client is null) return false;
            // Check if communication was already closed
            if (Stream is null) return false;
            try
            {
                // Send a connection end symbol to the server
                var fin_byte = Encoding.UTF8.GetBytes(Constants.FIN.ToString());
                Stream.Write(fin_byte, 0, fin_byte.Length);
            }
            // Possible Exexption if Connection was already closed
            catch (Exception) {}
            // Close and Dispose Client
            Client.Close();
            Client.Dispose();
            Client = null;

            return true;
        }

        /// <summary>
        /// Thread for constant reading of incoming bytes on the connection 
        /// </summary>
        Thread? Reader;

        /// <summary>
        /// Start the reader thread to read incoming bytes on the connection
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void StartReader()
        {
            // Prechecks if Reader already active or Connection is closed
            if (Reader is not null) return;
            if (Client is null) throw new Exception("You need to open a connection first!");
            Stream ??= Client.GetStream();

            //Create Reader
            Reader = new Thread(() => 
            {
                // Declare known tokens for commuication
                byte[] tokenSOM = Encoding.UTF8.GetBytes(Constants.SOM);
                byte[] tokenEOM = Encoding.UTF8.GetBytes(Constants.EOM);
                byte[] tokenFIN = Encoding.UTF8.GetBytes(Constants.FIN);

                // Reusable buffer for incoming messages
                byte[] buffer = new byte[Constants.BUFFER_SIZE_BYTE];
                while (!isCancelled)
                {
                    bool messageComplete = false; // shows if incoming bytes had and End Of Message token
                    // Read first part of message
                    int bytesRead = 0;
                    try
                    {
                        bytesRead = Stream.Read(buffer, 0, buffer.Length);
                    }
                    catch(IOException)
                    {
                        Dispose();
                        return;
                    }

                    if(isCancelled){
                        Dispose();
                        return;
                    }

                    if (bytesRead == 0) throw new Exception("Server did not send any Data!");
                    // Cut out the relevant Bytes from buffer
                    var IncomingMessageInBytes = buffer[0..bytesRead];
                    // Check if size of bytesRead is reasonable (Check if message is at least as long as the shortest token)
                    if (bytesRead < Constants.FIN.Length) throw new Exception("Connection to slow!");
                    // Check if server requested a end of connection
                    if (Enumerable.SequenceEqual(IncomingMessageInBytes[0..tokenFIN.Length], tokenFIN))
                    {
                        Dispose();
                        return;
                    }
                    // Check if Message started with a Start Of Message token
                    if (!Enumerable.SequenceEqual(IncomingMessageInBytes[0..tokenSOM.Length], tokenSOM)) throw new Exception("Communication not valid!");
                    // If yes cut away the token part
                    IncomingMessageInBytes = IncomingMessageInBytes[tokenSOM.Length..];

                    while (!messageComplete)
                    {
                        // Check if message is complet with the End Of Message token at the End of the message
                        if (Enumerable.SequenceEqual(
                            IncomingMessageInBytes[(IncomingMessageInBytes.Length - tokenEOM.Length)..IncomingMessageInBytes.Length],
                            tokenEOM))
                        {
                            // If yes end the read loop and cut away the EOM part
                            messageComplete = true;
                            IncomingMessageInBytes = IncomingMessageInBytes[..(IncomingMessageInBytes.Length - tokenEOM.Length)];
                            break;
                        }
                        // If message is not complet recive the rest of the message
                        buffer = new byte[Constants.BUFFER_SIZE_BYTE];
                        bytesRead = Stream.Read(buffer, 0, buffer.Length);
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
                    var process = new Thread(() => ProccessMessage(IncomingMessage));
                    process.Start();
                }
            });
            // Start Reader
            Reader.Start();
        }

        /// <summary>
        /// Handling of incoming messages
        /// </summary>
        /// <param name="message">JSON string</param>
        private void ProccessMessage(string message)
        {
            // Get package ID by peaking the package preheader
            var packageInfo = new VOTPInfo(message);
            var packageID   = packageInfo.PackageID;

            // Check if packageID is known
            if (!CallBackRegister.ContainsKey(packageID)) throw new NotImplementedException(); // TODO: Handling if message is not a anserwer

            // Call the function witch is connected to the packageID
            _ = CallBackRegister[packageID](message);
            
            // Remove the callback from the register
            CallBackRegister.Remove(packageID);
        }

        /// <summary>
        /// Thread for a "use if needed" process to write messages to the server
        /// </summary>
        Thread? Writer;

        /// <summary>
        /// Task handler
        /// </summary>
        /// <returns></returns>
        private void StartWriter()
        {
            // Prechecks if writer already active or connection is closed
            if (Writer is not null) return;
            if (Client is null) throw new Exception("You need to open a connection first!");
            Stream ??= Client.GetStream();

            // Create & start writer
            Writer = new Thread(async () =>
            {
                while (!isCancelled)
                {
                    // Wait for an item to be queued
                    await itemInQueue.WaitAsync();
                    if (isCancelled) return; // Not nessarary?
                    // Constant recalling as long as a item is in the queue without falling back to the waiting for new messages
                    while (Queue.Count > 0)
                    {
                        if (isCancelled) return;
                        // Get next item in queue
                        var nextQueueItem = Queue.Dequeue();

                        // Declare Tokens and convert message to a usable format
                        byte[] messageBytes = Encoding.UTF8.GetBytes(nextQueueItem.Message);
                        byte[] tokenSOM = Encoding.UTF8.GetBytes(Constants.SOM);
                        byte[] tokenEOM = Encoding.UTF8.GetBytes(Constants.EOM);

                        // Encrypted messages if activated
                        if (secureCommunicationEnabled)
                        {
                            if (CommunicationKey is null) throw new ArgumentNullException();
                            messageBytes = CryproManager.AesEncyrpt(CommunicationKey, messageBytes);
                        }

                        // Stick the Start Of Message and the End Of Message Token to message
                        var bytesToSend = tokenSOM.Concat(messageBytes).Concat(tokenEOM).ToArray();

                        Stream.Write(bytesToSend, 0, bytesToSend.Length);

                        // Add the callback function to the Dictonary
                        CallBackRegister.Add(nextQueueItem.ID, nextQueueItem.CallBack);
                    }
                }
            });
            Writer.Start();
        }

        /// <summary>
        /// Stops the current Reader and Writer
        /// </summary>
        public void StopHandler()
        {
            // Set stoptoken
            isCancelled = true;
            // Allow Handler to run, even when their is no message to send
            itemInQueue.Release();
            // Removing Writer and Reader
            Writer = null;
            Reader = null;
        }

        /// <summary>
        /// Start the Reader and Writer
        /// </summary>
        public void StartHandler()
        {
            // check if Client is connected, if not connect
            if (Client is null) Connect();
            // Start reader and writer
            StartWriter();
            StartReader();
        }

        /// <summary>
        /// Add an Message to the "to send" queue
        /// </summary>
        /// <param name="message">Message as String (Must not be null)</param>
        /// <param name="callBack">Task to perform with the reply</param>
        /// <returns>packageID of message</returns>
        private async Task<long> EnqueueItem(string? message, Func<string?, Task> callBack, long id)
        {
            if (message is null) { return -1; }
            
            await QueueBlock.WaitAsync();

            // Add message to queue and signilase the change to writer
            Queue.Enqueue(new QueueItem(message, callBack, id));
            itemInQueue.Release();

            QueueBlock.Release();
            return id;
        }

        /// <summary>
        /// Add a message to the Queue. This way you recive the message instead of an complety async Task
        /// </summary>
        /// <param name="message">Message as String (Must not be null)</param>
        /// <returns>Response from server</returns>
        public async Task<VOTP> EnqueueItem(VOTP packageToSend)
        {
            // SetpackageID
            var id = IDNew++;
            packageToSend.PackageID = id;
            // Serialize
            var serialized = packageToSend.Serialize();

            // Send and Reciver anserw
            string? response = "";

            // Awaitable blockade witch will be relaeased if a message was recived
            SemaphoreSlim responseReady = new(0, 1);

            _ = await EnqueueItem(serialized, (lResponse) => { response = lResponse; responseReady.Release(); return Task.CompletedTask; }, id);

            // Await the response of server
            await responseReady.WaitAsync();

            return new VOTP(response);
        }

        public void Dispose()
        {
            StopHandler();
            Queue.Clear();
            Disconect();
            GC.SuppressFinalize(this);
        }
    }
}
