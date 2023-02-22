/**
 * @author      - Timeplex
 * 
 * @created     - 20.01.2023
 * 
 * @last_change - 20.01.2023
 */
namespace VoTCore.Package.SData
{
    /// <summary>
    /// Body for a single UUID/GUID
    /// </summary>
    public class SData_Guid : SData<Guid>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="data">UUID/GUID</param>
        public SData_Guid(Guid data)
            : base(data, BodyType.SDATA_GUID)
        {
        }
    }
}
