using System.Text.Json.Serialization;
/**
* @author      - Timeplex
* 
* @created     - 13.01.2023
* 
* @last_change - 13.01.2023
*/
namespace VoTCore.Package.Interfaces
{
    /// <summary>
    /// Interface for package bodys
    /// </summary>
    public interface IVOTPBody
    {
        /// <summary>
        /// Type(s) of Message
        /// </summary>
        [JsonIgnore]
        BodyType Type { get; }
    }
}