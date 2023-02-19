using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoTCore.Package.SData;

namespace VoTCore.Package.AbsData
{
    public class AbsData_InviteAccept : SData_Long
    {
        [JsonIgnore]
        public new BodyType Type { get => BodyType.ABSDATA_INVITE_ACCEPT; }

        [JsonConstructor]
        public AbsData_InviteAccept(long data) : base(data)
        {
        }
    }
}
