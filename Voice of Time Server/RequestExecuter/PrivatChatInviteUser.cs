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
 * @last_change - 18.02.2023
 */
namespace Voice_of_Time_Server.RequestExecuter
{
    /// <summary>
    /// Function for inviting a user into a existing chat
    /// </summary>
    internal class PrivatChatInviteUser : IServerRequestExecuter
    {
        bool IServerRequestExecuter.ExecuteOnlyIfVerified => true;

        (IVOTPHeader, IVOTPBody?)? IServerRequestExecuter.ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket)
        {
            if (body is not AbsData_Invite invite)
            {
                return(new HeaderAck(false), new SData_InternalException(InternalExceptionCode.WRONG_BODY_TYPE, message: $"Wrong Body! Need to be a {nameof(AbsData_Invite)}"));
            }
            if (socket.UserID != invite.SourceID)
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.SOURCE_UNEQUAL_USER, message: $"UserID:{socket.UserID} is not the same as SourceID:{invite.SourceID}"));
            }
            if (!ServerData.server.UserExists(invite.TargetID))
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.USER_DOES_NOT_EXISTS, message: $"UserID:{invite.TargetID} is not a User of this Server!"));
            }
            if (!ServerData.server.ChatExists(invite.ChatID))
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.CHAT_DOES_NOT_EXISTS, message: $"Chat with the ID:{invite.ChatID} is unknwon!"));
            }

            ChatUserState UserChatState   = ServerData.server.GetChatMember(invite.ChatID, socket.UserID);
            ChatUserState TargetChatState = ServerData.server.GetChatMember(invite.ChatID, invite.TargetID);

            if (TargetChatState != ChatUserState.NONE) // TODO: Detailed checks
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.CHAT_ALREADY_MEMBER, message: $"Taget user is alrady part of the Chat!"));
            }
            if (UserChatState == ChatUserState.NONE || UserChatState.HasFlag(ChatUserState.BLOCKED))
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.CHAT_NOT_MEMBER, message: $"You are not a member of this chat!"));
            }
            if (!UserChatState.HasFlag(ChatUserState.ADMIN) && !UserChatState.HasFlag(ChatUserState.MODERATOR)) // Inviter needs some privileges to invite someone. Not everyone can invite anyone
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.CHAT_NO_PERMISSIONS, message: $"You don't have the nessesary rights to do that!"));
            }

            ServerData.server.AddChatUser(invite.ChatID, invite.TargetID, ChatUserState.INVITED); //TODO: IDEA: possible instant privileges like invite as moderator or so...
            return (new HeaderAck(true), null);
        }
    }
}
