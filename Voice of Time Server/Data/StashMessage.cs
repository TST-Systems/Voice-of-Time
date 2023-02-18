using VoTCore;

/**
 * @author      - Timeplex
 * 
 * @created     - 16.02.2023
 * 
 * @last_change - 16.02.2023
 */
namespace Voice_of_Time_Server.Data
{
    internal readonly struct StashMessage
    {
        public readonly long Receipt;
        public readonly DataHandling MessageHandling;
        public readonly long AuthorID;
        public readonly long TargetID;
        public readonly DateTime Created;
        public readonly DateTime Expires;
        public readonly string Message;

        public StashMessage(long receipt, DataHandling messageHandling, long authorID, long targetID, DateTime created, DateTime expires, string message)
        {
            Receipt = receipt;
            MessageHandling = messageHandling;
            AuthorID = authorID;
            TargetID = targetID;
            Created = created;
            Expires = expires;
            Message = message;
        }
    }
}