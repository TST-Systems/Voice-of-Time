using Voice_of_Time_Server.RequestExecuter.Interface;
using Voice_of_Time_Server.Shared;
using Voice_of_Time_Server.Transfer;
using VoTCore;
using VoTCore.Package.Header;
using VoTCore.Package.Interfaces;
using VoTCore.Package.SData;
using VoTCore.Package.SecData;

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
    /// <para>Function for exchanging public keys with the server.</para>
    /// <para>Client->Server => Server->Client</para>
    /// The public key will be set onto the socket and also be updatet for the user if he is verified
    /// </summary>
    internal class ServerPublicKeyExchange : IServerRequestExecuter
    {
        public bool ExecuteOnlyIfVerified => false;

        (IVOTPHeader, IVOTPBody?)? IServerRequestExecuter.ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket)
        {
            if(body is not SecData_Key_RSA rsaBody)
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.WRONG_BODY_TYPE, "You also need to share your Public Key!"));
            }

            var userKey = rsaBody.GetKey();

            if (socket.CommunicationVerified)
            {
                ServerData.server.ChangeUserKey(socket.UserID, userKey);
            }

            socket.UserPubKey = userKey;

            return (new HeaderAck(true), new SecData_Key_RSA(ServerData.server.ServerKey, 0));
        }
    }
}
