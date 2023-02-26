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
     /// Body for a single string
     /// </summary>
    public class SData_String : SData<string?>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="data">String</param>
        public SData_String(string? data) 
            : base(data, BodyType.SDATA_STRING)
        {
        }
    }
}