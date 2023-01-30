/**
 * @author      - Timeplex
 * 
 * @created     - 20.01.2023
 * 
 * @last_change - 20.01.2023
 */
namespace VoTCore.Package.SData
{
    public class SData_String : SData<string?>
    {
        public SData_String(string? data) 
            : base(data, BodyType.SDATA_STRING)
        {
        }
    }
}