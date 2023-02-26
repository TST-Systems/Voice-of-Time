using System.Text;
using Voice_of_Time.Shared;
using Voice_of_Time.Shared.Functions;
using Voice_of_Time.Transfer;
using VoTCore.Communication;
using VoTCore.Communication.Data;
using VoTCore.Controll;
using VoTCore.Data;
using VoTCore.Package;
using VoTCore.Package.Header;
using VoTCore.Secure;

namespace Voice_of_Time.Cmd.Commands
{
    /// <summary>
    /// Get messages from stash and process them
    /// </summary>
    internal class Stash : IConsoleCommandAsync
    {
        public string Command => "stash";

        private readonly string[] aliases = { "st" };
        public string[] Aliases => aliases;

        public string Usage => "stash [argument]";

        public async Task<bool> ExecuteCommand(string command, string[] args)
        {
            if (ClientData.CurrentConnection is null)
            {
                Console.WriteLine("You are currently not connected to any server.");
                return true;
            }

            if (args.Length <= 0)
                return false;


            return args[0] switch
            {
                "sync" => await StashSync(), 
                _      => false,
            }; 
        }

         /// <summary>
         /// Get all messages that are in the own stash or in the stashes of the chats where self is member
         /// </summary>
         /// <returns>success</returns>
         /// <exception cref="Exception"></exception>
        public async Task<bool> StashSync()
        {
            if (ClientData.CurrentConnection is null || ClientData.CurrentClient is null) { return false; }
            ClientSocket currentConnection = ClientData.GetConnection((Guid)ClientData.CurrentConnection) ?? throw new Exception();
            // Sync with own Stash
            var stashID    = ClientData.CurrentClient.UserID;
            var receiptIDs = new List<long>();
            var requestIDs = new List<long>();

            // Get all Receipt IDs from stash
            receiptIDs = await Requests.GetStashReceiptIDList(currentConnection, ClientData.CurrentClient, stashID);
            
            // Look up the current status and add to requestList
            foreach (var receiptID in receiptIDs)
            {
                var Receipt = (stashID, receiptID);

                if (!ClientData.CurrentClient.ReceiptStatusDictionary.ContainsKey(Receipt)) {
                    ClientData.CurrentClient.ReceiptStatusDictionary.Add(Receipt, ReceiptStatus.TO_REQUEST);
                }

                var msgState = ClientData.CurrentClient.ReceiptStatusDictionary[Receipt];

                if (msgState == ReceiptStatus.REC_AND_ACC) continue;
                if (msgState == ReceiptStatus.TO_REQUEST) requestIDs.Add(receiptID);
            }

            requestIDs.Sort();
            Console.WriteLine($"Getting {requestIDs.Count} personal messages");

            // process each request by it self
            foreach (var requestID in requestIDs) 
            { 
                StashMessage message = await Requests.GetStashMessage(currentConnection, ClientData.CurrentClient, stashID, requestID);

                var handling = message.MessageHandling;
                var msg      = message.Message;

                var desMsg = new VOTP(msg);

                // Check if Header can be processed
                if (desMsg.Header is HeaderStd header)
                {
                    // Check if mewssageType can be Processed
                    if (header.MessageType == 0) // 0 = Invite
                    {
                        // Check if Body fits the messageType
                        if (desMsg.Body is not PrivatChat body)
                        {
                            throw new Exception("Body is not in the right format!");
                        }
                        // Decrypted if nessesary
                        if (body.CryptedReciver != -1)
                        {
                            if (body.CryptedReciver != ClientData.CurrentClient.UserID)
                            {
                                throw new Exception("Invite is not for this client!");
                            }
                            body.DecryptData(ClientData.CurrentClient.UserKey);
                        }
                        // Tell server that invite will for now be accepted
                        await Requests.AcceptInvite(currentConnection, ClientData.CurrentClient, body.ChatID);
                        // Add change the stae of the message to recived and ackliged
                        ClientData.CurrentClient.TextChats.Add(body);
                        ClientData.CurrentClient.ReceiptStatusDictionary[(stashID, requestID)] = ReceiptStatus.REC_AND_ACC;
                        Console.WriteLine($"Added new Chat: {body.Title}");
                    }
                    else
                    {
                        throw new Exception("Unknwon MessageType");
                    }

                    // Check data handling
                    if (handling == DataHandling.REMOVE_AFTER_GET_ACK)
                    {
                        // If message has to be deletet after successfull reciving delete it and remove the entry from the ReceiptDictonary
                        await Requests.RemoveStashMessage(currentConnection, ClientData.CurrentClient, stashID, requestID);
                        ClientData.CurrentClient.ReceiptStatusDictionary.Remove((stashID, requestID));
                    }

                    continue;
                }
                throw new Exception("Unknown Header!");
            }

            // Repeat for each chat the same procedure
            foreach (var chat in ClientData.CurrentClient.TextChats)
            {
                if (chat is not PrivatChat privatChat) continue;
                Console.WriteLine($"Getting {privatChat.Title}: ");
                var suc = await ChatStashSync(privatChat);
                if (suc)
                    Console.WriteLine("Succes");
                else
                    Console.WriteLine("Error");
            }


            return true;
        }

        /// <summary>
        /// Recive all messages of a chat and process them
        /// </summary>
        /// <param name="chat">Chat to check the stash</param>
        /// <returns>sucess</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> ChatStashSync(PrivatChat chat)
        {
            if (ClientData.CurrentConnection is null || ClientData.CurrentClient is null) { return false; }
            ClientSocket currentConnection = ClientData.GetConnection((Guid)ClientData.CurrentConnection) ?? throw new Exception();
            // Sync with own Stash
            var receiptIDs = new List<long>();
            var requestIDs = new List<long>();

            // Get all Receipt IDs from stash
            receiptIDs = await Requests.GetStashReceiptIDList(currentConnection, ClientData.CurrentClient, chat.ChatID);

            // Look up the current status and add to requestList
            foreach (var receiptID in receiptIDs)
            {
                var Receipt = (chat.ChatID, receiptID);

                if (!ClientData.CurrentClient.ReceiptStatusDictionary.ContainsKey(Receipt))
                {
                    ClientData.CurrentClient.ReceiptStatusDictionary.Add(Receipt, ReceiptStatus.TO_REQUEST);
                }

                var msgState = ClientData.CurrentClient.ReceiptStatusDictionary[Receipt];

                if (msgState == ReceiptStatus.REC_AND_ACC) continue;
                if (msgState == ReceiptStatus.TO_REQUEST) requestIDs.Add(receiptID);
            }

            requestIDs.Sort();
            Console.WriteLine($"Getting {requestIDs.Count} new messages");

            // process each request by it self
            foreach (var requestID in requestIDs)
            {
                StashMessage message = await Requests.GetStashMessage(currentConnection, ClientData.CurrentClient, chat.ChatID, requestID);

                var handling = message.MessageHandling;
                var msg      = message.Message;
                msg = Encoding.UTF8.GetString(CryproManager.AesDecyrpt(chat.Key, Convert.FromBase64String(msg)));

                var desMsg = new VOTP(msg);

                // Ceck if package can be processed
                if (desMsg.Header is HeaderStd header)
                {
                    if (header.MessageType == 1) // 1 = Message
                    {
                        if (desMsg.Body is not Message body)
                        {
                            throw new Exception("Body is not in the right format!");
                        }
                        // TODO: Confirm ack
                        chat.AddMessage(body);
                        ClientData.CurrentClient.ReceiptStatusDictionary[(chat.ChatID, requestID)] = ReceiptStatus.REC_AND_ACC;
                    }
                    else
                    {
                        throw new Exception("Unknwon MessageType");
                    }

                    continue;
                }
                throw new Exception("Unknown Header!");
            }

            return true;
        }
    }
}
