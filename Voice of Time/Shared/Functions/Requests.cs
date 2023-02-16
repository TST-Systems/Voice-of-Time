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

/**
 * @author      - Timeplex
 * 
 * @created     - 10.02.2023
 * 
 * @last_change - 12.02.2023
 */
namespace Voice_of_Time.Shared.Functions
{
    internal static class Requests
    {
        #region Process Helper
        private static async Task<T> RequestPackageHandler<T>(ClientSocket socket, VOTP package) where T : IVOTPBody
        {
            var resultPackage = await socket.EnqueueItem(package);

            // Check if Header is ok
            if (resultPackage.Header is not HeaderAck resHeader) throw new Exception("Server didn't responded correctly!");
            if (resHeader.Successful is false)
            {
                string additionalInfo = "";

                if (resultPackage.Body is SData_String messageWrapper) additionalInfo += "\n" + messageWrapper.Data;

                throw new Exception("Server coudn't Process the Request!" + additionalInfo);
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
            var header = new HeaderReq(userID, RequestType.IDENTITY);
            var toSend = new VOTP(header);

            var result = await RequestPackageHandler<SData_Guid>(socket, toSend);

            return result.Data;
        }

        public static async Task<RSA> KeyExchangeWithServer(ClientSocket socket, RSA key, long userID = -1)
        {
            var header = new HeaderReq(userID, RequestType.KEY_EXCHANGE, 0);
            var body   = new SecData_Key_RSA(key, userID);

            var toSend = new VOTP(header, body);

            var result = await RequestPackageHandler<SecData_Key_RSA>(socket, toSend);

            return result.GetKey();
        }

        public static async Task OpenSecureCommunication(ClientSocket socket, RSA decryptionKey, long userID = -1)
        {
            var header = new HeaderReq(userID, RequestType.COMM_KEY);
            var toSend = new VOTP(header);

            var result = await RequestPackageHandler<SecData_Key_Aes>(socket, toSend);

            result.DecryptData(decryptionKey);

            var key = result.GetKey();

            socket.SetCommunicationKey(key);
        }
            
        public static async Task<long> RegisterClient(ClientSocket socket)
        {
            var header = new HeaderReq(-1, RequestType.REGISTRATION);
            var toSend = new VOTP(header);

            var result = await RequestPackageHandler<SData_Long>(socket, toSend);

            return result.Data;
        }

        public async static Task SetUsername(ClientSocket socket, string username, long userID)
        {
            var header = new HeaderReq(userID, RequestType.SET_USERNAME);
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
            var header = new HeaderReq(client.UserID, RequestType.VERIFY);
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

            var head = new HeaderReq(ClientData.CurrentClient.UserID, RequestType.GET_USERID_LIST);
            var package = new VOTP(head);
            var resPackage = await (ClientData.GetConnection(ClientData.CurrentConnection ?? throw new Exception("No activ connection")) ?? throw new Exception("No activ connection")).EnqueueItem(package);

            if (resPackage.Header is not HeaderAck resHeader) throw new Exception("Server didn't responded correctly!");
            if (resHeader.Successful is false) throw new Exception("Server couldn't responded correctly!");

            if (resPackage.Body is not AData_Long resBody) throw new Exception("Server didn't responded correctly!");

            return new(resBody.Data);
        }

        public static async Task<bool> TryGettingUserAsync(ClientSocket socket, Client sender, long targetID)
        {
            var header = new HeaderReq(sender.UserID, RequestType.GET_PUBLIC_USER);
            var body   = new SData_Long(targetID);
            var toSend = new VOTP(header, body);

            var result = await RequestPackageHandler<PublicClient>(socket, toSend);

            return sender.AppendOrOverridePublicClint(result);
        }

        public static async Task<long> GetChatID(ClientSocket socket, long userID)
        {
            if (userID <= 0) return -1;

            var header  = new HeaderReq(userID, RequestType.REGISTER_PRIVAT_CHAT);
            var package = new VOTP(header);

            var result = await RequestPackageHandler<SData_Long>(socket, package);

            return result.Data;
        }

        public static async Task<bool> InviteUserToGroupAsync(ClientSocket socket, Client client, PublicClient pubClient, PrivatChat chat, DataHandling handling)
        {
            var targetKey = (pubClient.PublicKey ?? throw new PublicKeyMissingExeption()).PublicKey;

            // Tell the Server that target is allowed to Join the Group
            {
                var header = new HeaderReq(client.UserID, RequestType.INVITE_USER_PRIVATCHAT);
                var body   = new AbsData_Invite(client.UserID, pubClient.UserID, chat.ChatID);
                var toSend = new VOTP(header, body);

                await RequestPackageHandler(socket, toSend);
            }
            /*
            // Send the target the inventation
            {
                var header = new HeaderStash(client.UserID, pubClient.UserID, DateTime.Now.AddDays(30), handling);
                var body = chat;
                if (body.CryptedReciver >= 0 && body.CryptedReciver != pubClient.UserID)
                {
                    return false;
                }
                if (body.CryptedReciver != pubClient.UserID) body.EncryptData(targetKey, pubClient.UserID);

                var toSend = new VOTP(header, body);

                await RequestPackageHandler<SData_Long>(socket, toSend);
            }
            */
            return true;
        }
    }
}