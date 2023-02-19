using Voice_of_Time_Server.RequestExecuter.Interface;
using Voice_of_Time_Server.Shared;
using Voice_of_Time_Server.Transfer;
using VoTCore.Communication.Extra;
using VoTCore.Package.AbsData;
using VoTCore.Package.AData;
using VoTCore.Package.Header;
using VoTCore.Package.Interfaces;
using VoTCore.Package.SData;

/**
 * @author      - Timeplex
 * 
 * @created     - 19.02.2023
 * 
 * @last_change - 19.02.2023
 */
namespace Voice_of_Time_Server.RequestExecuter
{
    internal class StashList : IServerRequestExecuter
    {
        bool IServerRequestExecuter.ExecuteOnlyIfVerified => true;

        (IVOTPHeader, IVOTPBody?)? IServerRequestExecuter.ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket)
        {
            if (body is not SData_Long receiptBody)
            {
                return (new HeaderAck(false), new SData_Exception($"No \"{nameof(AbsData_Receipt)}\" was send!"));
            }

            var targetStashID = receiptBody.Data;

            // Check if Target is self
            if (targetStashID != socket.UserID)
            {
                if (ServerData.server.UserExists(targetStashID))
                {
                    return (new HeaderAck(false), new SData_Exception($"You don't have the permissions to access Stash:{targetStashID}!"));
                }

                if (!ServerData.server.ChatExists(targetStashID))
                {
                    return (new HeaderAck(false), new SData_Exception($"Stash {targetStashID} does not exists!"));
                }

                var chatUserState = ServerData.server.GetChatMember(targetStashID, socket.UserID);

                if ((chatUserState & ChatUserState.MEMBER) != ChatUserState.MEMBER)
                {
                    return (new HeaderAck(false), new SData_Exception($"You don't have the permissions to access Stash:{targetStashID}!"));
                }
            }

            var messageIDs = ServerData.server.StashMessageList(targetStashID);

            return (new HeaderAck(true), new AData_Long(messageIDs));
        }
    }
}
