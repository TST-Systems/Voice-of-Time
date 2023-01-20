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
            else this.Type = BodyType.NONE;
        }

        [JsonConstructor]
        public VOTPInfo(short Version, BodyType Type)
        {
            this.Version = Version;
            this.Type    = Type;
        }

        public short Version { get; }

        public BodyType Type { get; }
    }
}
