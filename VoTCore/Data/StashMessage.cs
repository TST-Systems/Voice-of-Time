/**
 * @author      - Timeplex
 * 
 * @created     - 16.02.2023
 * 
 * @last_change - 18.02.2023
 */
namespace VoTCore.Data
{
    public class StashMessage
    {
        public long         ReceiptID       { get; }
        public DataHandling MessageHandling { get; }
        public long         AuthorID        { get; }
        public long         TargetID        { get; }
        public DateTime     Created         { get; }
        public DateTime     Expires         { get; }
        public string Message               { get; }

        public StashMessage(long receiptID, DataHandling messageHandling, long authorID, long targetID, DateTime created, DateTime expires, string message)
        {
            ReceiptID       = receiptID;
            MessageHandling = messageHandling;
            AuthorID        = authorID;
            TargetID        = targetID;
            Created         = created;
            Expires         = expires;
            Message         = message;
        }
    }
}