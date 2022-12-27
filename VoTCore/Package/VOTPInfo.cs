using System.Text.Json.Serialization;

namespace VoTCore.Package
{
    public class VOTPInfo
    {
        public VOTPInfo(VOTP package)
        {
            this.Version = package.Header.Version;
            if (package.Data != null)
                this.Type = package.Data.Type;
            else this.Type = MessageType.NONE;
        }

        [JsonConstructor]
        public VOTPInfo(short Version, MessageType Type)
        {
            this.Version = Version;
            this.Type    = Type;
        }

        public short Version { get; }

        public MessageType Type { get; }
    }
}
