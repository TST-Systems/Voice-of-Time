/**
 * @author      - Timeplex
 * 
 * @created     - 16.02.2023
 * 
 * @last_change - 18.02.2023
 */
namespace VoTCore.Data
{
    /// <summary>
    /// Wrapper for a message and all its meta data with can be stored and getted in and from the stash
    /// </summary>
    public class StashMessage
    {
        /// <summary>
        /// ID of message inside a stash
        /// </summary>
        public long         ReceiptID       { get; }
        /// <summary>
        /// Data handling instruction
        /// </summary>
        public DataHandling MessageHandling { get; }
        /// <summary>
        /// ID of the creator of the message
        /// </summary>
        public long         AuthorID        { get; }
        /// <summary>
        /// ID of the targeted stash
        /// </summary>
        public long         TargetID        { get; }
        /// <summary>
        /// Date and Time of creation
        /// </summary>
        public DateTime     Created         { get; }
        /// <summary>
        /// Date and Time of expiring
        /// </summary>
        public DateTime     Expires         { get; }
        /// <summary>
        /// The message
        /// </summary>
        public string       Message         { get; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="receiptID"></param>
        /// <param name="messageHandling"></param>
        /// <param name="authorID"></param>
        /// <param name="targetID"></param>
        /// <param name="created"></param>
        /// <param name="expires"></param>
        /// <param name="message"></param>
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