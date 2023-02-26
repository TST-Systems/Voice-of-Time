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
    /// <summary>
    /// Function for register a new user. A username is a that stage not nessaray and can be set later <see cref="UserSetUsername"/>
    /// </summary>
    internal class UserRegister : IServerRequestExecuter 
    {
        bool IServerRequestExecuter.ExecuteOnlyIfVerified => false;

        (IVOTPHeader, IVOTPBody?)? IServerRequestExecuter.ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket)
        {
            if(socket.CommunicationVerified || socket.UserID != -1)
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.COMMUNICATION_ALREADY_VERIFIED, "You are alredy logged in!"));
            }
            if(!socket.SecureCommunicationEnabled || socket.UserPubKey is null)
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.COMMUNICATION_NOT_SECURE, "You need to first open a secure communication!"));
            }

            var userID = ServerData.server.AddUser(socket.UserPubKey, "");

            socket.CommunicationVerified = true;
            socket.UserID                = userID;

            return (new HeaderAck(true), new SData_Long(userID));
        }
    }
}
