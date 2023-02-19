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
    public class AbsData_Invite : AData_Long
    {
        [JsonIgnore]
        public long SourceID { get => Data[0]; set => Data[0] = value; }
        [JsonIgnore]
        public long TargetID { get => Data[1]; set => Data[1] = value; }
        [JsonIgnore]
        public long ChatID   { get => Data[2]; set => Data[2] = value; }
        [JsonIgnore]
        public override BodyType Type => BodyType.ABSDATA_INVITE;

        public AbsData_Invite(long sourceID, long targetID, long chatID) : base(new long[3])
        {
            SourceID = sourceID;
            TargetID = targetID;
            ChatID   = chatID;
        }

        [JsonConstructor]
        public AbsData_Invite(long[] data) : base(data)
        {
        }        
    }
}
