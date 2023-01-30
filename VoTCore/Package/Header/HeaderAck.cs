using System.Text.Json.Serialization;
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
