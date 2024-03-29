﻿using Voice_of_Time_Server.RequestExecuter.Interface;
using Voice_of_Time_Server.Shared;
using Voice_of_Time_Server.Transfer;
using VoTCore;
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
    /// Function for getting a public user if he exists
    /// </summary>
    internal class PublicUserGet : IServerRequestExecuter
    {
        bool IServerRequestExecuter.ExecuteOnlyIfVerified => true;

        (IVOTPHeader, IVOTPBody?)? IServerRequestExecuter.ExecuteRequest(HeaderReq header, IVOTPBody? body, SocketHandler socket)
        {
            if (body is not SData_Long longBody)
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.WRONG_BODY_TYPE, $"Wrong Body! Need to be a {nameof(SData_Long)}"));
            }

            if (!ServerData.server.UserExists(longBody.Data))
            {
                return (new HeaderAck(false), new SData_InternalException(InternalExceptionCode.USER_DOES_NOT_EXISTS, $"User with the ID: {longBody.Data} is unknown!"));
            }

            return (new HeaderAck(true), ServerData.server.GetUser(longBody.Data));
        }
    }
}
