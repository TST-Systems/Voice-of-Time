using System.Text.Json.Serialization;
using VoTCore.Data;
/**
 * @author      - Timeplex
 * 
 * @created     - 18.02.2023
 * 
 * @last_change - 18.02.2023
 */
namespace VoTCore.Package.StashData
{
    /**
     * long         Receipt          - NO
     * DataHandling MessageHandling  - YES
     * long         AuthorID         - NO
     * long         TargetID         - YES
     * DateTime     Created          - NO
     * DateTime     Expires          - YES
     * string       Message          - YES
     */
    public class StashData_Add : StashData
    {
        [JsonIgnore]
        public override BodyType Type => BodyType.STASHDATA_ADD;

        [JsonConstructor]
        public StashData_Add(StashMessage message) : 
            base(new(-1, message.MessageHandling, -1, message.TargetID, DateTime.MinValue, message.Expires, message.Message))
        {
        }

        public StashData_Add(string message, long targetID, DateTime expires, DataHandling messageHandling = DataHandling.REMOVE_AFTER_GET_ACK) : 
            base(new(-1, messageHandling, -1, targetID, DateTime.MinValue ,expires, message)) { }
    }
}
