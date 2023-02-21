using Voice_of_Time_Server.RequestExecuter.Interface;
using Voice_of_Time_Server.Shared;
using Voice_of_Time_Server.Transfer;
using VoTCore.Package.Header;
using VoTCore.Package.Interfaces;
using VoTCore.Package.SData;
/**
* @author      - Timeplex
* 
* @created     - 16.02.2023
* 
* @last_change - 17.02.2023
*/
namespace Voice_of_Time_Server.RequestExecuter
{
    /// <summary>
    /// Function for getting the UID of the server
    /// </summary>
    internal class ServerGetIdentitiy : IServerRequestExecuter
    {
        public bool ExecuteOnlyIfVerified => false;

        public (IVOTPHeader, IVOTPBody?)? ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket)
        {
            if (body is not null)
            {
                socket.WriteInfo($"User send unexpected Body: {body.GetType().Name}");
            }

            var sendHeader = new HeaderAck(true);
            var sendBody   = new SData_Guid(ServerData.server.ServerIdentity);

            return (sendHeader, sendBody);
        }
    }
}
