using Voice_of_Time_Server.RequestExecuter.Interface;
using Voice_of_Time_Server.Shared;
using Voice_of_Time_Server.Transfer;
using VoTCore;
using VoTCore.Communication.Extra;
using VoTCore.Data;
using VoTCore.Package.AbsData;
using VoTCore.Package.Header;
using VoTCore.Package.Interfaces;
using VoTCore.Package.SData;
using VoTCore.Package.StashData;

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
    /// Function for getting a message from a stash
    /// </summary>
    internal class StashGet : IServerRequestExecuter
    {
        bool IServerRequestExecuter.ExecuteOnlyIfVerified => true;

        (IVOTPHeader, IVOTPBody?)? IServerRequestExecuter.ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket)
        {
            if(body is not AbsData_Receipt receiptBody)
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.WRONG_BODY_TYPE, $"No \"{nameof(AbsData_Receipt)}\" was send!"));
            }

            var targetStashID = receiptBody.TargetID;

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

            var message = ServerData.server.StashMessageGet(targetStashID, receiptBody.ReceiptID);

            if (message is null)
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.STASH_NO_MESSAGE_UNDER_ID, $"No message under ReceiptID:{receiptBody.ReceiptID}!"));
            }

            return (new HeaderAck(true), new StashData(message));
        }
    }
}
