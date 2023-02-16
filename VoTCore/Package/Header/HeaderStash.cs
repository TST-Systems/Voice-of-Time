using System.Text.Json.Serialization;
using VoTCore.Package.Interfaces;

/**
 * @author      - Timeplex
 * 
 * @created     - 12.02.2023
 * 
 * @last_change - 16.02.2023
 */
namespace VoTCore.Package.Header
{
    /// <summary>
    /// Header Send->Store->Get
    /// </summary>
    public class HeaderStash : IVOTPHeader
    {
        [JsonIgnore]
        public short Version => 4;

        public long SenderID { get; }
        public long ReciverID { get; }

        public StashMode Mode { get; }

        [JsonConstructor]
        public HeaderStash(long senderID, long reciverID, StashMode mode)
        {
            SenderID  = senderID;
            ReciverID = reciverID;
            Mode      = mode;
        }
    }
}