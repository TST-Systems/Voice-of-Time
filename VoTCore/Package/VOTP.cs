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
        private readonly IVOTPHeader header;
        private readonly IVOTPBody?  data;

        public VOTP(IVOTPHeader header, IVOTPBody? data)
        {
            this.header = header;
            this.data = data;
        }

        public IVOTPHeader Header
        {
            get { return header; }
        }

        public IVOTPBody? Data
        {
            get { return data; }
        }
    }
}
