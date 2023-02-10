using Voice_of_Time.Transfer;
using VoTCore.Package.Header;
using VoTCore.Package.SData;
using VoTCore.Package;
using System.Security.Cryptography;
using VoTCore.Package.SecData;
using Voice_of_Time.User;
using VoTCore.Package.AData;

/**
 * @author      - Timeplex
 * 
 * @created     - 10.02.2023
 * 
 * @last_change - 10.02.2023
 */
namespace Voice_of_Time.Shared.Functions
{
    internal static class Requests
    {
        public static async Task<Guid> RequestServerID(CSocketHold socket, long userID = -1)
        {
            var header  = new HeaderReq(userID, RequestType.IDENTITY);

            var toSend  = new VOTP(header).Serialize();

            var recived = await socket.EnqueueItem(toSend);

            var body    = new VOTP(recived).Body;

            if (body is not SData_Guid sDGuid) throw new Exception("Sever didn't replyed correctly");

            return sDGuid.Data;
        }

        public static async Task<RSA> KeyExchangeWithServer(CSocketHold socket, RSA key, long userID = -1)
        {
            var header = new HeaderReq(userID, RequestType.KEY_EXCHANGE, 0);
            var body   = new SecData_Key_RSA(key, userID);

            var toSend = new VOTP(header, body);

            var recive = await socket.EnqueueItem(toSend.Serialize());
            var result = new VOTP(recive);

            if (result.Header is not HeaderAck resHeader) throw new Exception("Header wasn't expected!");
            if (!resHeader.Successful) throw new Exception("Server didn't responded correctly");

            if (result.Body is not SecData_Key_RSA resBody) throw new Exception("Body wasn't expected!");

            return resBody.GetKey();
        }

        public static async Task OpenSecureCommunication(CSocketHold socket, RSA decryptionKey, long userID = -1)
        {
            var header = new HeaderReq(userID, RequestType.COMM_KEY);
            var toSend = new VOTP(header);
            var recive = await socket.EnqueueItem(toSend.Serialize());
            var result = new VOTP(recive);

            if (result.Header is not HeaderAck resHeader) throw new Exception("Header wasn't expected!");
            if (result.Body is not SecData_Key_Aes resBody) throw new Exception("Body wasn't expected!");

            if (!resHeader.Successful) throw new Exception("Server didn't responded correctly");

            resBody.DecryptData(decryptionKey);

            var key = resBody.GetKey();

            socket.SetCommunicationKey(key);
        }

        public static async Task<long> RegisterClient(CSocketHold socket)
        {
            var header = new HeaderReq(-1, RequestType.REGISTRATION);
            var toSend = new VOTP(header);
            var recive = await socket.EnqueueItem(toSend.Serialize());
            var result = new VOTP(recive);

            if (result.Header is not HeaderAck resHeader) throw new Exception("Header wasn't expected!");
            if (result.Body is not SData_Long resBody) throw new Exception("Body wasn't expected!");

            if (!resHeader.Successful) throw new Exception("Server didn't responded correctly");

            return resBody.Data;
        }

        public async static Task SetUsername(CSocketHold socket, string username, long userID)
        {
            var header = new HeaderReq(userID, RequestType.SET_USERNAME);
            var body = new SData_String(username);
            var toSend = new VOTP(header, body);
            var recive = await socket.EnqueueItem(toSend.Serialize());
            var result = new VOTP(recive);

            if (result.Header is not HeaderAck resHeader) throw new Exception("Header wasn't expected!");

            if (!resHeader.Successful) throw new Exception("Server didn't responded correctly");
        }

        public static async Task TestConnection(CSocketHold socket, Guid serverID, long userID)
        {
            var newServerID = await Requests.RequestServerID(socket, userID);

            if (serverID.CompareTo(newServerID) != 0) throw new Exception("Connection Invalid!");
        }

        public static async Task<bool> ValidateSelf(CSocketHold socket, Client c)
        {
            var head = new HeaderReq(c.UserID, RequestType.VERIFY);
            var body = new SData_Long(c.UserID);
            var package = new VOTP(head, body);

            var result = new VOTP(await socket.EnqueueItem(package.Serialize()));

            if (result.Header is not HeaderAck resHeader) throw new Exception("Header wasn't expected!");
            if (result.Body is not SecData_Key_Aes resBody) throw new Exception("Body wasn't expected!");

            if (!resHeader.Successful) return false;

            resBody.DecryptData(c.UserKey);

            var key = resBody.GetKey();

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

        public static async Task<bool> TryGettingUserAsync(long senderID, long targetID)
        {
            if (ClientData.CurrentClient is null || ClientData.CurrentConnection is null) return false;

            var header = new HeaderReq(senderID, RequestType.GET_PUBLIC_USER);
            var body = new SData_Long(targetID);
            var package = new VOTP(header, body);

            var connection = ClientData.GetConnection((Guid)ClientData.CurrentConnection) ?? throw new Exception("Connection is not Registert!");

            var result = new VOTP(await connection.EnqueueItem(package.Serialize()));

            if (result.Header is not HeaderAck resHeader) throw new Exception("Server didn't responded correctly!");
            if (resHeader.Successful is false) return false;

            if (result.Body is null) return false;
            if (result.Body is not SecData_ClientShare resBody) throw new Exception("Server didn't responded correctly!");

            var UserEntry = resBody.GetPublicClient();

            return ClientData.CurrentClient.AppendOrOverridePublicClint(UserEntry);
        }
    }
}
