/**
* @author      - Timeplex
* 
* @created     - 24.12.2022
* 
* @last_change - 05.01.2023
*/
namespace Voice_of_Time_Server.User
{
    internal class UserInfo
    {
        public string UserName { get; set; }

        public UserInfo(string userName)
        {
            UserName = userName;
        }
    }
}