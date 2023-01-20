using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoTCore.Package.Interfaces;

namespace VoTCore.Package.Header
{
    public class HeaderAck : IVOTPHeader
    {
        public bool Successful { get; }

        [JsonIgnore]
        public short Version => 3;

        public HeaderAck(bool successful)
        {
            Successful = successful;
        }
    }
}
