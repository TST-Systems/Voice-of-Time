/**
 * @author      - Timeplex
 * 
 * @created     - 20.01.2023
 * 
 * @last_change - 20.01.2023
 */
namespace VoTCore.Package.SData
{
    public class SData_Guid : SData<Guid>
    {
        public SData_Guid(Guid data)
            : base(data, BodyType.SDATA_GUID)
        {
        }
    }
}
