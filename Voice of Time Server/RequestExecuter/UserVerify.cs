using Voice_of_Time_Server.RequestExecuter.Interface;
using Voice_of_Time_Server.Shared;
using Voice_of_Time_Server.Transfer;
using VoTCore;
using VoTCore.Package.Header;
using VoTCore.Package.Interfaces;
using VoTCore.Package.SData;

/**
* @author      - Timeplex
* 
* @created     - 18.02.2023
* 
* @last_change - 18.02.2023
*/
namespace Voice_of_Time_Server.RequestExecuter
{
    // TODO: Possible glitch where a user sends first a verification and then a public key without waiting for the respnse, so that he would be overrideing the pub key and would now have stollen the acount. Still he would not be able to read any chats but he has kicked out the owner from its acounr and can now be someone else. 
    // fix: lock KeyExchange while verifiying
    /// <summary>
    /// Function for verifying self over userID
    /// </summary>
    internal class UserVerify : IServerRequestExecuter
    {
        bool IServerRequestExecuter.ExecuteOnlyIfVerified => false;

        (IVOTPHeader, IVOTPBody?)? IServerRequestExecuter.ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket)
        {
            if (socket.CommunicationVerified || socket.UserID != -1)
            {
                socket.RequestConnectionClose = true;
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.COMMUNICATION_ALREADY_VERIFIED, "You are already verifiyed! Server closes the connection!"));
            }
            if(body is not SData_Long longBody)
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.WRONG_BODY_TYPE, "You need to send your ID!"));
            }

            var expectedUserID = longBody.Data;

            if (!ServerData.server.UserExists(expectedUserID))
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.USER_DOES_NOT_EXISTS, "User unknown! Register first!"));
            }

            socket.UserID = expectedUserID;

            var clientData = ServerData.server.GetUser(expectedUserID) ?? throw new Exception($"User {expectedUserID} is known & unknown at the same time :/");
            socket.UserPubKey = clientData.Key.PublicKey;

            IServerRequestExecuter openSecureCommunication = new CommunicationGetKeyAndSecure();
            var (returnHeader, returnBody) = openSecureCommunication.ExecuteRequest(header, null, socket) ?? throw new Exception("Internal Error");

            socket.CommunicationVerified = true; // <- Expect that user will be able to communicate with the given public key, so if he doesn't have the privat key part he has to close the connection

            return (returnHeader, returnBody);
        }
    }
}
