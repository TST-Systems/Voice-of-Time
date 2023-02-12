using System.Data;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;
using VoTCore.Secure;
using VoTCore.Package;
using System.Text.Json;

/**
 * @author      - Timeplex, SalzstangeManga
 * 
 * @created     - 09.01.2023
 * 
 * @last_change - 12.02.2023
 */
namespace Voice_of_Time.Transfer
{
    internal class ClientSocket : IDisposable
    {
        private record QueueItem
        (
            /// <summary>
            /// Message to send
            /// </summary>
            string Message,
            /// <summary>
            /// Function to call with when task is done 
            /// </summary>
            Func<string?, Task> CallBack,
            /// <summary>
            /// ID to track task
            /// </summary>
            long ID
        );

        /// <summary>
        /// Client Server Connection
        /// </summary>
        private TcpClient?     Client;
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
        /// Current status of Client-Server connection
        /// </summary>
        private protected ConnectionState currentState;
        /// <summary>
        /// Current status of Client-Server connection
        /// </summary>
        public ConnectionState CurrentState { get => currentState; }

        private Aes? CommunicationKey           = null;
        private bool secureCommunicationEnabled = false;

        public bool CommunicationKeyIsSet      { get => CommunicationKey != null; }
        public bool SecureCommunicationEnabled { get => secureCommunicationEnabled; }

        public readonly string Address;
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

        internal void SetCommunicationKey (Aes key, bool enable = true)
        {
            CommunicationKey           = key;
            secureCommunicationEnabled = enable;
        }

        internal void RemoveCommunicationKey()
        {
            CommunicationKey           = null;
            secureCommunicationEnabled = false;
        }

        internal void SwitchSecureCommunicationState(bool enable)
        {
            if (CommunicationKey is null) return;
            secureCommunicationEnabled = enable;
        }

        internal bool Connect()
        {
            if (Client is not null) return false;
            Client = new(Address, Port);
            return true;
        }

        /// <summary>
        /// Breaks up Connection with Server
        /// </summary>
        /// <returns></returns>
        internal bool Disconect()
        {
            if (Client is null) return false;
            if (Stream is null) return false;
            var fin_byte = Encoding.UTF8.GetBytes(Constants.FIN.ToString());
            Stream.Write(fin_byte, 0, fin_byte.Length);
            Client.Close();
            Client = null;
            return true;
        }

        Thread? Reader;

        private void StartReader()
        {
            if (Reader is not null) return;
            if (Client is null) throw new Exception("You need to open a connection first!");
            Stream ??= Client.GetStream();

            Reader = new Thread(() => 
            {
                byte[] tokenSOM = Encoding.UTF8.GetBytes(Constants.SOM);
                byte[] tokenEOM = Encoding.UTF8.GetBytes(Constants.EOM);
                byte[] tokenFIN = Encoding.UTF8.GetBytes(Constants.FIN);

                byte[] buffer = new byte[Constants.BUFFER_SIZE_BYTE];
                while (!isCancelled)
                {
                    bool messageComplete = false;

                    int bytesRead = Stream.Read(buffer, 0, buffer.Length);

                    if (bytesRead == 0) throw new Exception("Server didn't send Data!");

                    var IncomingMessageInBytes = buffer[0..bytesRead];

                    if (bytesRead < Constants.FIN.Length) throw new Exception("Connection to slow!");

                    if (Enumerable.SequenceEqual(IncomingMessageInBytes[0..tokenFIN.Length], tokenFIN))
                    {
                        Dispose();
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
                        bytesRead = Stream.Read(buffer, 0, buffer.Length);

                        IncomingMessageInBytes = IncomingMessageInBytes.Concat(buffer[0..bytesRead]).ToArray();
                    }

                    if (SecureCommunicationEnabled)
                    {
                        if (CommunicationKey is null) throw new Exception("Inteneral Error!");
                        IncomingMessageInBytes = CryproManager.AesDecyrpt(CommunicationKey, IncomingMessageInBytes);
                    }

                    var IncomingMessage = Encoding.UTF8.GetString(IncomingMessageInBytes);

                    // PROCESS
                    var process = new Thread(() => ProccessMessage(IncomingMessage));
                    process.Start();
                }
            });
            Reader.Start();
        }

        /// <summary>
        /// {XYZ}  -> Callback(?) / Handling
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private void ProccessMessage(string message)
        {
            var packageInfo = new VOTPInfo(message);

            var packageID   = packageInfo.PackageID;

            if (!CallBackRegister.ContainsKey(packageID)) throw new NotImplementedException();

            _ = CallBackRegister[packageID](message);

            CallBackRegister.Remove(packageID);
        }


        Thread? Writer;

        /// <summary>
        /// Task handler
        /// </summary>
        /// <returns></returns>
        private void StartWriter()
        {
            if (Writer is not null) return;
            if (Client is null) throw new Exception("You need to open a connection first!");
            Stream ??= Client.GetStream();

            Writer = new Thread(async () =>
            {
                while (!isCancelled)
                {
                    await itemInQueue.WaitAsync();
                    if (isCancelled) return;
                    while (Queue.Count > 0)
                    {
                        if (isCancelled) return;
                        var nextQueueItem = Queue.Dequeue();

                        byte[] messageBytes = Encoding.UTF8.GetBytes(nextQueueItem.Message);
                        byte[] tokenSOM = Encoding.UTF8.GetBytes(Constants.SOM);
                        byte[] tokenEOM = Encoding.UTF8.GetBytes(Constants.EOM);

                        if (secureCommunicationEnabled)
                        {
                            if (CommunicationKey is null) throw new ArgumentNullException();
                            messageBytes = CryproManager.AesEncyrpt(CommunicationKey, messageBytes);
                        }

                        var bytesToSend = tokenSOM.Concat(messageBytes).Concat(tokenEOM).ToArray();

                        Stream.Write(bytesToSend, 0, bytesToSend.Length);

                        CallBackRegister.Add(nextQueueItem.ID, nextQueueItem.CallBack);
                    }
                }
            });
            Writer.Start();
        }

        /// <summary>
        /// Stops the current Handler
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

        public void StartHandler()
        {
            if (Client is null) Connect();
            StartWriter();
            StartReader();
        }

        /// <summary>
        /// Add an Message to the "to send" queue
        /// </summary>
        /// <param name="message">Message as String (Must not be null)</param>
        /// <param name="callBack">Task to perform with the reply</param>
        /// <returns></returns>
        private async Task<long> EnqueueItem(string? message, Func<string?, Task> callBack, long id)
        {
            if (message is null) { return -1; }
            await QueueBlock.WaitAsync();
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

            SemaphoreSlim responseReady = new(0, 1);

            _ = await EnqueueItem(serialized, (lResponse) => { response = lResponse; responseReady.Release(); return Task.CompletedTask; }, id);

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
