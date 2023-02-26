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
    /// <summary>
    /// Body for adding data to a stash
    /// </summary>
    public class StashData_Add : StashData
    {
        [JsonIgnore]
        public override BodyType Type => BodyType.STASHDATA_ADD;

        /// <summary>
        /// JSON constructor
        /// </summary>
        /// <param name="data"></param>
        [JsonConstructor]
        public StashData_Add(StashMessage data) : 
            base(new(-1, data.MessageHandling, -1, data.TargetID, DateTime.MinValue, data.Expires, data.Message))
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="message">Message to store</param>
        /// <param name="targetID">ID of stash to stash the message into</param>
        /// <param name="expires">Date of expiring</param>
        /// <param name="messageHandling">Data handling agreement</param>
        public StashData_Add(string message, long targetID, DateTime expires, DataHandling messageHandling = DataHandling.REMOVE_AFTER_GET_ACK) : 
            base(new(-1, messageHandling, -1, targetID, DateTime.MinValue ,expires, message)) { }
    }
}
