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
 * @created     - 19.02.2023
 * 
 * @last_change - 19.02.2023
 */
namespace Voice_of_Time_Server.RequestExecuter
{
    internal class PrivatChatInviteAccept : IServerRequestExecuter
    {
        bool IServerRequestExecuter.ExecuteOnlyIfVerified => true;

        (IVOTPHeader, IVOTPBody?)? IServerRequestExecuter.ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket)
        {
            if (body is not AbsData_InviteAccept invAccBody)
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.WRONG_BODY_TYPE, $"Body need to be {nameof(AbsData_InviteAccept)}"));
            }

            if (!ServerData.server.ChatExists(invAccBody.Data))
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.CHAT_DOES_NOT_EXISTS, message: $"Chat unknown: {invAccBody.Data}"));
            }

            var userChatSate = ServerData.server.GetChatMember(invAccBody.Data, socket.UserID);

            if (!userChatSate.HasFlag(ChatUserState.INVITED) || userChatSate.HasFlag(ChatUserState.BLOCKED))
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.CHAT_NOT_INVITED, message: $"You are not invited to: {invAccBody.Data}"));
            }

            userChatSate |=  ChatUserState.MEMBER;
            userChatSate &= ~ChatUserState.INVITED;

            ServerData.server.UpdateChatMemberState(invAccBody.Data, socket.UserID, userChatSate);

            return (new HeaderAck(true), null);
        }
    }
}
