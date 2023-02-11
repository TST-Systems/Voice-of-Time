﻿using Voice_of_Time.Transfer;
using VoTCore.Package.Header;
using VoTCore.Package.SData;
using VoTCore.Package;
using System.Security.Cryptography;
using VoTCore.Package.SecData;
using Voice_of_Time.User;
using VoTCore.Package.AData;
using VoTCore.Package.Interfaces;
using VoTCore.Exeptions;

/**
 * @author      - Timeplex
 * 
 * @created     - 10.02.2023
 * 
 * @last_change - 11.02.2023
 */
namespace Voice_of_Time.Shared.Functions
{
    internal static class Requests
    {
        #region Process Helper
        private static async Task<T> RequestPackageHandler<T>(CSocketHold socket, VOTP package) where T : IVOTPBody
        {
            if (!socket.IsRunning) throw new Exception("Connection is currently closed!");

            var serialized = package.Serialize();

            var serverAnser = await socket.EnqueueItem(serialized);

            var resultPackage = new VOTP(serverAnser);

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

        private static async Task RequestPackageHandler(CSocketHold socket, VOTP package)
        {
            try
            {
                _ = await RequestPackageHandler<IVOTPBody>(socket, package);
                throw new Exception("Server respnded with not requested Data!");
            }
            catch (PackageBodyNullException) { }
        }
        #endregion

        public static async Task<Guid> RequestServerID(CSocketHold socket, long userID = -1)
        {
            var header = new HeaderReq(userID, RequestType.IDENTITY);
            var toSend = new VOTP(header);

            var result = await RequestPackageHandler<SData_Guid>(socket, toSend);

            return result.Data;
        }

        public static async Task<RSA> KeyExchangeWithServer(CSocketHold socket, RSA key, long userID = -1)
        {
            var header = new HeaderReq(userID, RequestType.KEY_EXCHANGE, 0);
            var body   = new SecData_Key_RSA(key, userID);

            var toSend = new VOTP(header, body);

            var result = await RequestPackageHandler<SecData_Key_RSA>(socket, toSend);

            return result.GetKey();
        }

        public static async Task OpenSecureCommunication(CSocketHold socket, RSA decryptionKey, long userID = -1)
        {
            var header = new HeaderReq(userID, RequestType.COMM_KEY);
            var toSend = new VOTP(header);

            var result = await RequestPackageHandler<SecData_Key_Aes>(socket, toSend);

            result.DecryptData(decryptionKey);

            var key = result.GetKey();

            socket.SetCommunicationKey(key);
        }
            
        public static async Task<long> RegisterClient(CSocketHold socket)
        {
            var header = new HeaderReq(-1, RequestType.REGISTRATION);
            var toSend = new VOTP(header);

            var result = await RequestPackageHandler<SData_Long>(socket, toSend);

            return result.Data;
        }

        public async static Task SetUsername(CSocketHold socket, string username, long userID)
        {
            var header = new HeaderReq(userID, RequestType.SET_USERNAME);
            var body   = new SData_String(username);
            var toSend = new VOTP(header, body);

            await RequestPackageHandler(socket, toSend);
        }

        public static async Task TestConnection(CSocketHold socket, Guid serverID, long userID)
        {
            var newServerID = await Requests.RequestServerID(socket, userID);

            if (serverID.CompareTo(newServerID) != 0) throw new Exception("Connection Invalid!");
        }

        public static async Task<bool> ValidateSelf(CSocketHold socket, Client client)
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
            var result = await (ClientData.GetConnection(ClientData.CurrentConnection ?? throw new Exception("No activ connection")) ?? throw new Exception("No activ connection")).EnqueueItem(package.Serialize());
            var resPackage = new VOTP(result);

            if (resPackage.Header is not HeaderAck resHeader) throw new Exception("Server didn't responded correctly!");
            if (resHeader.Successful is false) throw new Exception("Server couldn't responded correctly!");

            if (resPackage.Body is not AData_Long resBody) throw new Exception("Server didn't responded correctly!");

            return new(resBody.Data);
        }

        public static async Task<bool> TryGettingUserAsync(CSocketHold socket, Client sender, long targetID)
        {
            var header = new HeaderReq(sender.UserID, RequestType.GET_PUBLIC_USER);
            var body   = new SData_Long(targetID);
            var toSend = new VOTP(header, body);

            var result = await RequestPackageHandler<SecData_ClientShare>(socket, toSend);

            var UserEntry = result.GetPublicClient();

            return sender.AppendOrOverridePublicClint(UserEntry);
        }

        public static async Task<long> GetChatID(CSocketHold socket, long userID)
        {
            if (userID <= 0) return -1;

            var header  = new HeaderReq(userID, RequestType.REGISTER_PRIVAT_CHAT);
            var package = new VOTP(header);

            var result = await RequestPackageHandler<SData_Long>(socket, package);

            return result.Data;
        }
    }
}
