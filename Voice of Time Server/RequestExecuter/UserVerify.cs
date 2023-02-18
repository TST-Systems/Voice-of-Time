using Voice_of_Time_Server.RequestExecuter.Interface;
using Voice_of_Time_Server.Shared;
using Voice_of_Time_Server.Transfer;
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
    internal class UserVerify : IServerRequestExecuter
    {
        bool IServerRequestExecuter.ExecuteOnlyIfVerified => false;

        (IVOTPHeader, IVOTPBody?)? IServerRequestExecuter.ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket)
        {
            if (socket.CommunicationVerified || socket.UserID != -1)
            {
                socket.RequestConnectionClose = true;
                return (new HeaderAck(false), new SData_Exception("You are already verifiyed! Server closes the connection!"));
            }
            if(body is not SData_Long longBody)
            {
                return (new HeaderAck(false), new SData_Exception("You need to send your ID!"));
            }

            var expectedUserID = longBody.Data;

            if (!ServerData.server.UserExists(expectedUserID))
            {
                return (new HeaderAck(false), new SData_Exception("User unknown! Register first!"));
            }

            socket.UserID = expectedUserID;

            var clientData = ServerData.server.GetUser(expectedUserID) ?? throw new Exception($"User {expectedUserID} is known & unknown at the same time :/");
            socket.UserPubKey = clientData.Key.PublicKey;

            IServerRequestExecuter openSecureCommunication = new CommunicationGetKeyAndSecure();
            var (returnHeader, returnBody) = openSecureCommunication.ExecuteRequest(header, null, socket) ?? throw new Exception("Internal Error");

            socket.CommunicationVerified = true;

            return (returnHeader, returnBody);
        }
    }
}
