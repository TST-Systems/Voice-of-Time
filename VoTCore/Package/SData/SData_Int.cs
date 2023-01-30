/**
 * @author      - Timeplex
 * 
 * @created     - 14.01.2023
 * 
 * @last_change - 20.01.2023
 */
namespace VoTCore.Package.SData
{
    public class SData_Int : SData<Int32>
    {
        public SData_Int(int data) 
            : base(data, BodyType.SDATA_INT)
        {
        }
    }
}
