/**
 * @author      - Timeplex
 * 
 * @created     - 20.01.2023
 * 
 * @last_change - 20.01.2023
 */
namespace VoTCore.Package.SData
{
    public class SData_Long : SData<Int64>
    {
        public SData_Long(long data) 
            : base(data, BodyType.SDATA_LONG)
        {
        }
    }
}
