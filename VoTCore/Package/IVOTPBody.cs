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
}
