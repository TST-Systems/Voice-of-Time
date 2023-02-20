using Voice_of_Time_Server.RequestExecuter.Interface;
using Voice_of_Time_Server.Shared;
using Voice_of_Time_Server.Transfer;
using VoTCore;
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
    internal class StashAdd : IServerRequestExecuter
    {
        bool IServerRequestExecuter.ExecuteOnlyIfVerified => true;

        (IVOTPHeader, IVOTPBody?)? IServerRequestExecuter.ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket)
        {
            // Check if User has send somthing to store
            if (body is not StashData stashAddBody)
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.WRONG_BODY_TYPE, "No data to store was send!"));
            }
            var messageToStore = stashAddBody.Data;

            // Check if user or chat exists
            var target = messageToStore.TargetID;
            if(target <= 0) return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.USER_INVALID, $"Invalid target: {target}!"));
            if (!ServerData.server.UserExists(target) && !ServerData.server.ChatExists(target))
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.ID_DOES_NOT_EXISTS, $"Target unknown: {target}!"));
            }

            // Store the message
            long receiptID = ServerData.server.StashMessage(target, socket.UserID, messageToStore.Message, messageToStore.Expires, messageToStore.MessageHandling);

            return (new HeaderAck(true), new AbsData_Receipt(target, receiptID));
        }
    }
}
