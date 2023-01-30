using System.Text.Json.Serialization;

/**
 * @author      - Timeplex
 * 
 * @created     - 24.12.2022
 * 
 * @last_change - 05.01.2023
 */
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
