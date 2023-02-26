using System.Text.Json.Serialization;
using VoTCore.Package.AData;

/**
 * @author      - Timeplex
 * 
 * @created     - 13.02.2023
 * 
 * @last_change - 13.02.2023
 */
namespace VoTCore.Package.AbsData
{
    /// <summary>
    /// Abstract data for informing the server about an inivte of a user to a chat
    /// </summary>
    public class AbsData_Invite : AData_Long
    {
        /// <summary>
        /// ID of inviter
        /// </summary>
        [JsonIgnore]
        public long SourceID { get => Data[0]; set => Data[0] = value; }
        /// <summary>
        /// ID of the invited
        /// </summary>
        [JsonIgnore]
        public long TargetID { get => Data[1]; set => Data[1] = value; }
        /// <summary>
        /// ID of the chat where the target was invited to
        /// </summary>
        [JsonIgnore]
        public long ChatID   { get => Data[2]; set => Data[2] = value; }
        [JsonIgnore]
        public override BodyType Type => BodyType.ABSDATA_INVITE;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="sourceID">ID of user</param>
        /// <param name="targetID">ID of user</param>
        /// <param name="chatID">ID of chat</param>
        public AbsData_Invite(long sourceID, long targetID, long chatID) : base(new long[3])
        {
            SourceID = sourceID;
            TargetID = targetID;
            ChatID   = chatID;
        }

        /// <summary>
        /// JSON constructor
        /// </summary>
        /// <param name="data"></param>
        [JsonConstructor]
        public AbsData_Invite(long[] data) : base(data)
        {
        }        
    }
}
