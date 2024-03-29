﻿using Voice_of_Time_Server.RequestExecuter;
using Voice_of_Time_Server.RequestExecuter.Interface;
using Voice_of_Time_Server.User;
using VoTCore;

/**
* @author      - Timeplex
* 
* @created     - 20.01.2023
* 
* @last_change - 16.02.2023
*/
namespace Voice_of_Time_Server.Shared
{
    internal static class ServerData
    {
#pragma warning disable CS8618 // Server will need to be Initialize() befor working
        public static Server server;
#pragma warning restore CS8618 // 

        /// <summary>
        /// Server request executer register
        /// </summary>
        private readonly static Dictionary<RequestType, IServerRequestExecuter> SRER = new();

        /// <summary>
        /// <para>Only call once!</para>
        /// Initialization of all default data
        /// </summary>
        // TODO: lock Initialize after first initzialisation
        public static void Initialize()
        {
            server = new Server(); // TODO: Load and Save Server
            // Register default Executer
            RegisterExecuter(new ServerGetIdentitiy(),           RequestType.SERVER_GET_IDENTITY);
            RegisterExecuter(new ServerPublicKeyExchange(),      RequestType.SERVER_PUBLIC_KEY_EXCHANGE);
            RegisterExecuter(new UserRegister(),                 RequestType.USER_REGISTRATION);
            RegisterExecuter(new UserVerify(),                   RequestType.USER_VERIFY);
            RegisterExecuter(new UserSetUsername(),              RequestType.USER_SET_USERNAME);
            RegisterExecuter(new CommunicationGetKeyAndSecure(), RequestType.COMMUNICATION_GET_KEY_AND_SECURE);
            RegisterExecuter(new PublicUserGet(),                RequestType.PUBLIC_USER_GET);
            RegisterExecuter(new PublicUserGetIDList(),          RequestType.PUBLIC_USER_GET_ID_LIST);
            RegisterExecuter(new PrivatChatRegister(),           RequestType.PRIVAT_CHAT_REGISTER);
            RegisterExecuter(new PrivatChatInviteUser(),         RequestType.PRIVAT_CHAT_INVITE_USER);
            RegisterExecuter(new PrivatChatInviteAccept(),       RequestType.PRIVAT_CHAT_INVITE_ACCEPT);         
            RegisterExecuter(new StashAdd(),                     RequestType.STASH_ADD);
            RegisterExecuter(new StashGet(),                     RequestType.STASH_GET);
            RegisterExecuter(new StashList(),                    RequestType.STASH_LIST);
            RegisterExecuter(new StashDelete(),                  RequestType.STASH_DELETE);
        }

        /// <summary>
        /// Request executer regestration
        /// </summary>
        /// <param name="executer">Request executer</param>
        /// <param name="key">RequestType</param>
        /// <returns>Could be set</returns>
        public static bool RegisterExecuter(IServerRequestExecuter executer, RequestType key)
        {
            if (SRER.ContainsKey(key))
            {
                return false;
            }

            SRER.Add(key, executer);
            return true;
        }

        /// <summary>
        /// Get a executer over is RequestType
        /// </summary>
        /// <param name="key">RequestType of request</param>
        /// <returns>Executer or default = null if not exists</returns>
        public static IServerRequestExecuter? GetExecuter(RequestType key)
        {
            return SRER.GetValueOrDefault(key);
        }
    }
}
