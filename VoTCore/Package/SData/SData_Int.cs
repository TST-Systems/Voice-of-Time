/**
 * @author      - Timeplex
 * 
 * @created     - 14.01.2023
 * 
 * @last_change - 20.01.2023
 */
namespace VoTCore.Package.SData
{
    /// <summary>
    /// Body for single int value
    /// </summary>
    public class SData_Int : SData<Int32>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="data">Int32</param>
        public SData_Int(int data) 
            : base(data, BodyType.SDATA_INT)
        {
        }
    }
}
