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
    public interface IVOTPBody
    {
        /// <summary>
        /// Type(s) of Message
        /// </summary>
        [JsonIgnore]
        BodyType Type { get; }
    }
}