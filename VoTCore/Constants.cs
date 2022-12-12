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
            { MessageType.TEXT_MESSAGE | MessageType.MEDIA_MESSAGE, typeof(FileMessage) },
        };

    }

    public enum MessageType
    {
        TEXT_MESSAGE    = 1 << 0,
        MEDIA_MESSAGE   = 1 << 1,
        CONTROL_MESSAGE = 1 << 2,
        KEY_MESSAGE     = 1 << 3,
    }
}
