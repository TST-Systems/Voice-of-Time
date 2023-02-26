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
    /// Interface for package Headers
    /// </summary>
    public interface IVOTPHeader
    {
        /// <summary>
        /// ID of header type, registert in constans
        /// </summary>
        short Version { get; }
    }
}