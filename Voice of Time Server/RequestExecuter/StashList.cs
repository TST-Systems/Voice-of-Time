using Voice_of_Time_Server.RequestExecuter.Interface;
using Voice_of_Time_Server.Shared;
using Voice_of_Time_Server.Transfer;
using VoTCore;
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
    /// <summary>
    /// Function for listing all receiptIDs of a single stash
    /// </summary>
    internal class StashList : IServerRequestExecuter
    {
        bool IServerRequestExecuter.ExecuteOnlyIfVerified => true;

        (IVOTPHeader, IVOTPBody?)? IServerRequestExecuter.ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket)
        {
            if (body is not SData_Long receiptBody)
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.WRONG_BODY_TYPE, $"No \"{nameof(AbsData_Receipt)}\" was send!"));
            }

            var targetStashID = receiptBody.Data;

            // Check if Target is Soruce. If not check if user has permissions to access the Target
            if (targetStashID != socket.UserID)
            {
                if (ServerData.server.UserExists(targetStashID))
                {
                    return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.STASH_NO_PERMISSIONS, $"You don't have the permissions to access Stash:{targetStashID}!"));
                }

                if (!ServerData.server.ChatExists(targetStashID))
                {
                    return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.CHAT_DOES_NOT_EXISTS, $"Stash {targetStashID} does not exists!"));
                }

                var chatUserState = ServerData.server.GetChatMember(targetStashID, socket.UserID);

                if ((chatUserState & ChatUserState.MEMBER) != ChatUserState.MEMBER)
                {
                    return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.STASH_NO_PERMISSIONS, $"You don't have the permissions to access Stash:{targetStashID}!"));
                }
            }

            var messageIDs = ServerData.server.StashMessageList(targetStashID);

            return (new HeaderAck(true), new AData_Long(messageIDs));
        }
    }
}
