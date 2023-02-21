using System.Text.Json.Serialization;
using VoTCore.Package.Interfaces;

namespace VoTCore.Package.AbsData
{
    public class AbsData_InviteAccept : IVOTPBody
    {
        [JsonIgnore]
        public BodyType Type { get => BodyType.ABSDATA_INVITE_ACCEPT; }

        public long Data { get; } // TODO: Rename after debvugsession!

        [JsonConstructor]
        public AbsData_InviteAccept(long data)
        {
            Data = data;
        }
    }
}
