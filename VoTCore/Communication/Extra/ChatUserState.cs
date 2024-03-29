﻿/**
 * @author      - Timeplex
 * 
 * @created     - 12.01.2023
 * 
 * @last_change - 12.02.2023
 */
namespace VoTCore.Communication.Extra
{
    /// <summary>
    /// Flags the state of the user in a chat
    /// </summary>
    [Flags]
    public enum ChatUserState : byte
    {
        ADMIN     = 0b_0100_0000,
        MODERATOR = 0b_0010_0000,
        MEMBER    = 0b_0001_0000,
        INVITED   = 0b_0000_1000,
        BLOCKED   = 0b_0000_0100,
        NONE      = 0b_0000_0000
    }
}