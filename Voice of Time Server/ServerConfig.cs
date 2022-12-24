using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Voice_of_Time_Server
{
    internal class ServerConfig
    {
        [JsonInclude]
        public string DisplayName { get; set; }
        [JsonInclude]
        public string Description { get; set; }
        [JsonInclude]
        public string Password { get; set; }

        [JsonInclude]
        public string Version { get; }

        public ServerConfig(string displayName = "VoT Server", string description = "VoT Server", string password = "")
        {
            DisplayName = displayName;
            Description = description;
            Password    = password;
            Version     = "alpha_0.1";
        }
    }
}
