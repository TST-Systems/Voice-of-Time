
using System.Text.Json.Serialization;
/**
* @author      - Timeplex
* 
* @created     - 17.02.2023
* 
* @last_change - 17.02.2023
*/
namespace VoTCore.Package.SData
{
    public class SData_Exception : SData<Exception>
    {
        [JsonConstructor]
        public SData_Exception(Exception? data) : base(data, BodyType.SDATA_EXCEPTION)
        {
        }

        public SData_Exception(string data) : base(new Exception(data), BodyType.SDATA_EXCEPTION)
        {
        }
    }
}
