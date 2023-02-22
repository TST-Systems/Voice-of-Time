using System.Text.Json.Serialization;
using VoTCore.Data;

/**
* @author      - Timeplex
* 
* @created     - 17.02.2023
* 
* @last_change - 17.02.2023
*/
namespace VoTCore.Package.SData
{
    /// <summary>
    /// Body for sending Internal Errors
    /// </summary>
    public class SData_InternalException : SData<InternalException>
    {
        /// <summary>
        /// JSON construcor, but can alos be used
        /// </summary>
        /// <param name="data">Direct internal exception</param>
        [JsonConstructor]
        public SData_InternalException(InternalException data) 
            : base(data, BodyType.SDATA_INTERNALEXCEPTION)
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error message</param>
        public SData_InternalException(InternalExceptionCode code, string message)
            : base(new(message, code), BodyType.SDATA_INTERNALEXCEPTION) { }

    }
}
