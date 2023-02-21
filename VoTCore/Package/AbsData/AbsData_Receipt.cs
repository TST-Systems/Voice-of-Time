using System.Text.Json.Serialization;
using VoTCore.Package.AData;

namespace VoTCore.Package.AbsData
{
    public class AbsData_Receipt : AData_Long
    {
        [JsonIgnore]
        public long TargetID  { get => Data[0]; set => Data[0] = value; }
        [JsonIgnore]
        public long ReceiptID { get => Data[1]; set => Data[1] = value; }
        [JsonIgnore]
        public override BodyType Type => BodyType.ABSDATA_RECEIPT;

        public AbsData_Receipt(long targetID, long receiptID) : base(new long[2])
        {
            TargetID  = targetID;
            ReceiptID = receiptID;
        }

        [JsonConstructor]
        public AbsData_Receipt(long[] data) : base(data)
        {
        }
    }
}
