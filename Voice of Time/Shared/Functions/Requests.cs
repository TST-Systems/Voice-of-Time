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

/**
 * @author      - Timeplex
 * 
 * @created     - 10.02.2023
 * 
 * @last_change - 20.02.2023
 */
namespace Voice_of_Time.Shared.Functions
{
    /// <summary>
    /// Collection of different queries to the server
    /// </summary>
    internal static class Requests
    {
        #region Process Helper
        /// <summary>
        /// A handler that helps with a generalization with the process it takes to send an recive data from a socket
        /// </summary>
        /// <typeparam name="T">Type of expeceted Body</typeparam>
        /// <param name="socket">Socket to communicate</param>
        /// <param name="package">Package to send</param>
        /// <returns>Body of servers anwser</returns>
        /// <exception cref="Exception">Server gave not the right anwser</exception>
        /// <exception cref="PackageBodyNullException">The server did not send any payload (body)</exception>
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

        /// <summary>
        /// Wrapper for the generic version when no body is expeced
        /// </summary>
        /// <param name="socket">Socket to communicate</param>
        /// <param name="package">Package to send</param>
        /// <exception cref="Exception">Server didn't responded as expected</exception>
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

        /// <summary>
        /// Request the UID of a given server
        /// </summary>
        /// <param name="socket">Socket to server</param>
        /// <param name="userID">Own userID, if already verified</param>
        /// <returns>UID of server</returns>
        public static async Task<Guid> RequestServerID(ClientSocket socket, long userID = -1)
        {
            var header = new HeaderReq(userID, RequestType.SERVER_GET_IDENTITY);
            var toSend = new VOTP(header);

            var result = await RequestPackageHandler<SData_Guid>(socket, toSend);

            return result.Data;
        }

        /// <summary>
        /// Request a exchange of public keys with the server
        /// </summary>
        /// <param name="socket">Socket to server</param>
        /// <param name="key">Own public Key to send (Can contain privat parts, witch will be sorted out)</param>
        /// <param name="userID">Own userID, if already verified</param>
        /// <returns>Public key of server</returns>
        public static async Task<RSA> KeyExchangeWithServer(ClientSocket socket, RSA key, long userID = -1)
        {
            var header = new HeaderReq(userID, RequestType.SERVER_PUBLIC_KEY_EXCHANGE, 0);
            var body   = new SecData_Key_RSA(key, userID);

            var toSend = new VOTP(header, body);

            var result = await RequestPackageHandler<SecData_Key_RSA>(socket, toSend);

            return result.GetKey();
        }

        /// <summary>
        /// Request the communication key for the socket and also an activation of a secure communication
        /// </summary>
        /// <param name="socket">Socket to server</param>
        /// <param name="decryptionKey">Own privat key to decrypt the AesKey</param>
        /// <param name="userID">Own userID, if already verified</param>
        public static async Task OpenSecureCommunication(ClientSocket socket, RSA decryptionKey, long userID = -1)
        {
            var header = new HeaderReq(userID, RequestType.COMMUNICATION_GET_KEY_AND_SECURE);
            var toSend = new VOTP(header);

            var result = await RequestPackageHandler<SecData_Key_Aes>(socket, toSend);

            result.DecryptData(decryptionKey);

            var key = result.GetKey();

            socket.SetCommunicationKey(key);
        }

        /// <summary>
        /// Request a regestration. You should not be verified already
        /// </summary>
        /// <param name="socket">Socket to server</param>
        /// <returns>New userID</returns>
        public static async Task<long> RegisterClient(ClientSocket socket)
        {
            var header = new HeaderReq(-1, RequestType.USER_REGISTRATION);
            var toSend = new VOTP(header);

            var result = await RequestPackageHandler<SData_Long>(socket, toSend);

            return result.Data;
        }

        /// <summary>
        /// <para>YOU NEED TO BE VERIFIED TO USE THIS!</para>
        /// Set or change the own username on the server
        /// </summary>
        /// <param name="socket">Socket to server</param>
        /// <param name="username">New username</param>
        /// <param name="userID">Own userID</param>
        /// <returns></returns>
        public async static Task SetUsername(ClientSocket socket, string username, long userID)
        {
            var header = new HeaderReq(userID, RequestType.USER_SET_USERNAME);
            var body   = new SData_String(username);
            var toSend = new VOTP(header, body);

            await RequestPackageHandler(socket, toSend);
        }

        /// <summary>
        /// <para>YOU NEED TO BE VERIFIED TO USE THIS!</para>
        /// Test the enryption of a socket
        /// </summary>
        /// <param name="socket">Socket to server</param>
        /// <param name="serverID">UID of server as comparable</param>
        /// <param name="userID">Own userID</param>
        /// <exception cref="Exception">If UID of sever does not match</exception>
        public static async Task TestConnection(ClientSocket socket, Guid serverID, long userID)
        {
            var newServerID = await Requests.RequestServerID(socket, userID);

            if (serverID.CompareTo(newServerID) != 0) throw new Exception("Connection Invalid!");
        }

        /// <summary>
        /// Alternitve to <see cref="RegisterClient"/>. Log in a existing client to a server.
        /// </summary>
        /// <param name="socket">Socket to server</param>
        /// <param name="client">Client instance for this server</param>
        /// <returns>success</returns>
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

        /// <summary>
        /// <para>YOU NEED TO BE VERIFIED TO USE THIS!</para>
        /// Request all userIDs of all regiserd users on a server
        /// </summary>
        /// <returns>List of all userIDs</returns>
        /// <exception cref="Exception">Server did not response as expected</exception>
        public static async Task<List<long>> RequestAllUserIDs() // TODO: Need rework to match (socket, userID) 
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

        /// <summary>
        /// <para>YOU NEED TO BE VERIFIED TO USE THIS!</para> 
        /// Request all accasable information of anouther client
        /// </summary>
        /// <param name="socket">Socket to server</param>
        /// <param name="sender">Client to server</param>
        /// <param name="targetID">UserID of targeted client</param>
        /// <returns></returns>
        public static async Task<bool> TryGettingUserAsync(ClientSocket socket, Client sender, long targetID)
        {
            var header = new HeaderReq(sender.UserID, RequestType.PUBLIC_USER_GET);
            var body   = new SData_Long(targetID);
            var toSend = new VOTP(header, body);

            var result = await RequestPackageHandler<PublicClient>(socket, toSend);

            return sender.AppendOrOverridePublicClint(result);
        }

        /// <summary>
        /// <para>YOU NEED TO BE VERIFIED TO USE THIS!</para>
        /// Register a new privat chat on the server
        /// </summary>
        /// <param name="socket">Socket of server</param>
        /// <param name="userID">Own userID</param>
        /// <returns>ChatID of new chat</returns>
        public static async Task<long> GetChatID(ClientSocket socket, long userID)
        {
            if (userID <= 0) return -1;

            var header  = new HeaderReq(userID, RequestType.PRIVAT_CHAT_REGISTER);
            var package = new VOTP(header);

            var result = await RequestPackageHandler<SData_Long>(socket, package);

            return result.Data;
        }

        /// <summary>
        /// <para>YOU NEED TO BE VERIFIED TO USE THIS!</para>
        /// Invite a user to a own chat or a chat where you meet the nessaray permissions. 
        /// </summary>
        /// <param name="socket">Socket to server</param>
        /// <param name="client">Client of server</param>
        /// <param name="pubClient">Other client to invite</param>
        /// <param name="chat">Targeted chat to invite the other user to</param>
        /// <param name="handling">Not implemet yet</param> // TODO
        /// <returns>success</returns>
        /// <exception cref="PublicKeyMissingExeption">The outher client has no public key present in its data</exception>
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

        /// <summary>
        /// <para>YOU NEED TO BE VERIFIED TO USE THIS!</para>
        /// Add a message to any stash you have access to
        /// </summary>
        /// <param name="socket">Socket to server</param>
        /// <param name="client">Client of server</param>
        /// <param name="stashID">StashID to access</param>
        /// <param name="toStash">Message to store</param>
        /// <param name="expires">Expiring date of message</param>
        /// <param name="handling">Message handling</param>
        public static async Task<(long stashID, long receiptID)> AddStashMessage(ClientSocket socket, Client client, long stashID, string toStash, DateTime? expires = null, DataHandling handling = DataHandling.NONE)
        {
            expires ??= DateTime.Now.AddDays(30);

            var header = new HeaderReq(client.UserID, RequestType.STASH_ADD);
            var body   = new StashData_Add(toStash, stashID, expires.Value, handling);
            var toSend = new VOTP(header, body);

            var receipt = await RequestPackageHandler<AbsData_Receipt>(socket, toSend);

            return (receipt.TargetID, receipt.ReceiptID);
        }

        /// <summary>
        /// <para>YOU NEED TO BE VERIFIED TO USE THIS!</para>
        /// Accept a invite to a privat chat
        /// </summary>
        /// <param name="socket">Socket to server</param>
        /// <param name="client">Client of server</param>
        /// <param name="chatID">ID of chat to join</param>
        public static async Task AcceptInvite(ClientSocket socket, Client client, long chatID)
        {
            var header = new HeaderReq(client.UserID, RequestType.PRIVAT_CHAT_INVITE_ACCEPT);
            var body   = new AbsData_InviteAccept(chatID);
            var toSend = new VOTP(header, body);

            await RequestPackageHandler(socket, toSend);
        }

        /// <summary>
        /// <para>YOU NEED TO BE VERIFIED TO USE THIS!</para>
        /// Get all IDs of messages stored in a given stash
        /// </summary>
        /// <param name="socket">Socket to server</param>
        /// <param name="client">Client of server</param>
        /// <param name="stashID">ID of stash to access</param>
        /// <returns>List of al receiptIDs</returns>
        public static async Task<List<long>> GetStashReceiptIDList(ClientSocket socket, Client client, long stashID)
        {
            var header = new HeaderReq(client.UserID, RequestType.STASH_LIST);
            var body   = new SData_Long(stashID);
            var toSend = new VOTP(header, body);

            var result = await RequestPackageHandler<AData_Long>(socket, toSend);

            return new(result.Data);
        }

        /// <summary>
        /// <para>YOU NEED TO BE VERIFIED TO USE THIS!</para>
        /// Get one Message of a Stash (handling is not performt automaticly)
        /// </summary>
        /// <param name="socket">Socket to server</param>
        /// <param name="client">Client of server</param>
        /// <param name="stashID">ID of stash to access</param>
        /// <param name="requestID">ID of message to get</param>
        /// <returns>Message and meta data</returns>
        public static async Task<StashMessage> GetStashMessage(ClientSocket socket, Client client, long stashID, long requestID)
        {
            var header = new HeaderReq(client.UserID, RequestType.STASH_GET);
            var body   = new AbsData_Receipt(stashID, requestID);
            var toSend = new VOTP(header, body);

            var result = await RequestPackageHandler<StashData>(socket, toSend);

            return result.Data;
        }
        /// <summary>
        /// <para>YOU NEED TO BE VERIFIED TO USE THIS!</para>
        /// Remove a message from the stash
        /// </summary>
        /// <param name="socket">Socket to server</param>
        /// <param name="client">Client of server</param>
        /// <param name="stashID">ID of stash to access</param>
        /// <param name="requestID">ID of message to delete</param>
        public static async Task RemoveStashMessage(ClientSocket socket, Client client, long stashID, long requestID)
        {
            var header = new HeaderReq(client.UserID, RequestType.STASH_DELETE);
            var body   = new AbsData_Receipt(stashID, requestID);
            var toSend = new VOTP(header, body);

            await RequestPackageHandler(socket, toSend);
        }
    }
}