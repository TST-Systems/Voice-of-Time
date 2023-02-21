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
namespace Voice_of_Time_Server.RequestExecuter.Interface
{
    internal interface IServerRequestExecuter
    {
        public bool ExecuteOnlyIfVerified { get; }
        public (IVOTPHeader, IVOTPBody?)? ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket);
    }
}