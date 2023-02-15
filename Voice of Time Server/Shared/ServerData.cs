using Voice_of_Time_Server.User;

/**
* @author      - Timeplex
* 
* @created     - 20.01.2023
* 
* @last_change - 20.01.2023
*/
namespace Voice_of_Time_Server.Shared
{
    internal static class ServerData
    {
        public static Server server;


        public static void Initialize()
        {
            server = new Server();
        }
    }
}
