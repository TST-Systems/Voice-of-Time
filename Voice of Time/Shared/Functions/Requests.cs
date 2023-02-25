using Voice_of_Time.Transfer;
using VoTCore.Package.Header;
using VoTCore.Package.SData;
using VoTCore.Package;
using System.Security.Cryptography;
using VoTCore.Package.SecData;
using Voice_of_Time.User;
using VoTCore.Package.AData;
using VoTCore.Package.Interfaces;
using VoTCore.Exeptions;
using VoTCore.User;
using VoTCore.Communication;
using VoTCore.Package.AbsData;
using VoTCore.Package.StashData;
using VoTCore.Data;
using Voice_of_Time.Cmd.Commands;

/**
 * @author      - Timeplex
 * 
 * @created     - 10.02.2023
 * 
 * @last_change - 20.02.2023
 */
namespace Voice_of_Time.Shared.Functions
{
    public static class Requests
    {
        #region Process Helper
        private static async Task<T> RequestPackageHandler<T>(ClientSocket socket, VOTP package) where T : IVOTPBody
        {
            var resultPackage = await socket.EnqueueItem(package);

            // Check if Header is ok
            if (resultPackage.Header is not HeaderAck resHeader) throw new Exception("Server didn't responded correctly!");
            if (resHeader.Successful is false)
            {
                if (resultPackage.Body is not SData_InternalException exception || exception.Data is null) 
                    throw new Exception("Server coudn't Process the Request!");

                throw new Exception("Server coudn't Process the Request!\n" + exception.Data.Code + "\n" + exception.Data.Message);
            }

            // Check if Data is ok
            if (resultPackage.Body is null) throw new PackageBodyNullException("Server anserwed with no data");
            if (resultPackage.Body is not T resBody) throw new Exception($"Server anserwed with wrong output : {resultPackage.Body.GetType().Name}");

            return resBody;
        }

        private static async Task RequestPackageHandler(ClientSocket socket, VOTP package)
        {
            try
            {
                _ = await RequestPackageHandler<IVOTPBody>(socket, package);
                throw new Exception("Server respnded with not requested Data!");
            }
            catch (PackageBodyNullException) { }
        }
        #endregion

        public static async Task<Guid> RequestServerID(ClientSocket socket, long userID = -1)
        {
            var header = new HeaderReq(userID, RequestType.SERVER_GET_IDENTITY);
            var toSend = new VOTP(header);

            var result = await RequestPackageHandler<SData_Guid>(socket, toSend);

            return result.Data;
        }

        public static async Task<RSA> KeyExchangeWithServer(ClientSocket socket, RSA key, long userID = -1)
        {
            var header = new HeaderReq(userID, RequestType.SERVER_PUBLIC_KEY_EXCHANGE, 0);
            var body   = new SecData_Key_RSA(key, userID);

            var toSend = new VOTP(header, body);

            var result = await RequestPackageHandler<SecData_Key_RSA>(socket, toSend);

            return result.GetKey();
        }

        public static async Task OpenSecureCommunication(ClientSocket socket, RSA decryptionKey, long userID = -1)
        {
            var header = new HeaderReq(userID, RequestType.COMMUNICATION_GET_KEY_AND_SECURE);
            var toSend = new VOTP(header);

            var result = await RequestPackageHandler<SecData_Key_Aes>(socket, toSend);

            result.DecryptData(decryptionKey);

            var key = result.GetKey();

            socket.SetCommunicationKey(key);
        }
            
        public static async Task<long> RegisterClient(ClientSocket socket)
        {
            var header = new HeaderReq(-1, RequestType.USER_REGISTRATION);
            var toSend = new VOTP(header);

            var result = await RequestPackageHandler<SData_Long>(socket, toSend);

            return result.Data;
        }

        public async static Task SetUsername(ClientSocket socket, string username, long userID)
        {
            var header = new HeaderReq(userID, RequestType.USER_SET_USERNAME);
            var body   = new SData_String(username);
            var toSend = new VOTP(header, body);

            await RequestPackageHandler(socket, toSend);
        }

        public static async Task TestConnection(ClientSocket socket, Guid serverID, long userID)
        {
            var newServerID = await Requests.RequestServerID(socket, userID);

            if (serverID.CompareTo(newServerID) != 0) throw new Exception("Connection Invalid!");
        }

        public static async Task<bool> ValidateSelf(ClientSocket socket, Client client)
        {
            var header = new HeaderReq(-1, RequestType.USER_VERIFY);
            var body   = new SData_Long(client.UserID);
            var toSend = new VOTP(header, body);

            var result = await RequestPackageHandler<SecData_Key_Aes>(socket, toSend);

            result.DecryptData(client.UserKey);

            var key = result.GetKey();

            socket.SetCommunicationKey(key);

            return true;
        }

        public static async Task<List<long>> RequestAllUserIDs()
        {
            var currentClient = ClientData.CurrentClient ?? throw new Exception("No activ connection!");

            var head = new HeaderReq(ClientData.CurrentClient.UserID, RequestType.PUBLIC_USER_GET_ID_LIST);
            var package = new VOTP(head);
            var resPackage = await (ClientData.GetConnection(ClientData.CurrentConnection ?? throw new Exception("No activ connection")) ?? throw new Exception("No activ connection")).EnqueueItem(package);

            if (resPackage.Header is not HeaderAck resHeader) throw new Exception("Server didn't responded correctly!");
            if (resHeader.Successful is false) throw new Exception("Server couldn't responded correctly!");

            if (resPackage.Body is not AData_Long resBody) throw new Exception("Server didn't responded correctly!");

            return new(resBody.Data);
        }

        public static async Task<bool> TryGettingUserAsync(ClientSocket socket, Client sender, long targetID)
        {
            var header = new HeaderReq(sender.UserID, RequestType.PUBLIC_USER_GET);
            var body   = new SData_Long(targetID);
            var toSend = new VOTP(header, body);

            var result = await RequestPackageHandler<PublicClient>(socket, toSend);

            return sender.AppendOrOverridePublicClint(result);
        }

        public static async Task<long> GetChatID(ClientSocket socket, long userID)
        {
            if (userID <= 0) return -1;

            var header  = new HeaderReq(userID, RequestType.PRIVAT_CHAT_REGISTER);
            var package = new VOTP(header);

            var result = await RequestPackageHandler<SData_Long>(socket, package);

            return result.Data;
        }

        public static async Task<bool> InviteUserToGroupAsync(ClientSocket socket, Client client, PublicClient pubClient, PrivatChat chat, DataHandling handling)
        {
            var targetKey = (pubClient.Key ?? throw new PublicKeyMissingExeption()).PublicKey;


            if (chat.CryptedReciver >= 0 && chat.CryptedReciver != pubClient.UserID)
            {
                return false;
            }

            // Tell the Server that target is allowed to Join the Group
            {
                var header = new HeaderReq(client.UserID, RequestType.PRIVAT_CHAT_INVITE_USER);
                var body   = new AbsData_Invite(client.UserID, pubClient.UserID, chat.ChatID);
                var toSend = new VOTP(header, body);

                await RequestPackageHandler(socket, toSend);
            }
            // Send the target the inventation
            {
                var header  = new HeaderStd(client.UserID, pubClient.UserID, 0);
                var body    = chat;
                if (body.CryptedReciver != pubClient.UserID) body.EncryptData(targetKey, pubClient.UserID);
                var toStash = new VOTP(header, body);

                var pheader = new HeaderReq(client.UserID, RequestType.STASH_ADD);
                var pbody   = new StashData_Add(toStash.Serialize(), pubClient.UserID, DateTime.Now.AddDays(30)); 
                var toSend  = new VOTP(pheader, pbody);


                var receipt = await RequestPackageHandler<AbsData_Receipt>(socket, toSend);
            }
            return true;
        }

        public static async Task<(long stashID, long receiptID)> AddStashMessage(ClientSocket socket, Client client, long stashID, string toStash, DateTime? expires = null, DataHandling handling = DataHandling.NONE)
        {
            expires ??= DateTime.Now.AddDays(30);

            var header = new HeaderReq(client.UserID, RequestType.STASH_ADD);
            var body   = new StashData_Add(toStash, stashID, expires.Value, handling);
            var toSend = new VOTP(header, body);

            var receipt = await RequestPackageHandler<AbsData_Receipt>(socket, toSend);

            return (receipt.TargetID, receipt.ReceiptID);
        } 

        public static async Task AcceptInvite(ClientSocket socket, Client client, long chatID)
        {
            var header = new HeaderReq(client.UserID, RequestType.PRIVAT_CHAT_INVITE_ACCEPT);
            var body   = new AbsData_InviteAccept(chatID);
            var toSend = new VOTP(header, body);

            await RequestPackageHandler(socket, toSend);
        }

        public static async Task<List<long>> GetStashReceiptIDList(ClientSocket socket, Client client, long stashID)
        {
            var header = new HeaderReq(client.UserID, RequestType.STASH_LIST);
            var body   = new SData_Long(stashID);
            var toSend = new VOTP(header, body);

            var result = await RequestPackageHandler<AData_Long>(socket, toSend);

            return new(result.Data);
        }

        public static async Task<StashMessage> GetStashMessage(ClientSocket socket, Client client, long stashID, long requestID)
        {
            var header = new HeaderReq(client.UserID, RequestType.STASH_GET);
            var body   = new AbsData_Receipt(stashID, requestID);
            var toSend = new VOTP(header, body);

            var result = await RequestPackageHandler<StashData>(socket, toSend);

            return result.Data;
        }

        public static async Task RemoveStashMessage(ClientSocket socket, Client client, long stashID, long requestID)
        {
            var header = new HeaderReq(client.UserID, RequestType.STASH_DELETE);
            var body   = new AbsData_Receipt(stashID, requestID);
            var toSend = new VOTP(header, body);

            await RequestPackageHandler(socket, toSend);
        }
    }
}