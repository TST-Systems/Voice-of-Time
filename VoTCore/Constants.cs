using VoTCore.Communication;
using VoTCore.Package;

namespace VoTCore
{
    public static class Constants
    {
        public readonly static Dictionary<Int16, Type> HeaderTypes = new()
        {
            { 1, typeof(VOTPHeaderV1) },
        };

        public readonly static Dictionary<MessageType, Type> BodyTypes   = new()
        {
            { MessageType.TEXT_MESSAGE,                             typeof(TextMessage) },
            { MessageType.TEXT_MESSAGE | MessageType.MEDIA_MESSAGE, typeof(MediaMessage) },
        };

    }

    public enum MessageType
    {
        TEXT_MESSAGE    = 0b00000001,
        MEDIA_MESSAGE   = 0b00000010,
        CONTROL_MESSAGE = 0b00000100,
        KEY_MESSAGE     = 0b00001000,
    }
}
