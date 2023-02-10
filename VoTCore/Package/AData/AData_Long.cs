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
    public class AData_Long : AData<long>
    {
        [JsonIgnore]
        public override BodyType Type => BodyType.ADATA_LONG;

        [JsonConstructor]
        public AData_Long(long[] data) : base(data) { }

        public AData_Long(List<long> data) : base(data) { }
    }
}
