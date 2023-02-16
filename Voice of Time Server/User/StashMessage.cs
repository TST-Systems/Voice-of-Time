using VoTCore;

/**
 * @author      - Timeplex
 * 
 * @created     - 16.02.2023
 * 
 * @last_change - 16.02.2023
 */
namespace Voice_of_Time_Server
{
    internal readonly struct StashMessage
    {
        public readonly Int64        Receipt;
        public readonly DataHandling MessageHandling;
        public readonly Int64        AuthorID;
        public readonly Int64        TargetID;
        public readonly DateTime     Created;
        public readonly DateTime     Expires;
        public readonly string       Message;

        public StashMessage(long receipt, DataHandling messageHandling, long authorID, long targetID, DateTime created, DateTime expires, string message)
        {
            Receipt         = receipt;
            MessageHandling = messageHandling;
            AuthorID        = authorID;
            TargetID        = targetID;
            Created         = created;
            Expires         = expires;
            Message         = message;
        }
    }
}