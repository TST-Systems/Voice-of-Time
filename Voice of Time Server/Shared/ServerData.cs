using Voice_of_Time_Server.RequestExecuter.Interface;
using Voice_of_Time_Server.User;
using VoTCore;

/**
* @author      - Timeplex
* 
* @created     - 20.01.2023
* 
* @last_change - 16.02.2023
*/
namespace Voice_of_Time_Server.Shared
{
    internal static class ServerData
    {
        public static Server server;

        private readonly static Dictionary<RequestType, IServerRequestExecuter> SRER = new(); 

        public static void Initialize()
        {
            server = new Server();
        }
    }
}
