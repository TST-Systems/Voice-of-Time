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
    internal class UserRegister : IServerRequestExecuter 
    {
        bool IServerRequestExecuter.ExecuteOnlyIfVerified => false;

        (IVOTPHeader, IVOTPBody?)? IServerRequestExecuter.ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket)
        {
            if(socket.CommunicationVerified || socket.UserID != -1)
            {
                return (new HeaderAck(false), new SData_Exception(new Exception("You are alredy logged in!")));
            }
            if(!socket.SecureCommunicationEnabled || socket.UserPubKey is null)
            {
                return (new HeaderAck(false), new SData_Exception(new Exception("You need to first open a secure communication!")));
            }

            var userID = ServerData.server.AddUser(socket.UserPubKey, "");

            socket.CommunicationVerified = true;
            socket.UserID                = userID;

            return (new HeaderAck(true), new SData_Long(userID));
        }
    }
}
