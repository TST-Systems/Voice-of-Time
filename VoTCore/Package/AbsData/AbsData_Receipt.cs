using System.Text.Json.Serialization;
using VoTCore.Package.AData;

namespace VoTCore.Package.AbsData
{
    /// <summary>
    /// Abstract Data for a receipt for a stored message
    /// </summary>
    public class AbsData_Receipt : AData_Long
    {
        /// <summary>
        /// ID of stash
        /// </summary>
        [JsonIgnore]
        public long TargetID  { get => Data[0]; set => Data[0] = value; }
        /// <summary>
        /// ID of message inside the stash
        /// </summary>
        [JsonIgnore]
        public long ReceiptID { get => Data[1]; set => Data[1] = value; }
        [JsonIgnore]
        public override BodyType Type => BodyType.ABSDATA_RECEIPT;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="targetID">ID of stash</param>
        /// <param name="receiptID">ID of message</param>
        public AbsData_Receipt(long targetID, long receiptID) : base(new long[2])
        {
            TargetID  = targetID;
            ReceiptID = receiptID;
        }

        /// <summary>
        /// JSON constructor
        /// </summary>
        /// <param name="data"></param>
        [JsonConstructor]
        public AbsData_Receipt(long[] data) : base(data)
        {
        }
    }
}
