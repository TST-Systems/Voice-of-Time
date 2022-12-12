using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoTCore.Communication;

namespace VoTCore.Package
{
    public class VOTPInfo
    {
        public VOTPInfo(VOTP package)
        {
            this.Version = package.Header.Version;
            if(package.Data != null)
                this.Type = package.Data.Type;
        }

        [JsonConstructor]
        public VOTPInfo(short Version, MessageType? Type)
        {
            this.Version = Version;
            this.Type    = Type;
        }

        public short Version { get; }

        public MessageType? Type { get; }
    }
}
