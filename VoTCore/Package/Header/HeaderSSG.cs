using System.Text.Json.Serialization;
using VoTCore.Package.Interfaces;

/**
 * @author      - Timeplex
 * 
 * @created     - 12.02.2023
 * 
 * @last_change - 12.02.2023
 */
namespace VoTCore.Package.Header
{
    /// <summary>
    /// Header Send->Store->Get
    /// </summary>
    public class HeaderSSG : IVOTPHeader
    {
        [JsonIgnore]
        public short Version => 4;

        public long SenderID { get; }
        public long ReciverID { get; }
        
        public DateTime ExpireDate { get; }

        public DataHandling StoreInfo { get; }

        [JsonConstructor]
        public HeaderSSG(long senderID, long reciverID, DateTime expireDate, DataHandling storeInfo)
        {
            SenderID   = senderID;
            ReciverID  = reciverID;
            ExpireDate = expireDate;
            StoreInfo  = storeInfo;
        }
    }
}