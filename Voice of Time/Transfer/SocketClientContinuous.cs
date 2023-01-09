using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics.Metrics;
using System.Diagnostics;

namespace Voice_of_Time.Transfer
{
    internal class SocketClientContinuous : SocketClient
    {
        private struct QueueItem
        {
            /// <summary>
            /// Message to send
            /// </summary>
            public string? Message { get; set; }
            /// <summary>
            /// Function to call with when task is done 
            /// </summary>
            public Action<string?> CallBack { get; set; }
            /// <summary>
            /// ID to track task
            /// </summary>
            public long ID { get; } 

            public QueueItem(string? message, Action<string?> callBack, long iD)
            {
                Message  = message;
                CallBack = callBack;
                ID       = iD;
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
        /// ID for new items
        /// </summary>
        private long IDNew = 0;
        /// <summary>
        /// ID of currently processed Task
        /// </summary>
        private long IDCurrent = -1;

        /// <summary>
        /// Current status of Client-Server connection
        /// </summary>
        private protected ConnectionState currentState;
        public ConnectionState CurrentState { get => currentState; }


        internal SocketClientContinuous(string address, int port) : base(address, port)
        {
            Client = new(
            IpEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);
            currentState = ConnectionState.Closed;
        }

        internal async Task<bool> StartConnection()
        {
            currentState = ConnectionState.Connecting;
            try
            {
                await Client.ConnectAsync(IpEndPoint);
            }
            catch(Exception ex) 
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

        internal long EnqueueItem(string? message, Action<string?> callBack)
        {
            if (message == null) { return -1; }
            lock (Queue)
            {
                var id = IDNew++;
                Queue.Enqueue(new QueueItem(message, callBack, id++));
                return id;
            }
        }

    }
}
