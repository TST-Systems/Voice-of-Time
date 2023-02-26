using Voice_of_Time_Server.RequestExecuter.Interface;
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
    /// Function to secure a socket and its communication with a Aes key on both sides
    /// </summary>
    internal class CommunicationGetKeyAndSecure : IServerRequestExecuter
    {
        bool IServerRequestExecuter.ExecuteOnlyIfVerified => false;

        (IVOTPHeader, IVOTPBody?)? IServerRequestExecuter.ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket)
        {
            if (socket.SecureCommunicationEnabled)
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.COMMUNICATION_ALREADY_SECURE, "You are already secured the communication!"));
            }

            if (socket.UserPubKey is null)
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.COMMUNICATION_NO_PUBLIC_KEY, "You need to verify OR exchange public keys first!"));
            }

            socket.RequestEncryption = true;

            // Send the Aes kes to user and encrypet it with the public key of the user
            var toSendBody = new SecData_Key_Aes(socket.CommunicationKey, 0);
            toSendBody.EncryptData(socket.UserPubKey, socket.UserID);

            return (new HeaderAck(true), toSendBody);
        }
    }
}
