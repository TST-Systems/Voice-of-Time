using System.Net.Sockets;
using Voice_of_Time_Server.RequestExecuter.Interface;
using Voice_of_Time_Server.Transfer;
using VoTCore.Package.Header;
using VoTCore.Package.Interfaces;
/**
* @author      - Timeplex
* 
* @created     - 16.02.2023
* 
* @last_change - 16.02.2023
*/
namespace Voice_of_Time_Server.RequestExecuter
{
    internal class ServerGetIdentitiy : IServerRequestExecuter
    {
        public bool ExecuteOnlyIfVerified => false;

        public (IVOTPHeader, IVOTPBody?)? ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket)
        {
            throw new NotImplementedException();
        }
    }
}
