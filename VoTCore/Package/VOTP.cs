using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoTCore.Communication;

namespace VoTCore.Package
{
    public class VOTP
    {
        public VOTP(IVOTPHeader header, IVOTPBody? data)
        {
            this.Header = header;
            this.Data = data;
        }

        public IVOTPHeader Header { get; }

        public IVOTPBody? Data { get; }
    }
}
