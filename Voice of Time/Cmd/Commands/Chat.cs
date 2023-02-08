using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using VoTCore.Algorithms;
using VoTCore.Controll;
using VoTCore.Package;
using VoTCore.Package.Header;
using VoTCore.Package.SData;

/**
 * @author      - Timeplex
 * 
 * @created     - 27.01.2023
 * 
 * @last_change - 28.01.2023
 */
namespace Voice_of_Time.Cmd.Commands
{
    internal class Chat : IConsoleCommandAsync, ICommandHelp
    {
        public string Command => "chat";

        public string[] Aliases { get => Array.Empty<string>(); }

        public string Usage => "help -c chat";

        public string[] CommandHelp
        {
            get => new string[]{
            "Usage: chat <argument>\n\n" +
            "List of Arugements:\n\n" +
            "list     - Show a list of all available chats\n" +
            "", };
        }

        public async Task<bool> ExecuteCommand(string command, string[] args)
        {
            if (ClientData.CurrentConnection is null) 
            {
                Console.WriteLine("You are currently not connected to any server!");
                return true; 
            }
            if (args.Length == 0) return false;

            switch (args[0])
            {
                case "list":
                    return ListChats();
                case "new":
                    return await CreateChatAsync();
            }

            return false;
        }

        protected virtual bool ListChats()
        {
            if (ClientData.CurrentConnection is null)
            {
                Console.WriteLine("You are currently not connected to any server.");
                return true;
            }

            var currentClientInstace = ClientData.GetServerCient((Guid)ClientData.CurrentConnection);

            if (currentClientInstace is null)
            {
                Console.WriteLine("An unknwon Error ocured!");
                return false;
            }

            Console.WriteLine($"You have currently {currentClientInstace.TextChats.Count} open chats:");
            int i = 0;
            foreach (var chat in currentClientInstace.TextChats)
            {
                Console.WriteLine($"[{i++}]: " + chat);
            }

            return true;
        }

        protected virtual async Task<bool> CreateChatAsync()
        {
            if (ClientData.CurrentConnection is null) {
                Console.WriteLine("You are currently on no Server!");
                return true; 
            }

            Console.WriteLine("Please Enter User(s) to invide by (,) devided (ID: #ZZZZ or Nick: Peter):");
            var input = Console.ReadLine();

            if(input is null or "")
            {
                Console.WriteLine("No user was enterd!");
                return true;
            }

            var userList = input.Split(',');

            if (userList.Length > 0) { userList = userList.Append(input).ToArray(); }

            for (int i = 0; i < userList.Length; i++)
            {
                userList[i] = userList[i].Trim();
            }

            // Get all UserIDs
            List<long> particepents = new();

            foreach (var user in userList)
            {
                try
                {
                    long id = -1;
                    // When user is defind over id
                    if (user.StartsWith('#'))
                    {
                        id = Base36.Decode(user[1..]);
                    }
                    // Or over full userID
                    else
                    {
                        id = long.Parse(user);
                    }

                    particepents.Add(id);
                }
                catch (Exception) 
                {
                    Console.WriteLine($"User: {user} coudn't be added!");
                }
            }

            // Sort the List and remove multiple entraces
            particepents.Sort();
            particepents = particepents.Distinct().ToList();

            //
            var currentClient = ClientData.CurrentClient;
            if (currentClient is null) throw new Exception("Connection was closed during operation!");

            // Check if users exist when not try to find out if user exists
            foreach (var particepent in particepents)
            {
                if (currentClient.UserDB.ContainsKey(particepent)) continue;

                if (await TryGettingUserAsync(currentClient.UserID, particepent)) continue;

                Console.WriteLine($"User: #{Base36.Encode(particepent)} coudn't be found");

                particepents.Remove(particepent);
            }
            
            return true;
        }

        private static async Task<bool> TryGettingUserAsync(long senderID, long targetID)
        {
            if (ClientData.CurrentConnection is null) return false;

            var header  = new HeaderReq(senderID, RequestType.GET_PUBLIC_USER);
            var body    = new SData_Long(targetID);
            var package = new VOTP(header, body);

            var connection = ClientData.GetConnection((Guid)ClientData.CurrentConnection) ?? throw new Exception("Connection is not Registert!");

            var result = new VOTP(await connection.EnqueueItem(package.Serialize()));

            if (result.Header is not HeaderAck resHeader) throw new Exception("Server didn't responded correctly!");
            if (result.Body is null) return false;
            if (result.Body is not SData_Long resBody) throw new Exception("Server didn't responded correctly!");

            if (resHeader.Successful is false) return false;
            if (resBody.Data < 0) return false;


            return false;
        }
    }
}
