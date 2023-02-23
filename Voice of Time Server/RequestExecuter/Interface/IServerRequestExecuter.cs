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
    /// <summary>
    /// Interface for all server request executer
    /// </summary>
    internal interface IServerRequestExecuter
    {
        /// <summary>
        /// Only execute if the user was already verfied
        /// </summary>
        public bool ExecuteOnlyIfVerified { get; }
        /// <summary>
        /// Function which that be called if a request fits the requierments
        /// </summary>
        /// <param name="header">Header of the incoming message</param>
        /// <param name="body">Body, if exists, of the incoming message</param>
        /// <param name="socket">Socket for meta inforamtaion aboute the connection and the user</param>
        /// <returns>Parrs of a VOTP or nothing if their was a error</returns>
        public (IVOTPHeader, IVOTPBody?)? ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket);
    }
}