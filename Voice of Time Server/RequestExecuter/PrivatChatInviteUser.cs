using Voice_of_Time_Server.RequestExecuter.Interface;
using Voice_of_Time_Server.Shared;
using Voice_of_Time_Server.Transfer;
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
    internal class PrivatChatInviteUser : IServerRequestExecuter
    {
        bool IServerRequestExecuter.ExecuteOnlyIfVerified => true;

        (IVOTPHeader, IVOTPBody?)? IServerRequestExecuter.ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket)
        {
            if (body is not AbsData_Invite invite)
            {
                return(new HeaderAck(false), new SData_Exception($"Wrong Body! Need to be a {nameof(AbsData_Invite)}"));
            }
            if (socket.UserID != invite.SourceID)
            {
                return (new HeaderAck(false), new SData_Exception($"UserID:{socket.UserID} is not the same as SourceID:{invite.SourceID}"));
            }
            if (!ServerData.server.UserExists(invite.TargetID))
            {
                return (new HeaderAck(false), new SData_Exception($"UserID:{invite.TargetID} is not a User of this Server!"));
            }
            if (!ServerData.server.ChatExists(invite.ChatID))
            {
                return (new HeaderAck(false), new SData_Exception($"Chat with the ID:{invite.ChatID} is unknwon!"));
            }

            ChatUserState UserChatState   = ServerData.server.GetChatMember(invite.ChatID, socket.UserID);
            ChatUserState TargetChatState = ServerData.server.GetChatMember(invite.ChatID, invite.TargetID);

            if (TargetChatState != ChatUserState.NONE) // TODO: Detailed checks
            {
                return (new HeaderAck(false), new SData_Exception($"Taget user is alrady part of the Chat!"));
            }
            if (UserChatState == ChatUserState.NONE || (UserChatState & ChatUserState.BLOCKED) != 0)
            {
                return (new HeaderAck(false), new SData_Exception($"You are not a member of this chat!"));
            }
            if ((UserChatState & ChatUserState.ADMIN) != 0 && (UserChatState & ChatUserState.MODERATOR) != 0)
            {
                return (new HeaderAck(false), new SData_Exception($"You don't have the nessesary rights to do that!"));
            }

            ServerData.server.AddChatUser(invite.ChatID, invite.TargetID, ChatUserState.INVITED);
            return (new HeaderAck(true), null);
        }
    }
}
