using System.Text.Json.Serialization;

/**
 * @author      - Timeplex
 * 
 * @created     - 10.02.2023
 * 
 * @last_change - 10.02.2023
 */
namespace VoTCore.Package.AData
{
    /// <summary>
    /// Array body for longs
    /// </summary>
    public class AData_Long : AData<long>
    {
        [JsonIgnore]
        public override BodyType Type => BodyType.ADATA_LONG;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="data"></param>
        [JsonConstructor]
        public AData_Long(long[] data) : base(data) { }

        /// <summary>
        /// Import a List of longs
        /// </summary>
        /// <param name="data"></param>
        public AData_Long(List<long> data) : base(data) { }
    }
}
