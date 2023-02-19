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
 * @last_change - 19.02.2023
 */
namespace Voice_of_Time_Server.RequestExecuter
{
    internal class StashDelete : IServerRequestExecuter
    {
        bool IServerRequestExecuter.ExecuteOnlyIfVerified => true;

        (IVOTPHeader, IVOTPBody?)? IServerRequestExecuter.ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket)
        {
            if (body is not AbsData_Receipt receiptBody)
            {
                return (new HeaderAck(false), new SData_Exception($"No \"{nameof(AbsData_Receipt)}\" was send!"));
            }

            var targetStashID = receiptBody.TargetID;

            // Check if Target is self
            ChatUserState chatUserState = 0;
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

                chatUserState = ServerData.server.GetChatMember(targetStashID, socket.UserID);

                if (chatUserState.HasFlag(ChatUserState.MEMBER))
                {
                    return (new HeaderAck(false), new SData_Exception($"You don't have the permissions to access Stash:{targetStashID}!"));
                }
            }

            var message = ServerData.server.StashMessageGet(targetStashID, receiptBody.ReceiptID);

            if (message is null || !message.HasValue)
            {
                return (new HeaderAck(false), new SData_Exception($"No message under ReceiptID:{receiptBody.ReceiptID}!"));
            }


            if(message.Value.AuthorID != socket.UserID)
            {
                if (!chatUserState.HasFlag(ChatUserState.MODERATOR) && !chatUserState.HasFlag(ChatUserState.ADMIN))
                {
                    return (new HeaderAck(false), new SData_Exception("You don't have the permissions to access this messsage!"));
                }
            }

            ServerData.server.StashMessageRemove(targetStashID, receiptBody.ReceiptID);


            return (new HeaderAck(true), null);
        }
    }
}
