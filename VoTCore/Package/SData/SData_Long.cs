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
    /// Body for a single long
    /// </summary>
    public class SData_Long : SData<Int64>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="data">Int64</param>
        public SData_Long(long data) 
            : base(data, BodyType.SDATA_LONG)
        {
        }
    }
}
