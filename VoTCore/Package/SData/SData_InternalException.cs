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
    public class SData_InternalException : SData<InternalException>
    {
        [JsonConstructor]
        public SData_InternalException(InternalException data) 
            : base(data, BodyType.SDATA_INTERNALEXCEPTION)
        {
        }

        public SData_InternalException(InternalExceptionCode code, string message)
            : base(new(message, code), BodyType.SDATA_INTERNALEXCEPTION) { }

    }
}
