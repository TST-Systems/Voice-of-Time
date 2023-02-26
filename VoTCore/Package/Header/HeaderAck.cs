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
    /// <summary>
    /// Header for acknowledge the processing of a package
    /// </summary>
    public class HeaderAck : IVOTPHeader
    {
        /// <summary>
        /// Displays the success
        /// </summary>
        public bool Successful { get; }

        [JsonIgnore]
        public short Version => 3;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="successful"></param>
        public HeaderAck(bool successful)
        {
            Successful = successful;
        }
    }
}
