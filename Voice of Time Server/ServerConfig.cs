using System.Text.Json.Serialization;

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
        public int Port { get; set; }

        [JsonIgnore]
        public string Version { get; }

        public ServerConfig(string displayName = "VoT Server", string description = "VoT Server", string password = "", int port = 15050)
        {
            DisplayName = displayName;
            Description = description;
            Password    = password;
            Version     = "alpha_0.1";
            Port        = port;
        }
    }
}
