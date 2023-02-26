using Voice_of_Time_Server.RequestExecuter.Interface;
using Voice_of_Time_Server.Shared;
using Voice_of_Time_Server.Transfer;
using VoTCore;
using VoTCore.Communication.Extra;
using VoTCore.Package.AbsData;
using VoTCore.Package.Header;
using VoTCore.Package.Interfaces;
using VoTCore.Package.SData;

/**
 * @author      - Timeplex
 * 
 * @created     - 18.02.2023
 * 
 * @last_change - 19.02.2023
 */
namespace Voice_of_Time_Server.RequestExecuter
{
    // TODO: Rework of chat and user differenztiation
    /// <summary>
    /// Function for deleting a message from a stash
    /// </summary>
    internal class StashDelete : IServerRequestExecuter
    {
        bool IServerRequestExecuter.ExecuteOnlyIfVerified => true;

        (IVOTPHeader, IVOTPBody?)? IServerRequestExecuter.ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket)
        {
            if (body is not AbsData_Receipt receiptBody)
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.WRONG_BODY_TYPE, $"No \"{nameof(AbsData_Receipt)}\" was send!"));
            }

            var targetStashID = receiptBody.TargetID;

            // Check if Target is Soruce. If not check if user has permissions to access the Target
            ChatUserState chatUserState = 0;
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

                chatUserState = ServerData.server.GetChatMember(targetStashID, socket.UserID);

                if (chatUserState.HasFlag(ChatUserState.MEMBER))
                {
                    return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.STASH_NO_PERMISSIONS, $"You don't have the permissions to access Stash:{targetStashID}!"));
                }
            }

            var message = ServerData.server.StashMessageGet(targetStashID, receiptBody.ReceiptID);

            if (message is null)
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.STASH_NO_MESSAGE_UNDER_ID, $"No message under ReceiptID:{receiptBody.ReceiptID}!"));
            }

            // If message comes from a chat check if the user is the author of the message or has the nessatry permissions to delete it anyways
            if(targetStashID != socket.UserID && message.AuthorID != socket.UserID)
            {
                if (!chatUserState.HasFlag(ChatUserState.MODERATOR) && !chatUserState.HasFlag(ChatUserState.ADMIN))
                {
                    return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.CHAT_NO_PERMISSIONS, "You don't have the permissions to access this messsage!"));
                }
            }

            ServerData.server.StashMessageRemove(targetStashID, receiptBody.ReceiptID);


            return (new HeaderAck(true), null);
        }
    }
}
