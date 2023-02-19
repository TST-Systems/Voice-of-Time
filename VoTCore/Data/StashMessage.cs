/**
 * @author      - Timeplex
 * 
 * @created     - 16.02.2023
 * 
 * @last_change - 18.02.2023
 */
namespace VoTCore.Data
{
    public readonly struct StashMessage
    {
        public readonly long         ReceiptID;
        public readonly DataHandling MessageHandling;
        public readonly long         AuthorID;
        public readonly long         TargetID;
        public readonly DateTime     Created;
        public readonly DateTime     Expires;
        public readonly string       Message;

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