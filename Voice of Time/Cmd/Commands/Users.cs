using System.Diagnostics;
using Voice_of_Time.Shared;
using Voice_of_Time.Shared.Functions;
using VoTCore.Algorithms;
using VoTCore.Controll;

/**
 * @author      - Timeplex
 * 
 * @created     - 08.02.2023
 * 
 * @last_change - 11.02.2023
 */
namespace Voice_of_Time.Cmd.Commands
{
    internal class Users : IConsoleCommandAsync
    {
        public string Command => "users";

        private readonly string[] aliases = { "u", "user", "client", "clients" };
        public string[] Aliases => aliases;

        public string Usage => "users list";

        public async Task<bool> ExecuteCommand(string command, string[] args)
        {
            if(args.Length <= 0) return false;

            if (args[0].ToLower() == "list")
            {
                return await RequestAllUser();
            }

            return false;
        }

        private async Task<bool> RequestAllUser()
        {
            var currentClient = ClientData.CurrentClient;

            if (currentClient == null) 
            {
                Console.WriteLine("You need to connect to a Server first!"); 
                return false; 
            }

            var (socket, client, _) = ClientData.GetCurrentConnection();

            List<long> userID = await Requests.RequestAllUserIDs();

            var loadingBar = new LoadingBar(userID.Count);
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            loadingBar.Start();

            foreach (var user in userID)
            {
                var suc = await Requests.TryGettingUserAsync(socket, client, user);
                if(suc)
                {
                    loadingBar.Update(loadingBar.CurrentStep + 1);
                }
                else
                {
                    loadingBar.Pause();
                    Console.WriteLine($"Coudn't get User: #{Base36.Encode(user)}");
                    loadingBar.Start();
                }
            }

            loadingBar.Stop();
            stopwatch.Stop();

            Console.WriteLine($"Time passed: {stopwatch.Elapsed.TotalMilliseconds}ms\n");

            var userList = new List<long>(currentClient.UserDB.Keys);
            userList.Sort();

            var pattern = "{0,16}|{1,-32}|{2}";

            Console.WriteLine(pattern, "ID", "Username", "Has Public Key");
            Console.WriteLine(pattern, "----------------", "--------------------------------", "----------------");

            foreach (var pubClientID in userList)
            {
                var pubClient = currentClient.UserDB[pubClientID];
                Console.WriteLine(pattern, Base36.Encode(pubClient.ID), pubClient.Username, pubClient.PublicKey is not null);
            }
            return true;
        }
    }
}
