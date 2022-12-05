using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoTCore.Package
{
    public interface IVOTPBody
    {
        /// <summary>
        /// Type(s) of Message
        /// </summary>
        MessageType Type { get; }
    }
    
    public enum MessageType
    {
        TEXT_MESSAGE    = 0b00000001,
        MEDIA_MESSAGE   = 0b00000010,
        CONTROL_MESSAGE = 0b00000100,
        KEY_MESSAGE     = 0b00001000,
    }
}
