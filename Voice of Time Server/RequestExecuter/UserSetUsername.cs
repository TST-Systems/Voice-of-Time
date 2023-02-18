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
    internal class UserSetUsername : IServerRequestExecuter
    {
        bool IServerRequestExecuter.ExecuteOnlyIfVerified => true;

        (IVOTPHeader, IVOTPBody?)? IServerRequestExecuter.ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket)
        {
            if (body is not SData_String strBody || strBody.Data is null)
            {
                return (new HeaderAck(false), new SData_Exception("No new username!"));
            }

            ServerData.server.ChangeUserUsername(socket.UserID, strBody.Data);

            return (new HeaderAck(true), null); 
        }
    }
}
