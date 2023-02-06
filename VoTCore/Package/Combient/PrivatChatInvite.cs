using System.Security.Cryptography;
using System.Text.Json.Serialization;
using VoTCore.Communication;
using VoTCore.Package.Interfaces;
using VoTCore.Package.SecData;
using VoTCore.User;
/**
 * @author      - Timeplex
 * 
 * @created     - 06.02.2023
 * 
 * @last_change - 06.02.2023
 */
namespace VoTCore.Package.Combient
{
    public class PrivatChatInvite : IVOTPBody
    {
        protected PrivatChat PrivatChat { get; set; }

        protected SecData_Key_Aes Aes   { get; }
        [JsonIgnore]
        public BodyType Type => BodyType.PRIVAT_CHAT_INVITE;

        // Only for JSON
        [JsonConstructor]
        protected PrivatChatInvite(PrivatChat privatChat, SecData_Key_Aes aes) 
        {
            PrivatChat = privatChat;
            Aes = aes;
        }

        public PrivatChatInvite(PrivatChat privatChat, PublicClient target, long sourceID)
        {
            PrivatChat = privatChat;
            Aes = new(privatChat.GroupKey, sourceID);
            Aes.EncryptData(target);
        }

        public PrivatChat GetChat (RSA userKey, long userID)
        {
            // Still encrypted?
            if (Aes.CryptedReciver >= 0)
                if (Aes.CryptedReciver != userID)
                    throw new Exception("Wrong Reciver!");
                else
                    Aes.DecryptData(userKey);
            // Chat is already valid?
            if (!PrivatChat.GroupKeyIsSet)    
                PrivatChat = new(Aes.GetKey(), PrivatChat.Participants, PrivatChat.ChatID, PrivatChat.Title);
           
            return PrivatChat;
        }
    }
}
