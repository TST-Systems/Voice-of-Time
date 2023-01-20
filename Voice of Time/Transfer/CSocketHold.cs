using System.Data;
using System.Net.Sockets;
using System.Text;

namespace Voice_of_Time.Transfer
{
    internal class CSocketHold : CSocketSingle, IDisposable
    {
        private struct QueueItem
        {
            /// <summary>
            /// Message to send
            /// </summary>
            public string Message { get; set; }
            /// <summary>
            /// Function to call with when task is done 
            /// </summary>
            public Func<string?, Task> CallBack { get; set; }
            /// <summary>
            /// ID to track task
            /// </summary>
            public long ID { get; }

            public QueueItem(string message, Func<string?, Task> callBack, long iD)
            {
                Message = message;
                CallBack = callBack;
                ID = iD;
            }
        }
        /// <summary>
        /// Client Server Connection
        /// </summary>
        private Socket Client { get; }

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
        /// ID for new items
        /// </summary>
        private long IDNew = 0;
        /// <summary>
        /// ID of currently processed Task
        /// </summary>
        private long IDCurrent = -1;
        /// <summary>
        /// Signal to show that the current handler has to stop
        /// </summary>
        private bool isCancelled = false;
        /// <summary>
        /// Current Message handler
        /// </summary>
        private Task? handler;
        /// <summary>
        /// Shows if the handler is currently running
        /// </summary>
        public bool IsRunning { get => handler != null; }

        /// <summary>
        /// Current status of Client-Server connection
        /// </summary>
        private protected ConnectionState currentState;
        /// <summary>
        /// Current status of Client-Server connection
        /// </summary>
        public ConnectionState CurrentState { get => currentState; }


        /// <summary>
        /// A socket for client server communication with the ability to connect once, send endlessly
        /// </summary>
        /// <param name="address">Ip addess / Domain of the server</param>
        /// <param name="port">Port of the server</param>
        internal CSocketHold(string address, int port) : base(address, port)
        {
            Client = new(
            IpEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);
            currentState = ConnectionState.Closed;
        }

        ~CSocketHold()
        {
            Dispose();
        }

        internal async Task<bool> AutoStart()
        {
            var con = Connect();
            StartHandler();
            return await con;
        }

        /// <summary>
        /// Connect with the Server
        /// </summary>
        /// <returns>Connection could be established</returns>
        internal async Task<bool> Connect()
        {
            currentState = ConnectionState.Connecting;
            try
            {
                await Client.ConnectAsync(IpEndPoint);
            }
            catch (Exception ex)
            {
#if DEBUG
                throw ex;
#endif
                Console.WriteLine(ex.ToString());
                currentState = ConnectionState.Broken;
                return false;
            }
            currentState = ConnectionState.Open;
            return true;
        }

        /// <summary>
        /// Breaks up Connection with Server
        /// </summary>
        /// <returns></returns>
        internal bool Disconect()
        {
            try
            {
                var fin_byte = Encoding.UTF8.GetBytes(Constants.FIN.ToString());
                var fin_code = Client.Send(fin_byte, SocketFlags.None);
                Client.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Breaks up Connection with Server
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> DisconectAsync()
        {
            try
            {
                var fin_byte = Encoding.UTF8.GetBytes(Constants.FIN.ToString());
                var fin_code = await Client.SendAsync(fin_byte, SocketFlags.None);
                Client.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Task handler
        /// </summary>
        /// <returns></returns>
        private async Task QueueHandler()
        {
            while (!isCancelled)
            {
                await itemInQueue.WaitAsync();
                if (isCancelled) return;
                while (Queue.Count > 0)
                {
                    if (isCancelled) return;
                    var nextQueueItem = Queue.Dequeue();

                    var messageBytes = Encoding.UTF8.GetBytes(Constants.SOM + nextQueueItem.Message + Constants.EOM);
                    var code = await Client.SendAsync(messageBytes, SocketFlags.None);

                    bool messageComplete = false;
                    string IncomingMessage = "";


                    //-----------------------------------------------------------------------------

                    var bufferSOM = new byte[Constants.BUFFER_SIZE_BYTE];
                    var receivedSOM = await Client.ReceiveAsync(bufferSOM, SocketFlags.None);
                    var responseSOM = Encoding.UTF8.GetString(bufferSOM, 0, receivedSOM);


                    var indexOfSOM = responseSOM.IndexOf(Constants.SOM);
                    if (indexOfSOM < 0)
                    {
                        Client.Close(); // + Fehler werfen
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
                        var received = await Client.ReceiveAsync(buffer, SocketFlags.None);
                        var response = Encoding.UTF8.GetString(buffer, 0, received);

                        indexOfEOM = response.IndexOf(Constants.EOM);
                        if (indexOfEOM > -1)
                        {
                            messageComplete = true;
                            response = response.Remove(indexOfEOM);
                        }
                        IncomingMessage += response;
                    }
                    _ = nextQueueItem.CallBack(IncomingMessage);


                }
            }
        }

        /// <summary>
        /// Start the Handler if not active
        /// </summary>
        public void StartHandler()
        {
            handler ??= QueueHandler();
        }

        /// <summary>
        /// Stops the current Handler
        /// </summary>
        public async void StopHandler()
        {
            // Check if handler is running
            if (handler is null) return;
            // Set stoptoken
            isCancelled = true;
            // Allow Handler to run, even when their is no message to send
            itemInQueue.Release();
            // Wait that the handler closes
            await handler;
            // Reset handler and the halt signal
            isCancelled = false;
            handler.Dispose();
            handler = null;
        }

        /// <summary>
        /// Add an Message to the "to send" queue
        /// </summary>
        /// <param name="message">Message as String (Must not be null)</param>
        /// <param name="callBack">Task to perform with the reply</param>
        /// <returns></returns>
        internal async Task<long> EnqueueItem(string? message, Func<string?, Task> callBack)
        {
            if (message is null) { return -1; }
            await QueueBlock.WaitAsync();
            var id = IDNew++;
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
        public async Task<string?> EnqueueItem(string? message)
        {
            string? response = "";

            SemaphoreSlim responseReady = new(0, 1);

            _ = await EnqueueItem(message, (lResponse) => { response = lResponse; responseReady.Release(); return Task.CompletedTask; });

            await responseReady.WaitAsync();

            return response;
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
