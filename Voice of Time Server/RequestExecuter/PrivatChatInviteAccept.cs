using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                return (new HeaderAck(false), new SData_Exception($"Body need to be {nameof(AbsData_InviteAccept)}"));
            }

            if (!ServerData.server.ChatExists(invAccBody.Data))
            {
                return (new HeaderAck(false), new SData_Exception($"Chat unknown: {invAccBody.Data}"));
            }

            var userChatSate = ServerData.server.GetChatMember(invAccBody.Data, socket.UserID);

            if (!userChatSate.HasFlag(ChatUserState.INVITED) || userChatSate.HasFlag(ChatUserState.BLOCKED))
            {
                return (new HeaderAck(false), new SData_Exception($"You are not invited to: {invAccBody.Data}"));
            }

            userChatSate |=  ChatUserState.MEMBER;
            userChatSate &= ~ChatUserState.INVITED;

            ServerData.server.UpdateChatMemberState(invAccBody.Data, socket.UserID, userChatSate);

            return (new HeaderAck(true), null);
        }
    }
}
