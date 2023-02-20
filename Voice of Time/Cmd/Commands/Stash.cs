using Voice_of_Time.Shared;
using Voice_of_Time.Shared.Functions;
using Voice_of_Time.Transfer;
using VoTCore.Communication;
using VoTCore.Controll;
using VoTCore.Data;
using VoTCore.Package;
using VoTCore.Package.Header;

namespace Voice_of_Time.Cmd.Commands
{
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

        private async Task<bool> StashSync()
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

            foreach (var requestID in requestIDs) 
            { 
                StashMessage message = await Requests.GetStashMessage(currentConnection, ClientData.CurrentClient, stashID, requestID);

                var handling = message.MessageHandling;
                var msg      = message.Message;

                var desMsg = new VOTP(msg);

                if (desMsg.Header is HeaderStd header)
                {
                    if (header.MessageType == 0) // 0 = Invite
                    {
                        if (desMsg.Body is not PrivatChat body)
                        {
                            throw new Exception("Body is not in the right format!");
                        }
                        if (body.CryptedReciver != -1)
                        {
                            if (body.CryptedReciver != ClientData.CurrentClient.UserID)
                            {
                                throw new Exception("Invite is not for this client!");
                            }
                            body.DecryptData(ClientData.CurrentClient.UserKey);
                        }
                        await Requests.AcceptInvite(currentConnection, ClientData.CurrentClient, body.ChatID);
                        ClientData.CurrentClient.TextChats.Add(body);
                    }
                    else
                    {
                        throw new Exception("Unknwon MessageType");
                    }

                    if(handling == DataHandling.REMOVE_AFTER_GET_ACK)
                    {
                        await Requests.RemoveStashMessage(currentConnection, ClientData.CurrentClient, stashID, requestID);
                    }

                    continue;
                }
                throw new Exception("Unknown Header!");
            }








            return true;
        }
    }
}
