using System.Text.Json.Serialization;
using VoTCore.Package.Interfaces;

/**
 * @author      - Timeplex
 * 
 * @created     - 20.01.2023
 * 
 * @last_change - 20.01.2023
 */
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
