using System.Buffers.Text;
using System.Text;
using Voice_of_Time.Shared;
using Voice_of_Time.Shared.Functions;
using Voice_of_Time.Transfer;
using VoTCore.Algorithms;
using VoTCore.Communication;
using VoTCore.Communication.Data;
using VoTCore.Controll;
using VoTCore.Package;
using VoTCore.Package.Header;
using VoTCore.Secure;

/**
 * @author      - Timeplex
 * 
 * @created     - 27.01.2023
 * 
 * @last_change - 13.02.2023
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
            "list                 - Show a list of all available chats\n" +
            "new                  - Create a new Chat with 1-X members\n" +
            "read [id]            - Display all Messanges of a Chat\n" +
            "write [id] {Message} - write a message to a chat", };
        }

        public async Task<bool> ExecuteCommand(string command, string[] args)
        {
            if (ClientData.CurrentConnection is null) 
            {
                Console.WriteLine("You are currently not connected to any server!");
                return true; 
            }
            if (args.Length == 0) return false;

            return args[0] switch
            {
                "list"  => ListChats(),
                "new"   => await CreateChatAsync(),
                "read"  => ReadMessanges(args),
                "write" => await WriteMessage(args),
                _ => false,
            };
        }

        private async Task<bool> WriteMessage(string[] args)
        {
            if (ClientData.CurrentConnection is null || ClientData.CurrentClient is null)
            {
                Console.WriteLine("You are currently not connected to any server.");
                return true;
            }
            ClientSocket currentConnection = ClientData.GetConnection((Guid)ClientData.CurrentConnection) ?? throw new Exception();
            if (args.Length <= 2) return false;
            int chatListID;
            try
            {
                chatListID = int.Parse(args[1]);
                if (chatListID >= ClientData.CurrentClient.TextChats.Count)
                {
                    Console.WriteLine($"Selected ID {chatListID} is too big!");
                    Console.WriteLine($"Use \"chat list\" to see the avaivible IDs!");
                    return true;
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"{args[1]} is not a number!");
                Console.WriteLine($"Use \"chat list\" to see the avaivible IDs!");
                return false;
            }

            if (ClientData.CurrentClient.TextChats[chatListID] is not PrivatChat chat) 
            {
                Console.WriteLine("You can not use this chat!");
                return true; 
            }
            var toSend = "";

            for(int i = 2; i < args.Length; i++)
            {
                if(i != 2)
                {
                    toSend += " ";
                }
                toSend += args[i];
            }

            var header  = new HeaderStd(ClientData.CurrentClient.UserID, chat.ChatID, 1);
            var message = new TextMessage(toSend, ClientData.CurrentClient.UserID, DateTime.Now);
            var toStash = new VOTP(header, message).Serialize();
            toStash     = Convert.ToBase64String(CryproManager.AesEncyrpt(chat.Key, Encoding.UTF8.GetBytes(toStash)));

            var Receipt = await Requests.AddStashMessage(currentConnection, ClientData.CurrentClient, chat.ChatID, toStash);


            ClientData.CurrentClient.ReceiptStatusDictionary[Receipt] = ReceiptStatus.IGNORE;

            chat.AddMessage(message);

            return true;
        }

        private bool ReadMessanges(string[] args)
        {
            if (ClientData.CurrentConnection is null || ClientData.CurrentClient is null)
            {
                Console.WriteLine("You are currently not connected to any server.");
                return true;
            }
            if (args.Length <= 1)
            {
                Console.WriteLine($"Use \"chat list\" to see the avaivible IDs!");
                return false;
            }
            int chatListID;
            try
            {
                chatListID = int.Parse(args[1]);
                if (chatListID >= ClientData.CurrentClient.TextChats.Count)
                {
                    Console.WriteLine($"Selected ID {chatListID} is too big!");
                    Console.WriteLine($"Use \"chat list\" to see the avaivible IDs!");
                    return true;
                }
            }
            catch(Exception)
            {
                Console.WriteLine($"{args[1]} is not a number!");
                Console.WriteLine($"Use \"chat list\" to see the avaivible IDs!");
                return false;
            }
            if (ClientData.CurrentClient.TextChats[chatListID] is not PrivatChat chat)
            {
                Console.WriteLine("Chat is not a Chat?");
                Console.WriteLine($"Use \"chat list\" to see the avaivible IDs!");
                return true;
            }
            Console.WriteLine($"Messages of {chat.Title}");
            foreach (var message in chat.GetMessages())
            {
                Console.WriteLine();
                Console.Write(message.DateOfCreation.ToString() + " ");
                if (message.AuthorID == ClientData.CurrentClient.UserID)
                    Console.Write("You");
                else if (ClientData.CurrentClient.UserDB.ContainsKey(message.AuthorID))
                    Console.Write(ClientData.CurrentClient.UserDB[message.AuthorID].Username);
                else
                    Console.Write($"#{Base36.Encode(message.AuthorID)}");
                Console.WriteLine(" :");
                Console.WriteLine(message.MessageString);
            }
            return true;
        }

        protected virtual bool ListChats()
        {
            if (ClientData.CurrentConnection is null || ClientData.CurrentClient is null)
            {
                Console.WriteLine("You are currently not connected to any server.");
                return true;
            }

            var currentClientInstace = ClientData.CurrentClient;

            if (currentClientInstace is null)
            {
                Console.WriteLine("An unknwon Error ocured!");
                return false;
            }

            Console.WriteLine($"You have currently {currentClientInstace.TextChats.Count} open chats:");
            int i = 0;
            foreach (var chat in currentClientInstace.TextChats)
            {
                Console.Write($"[{i++}]: ");
                if(chat is PrivatChat pC)
                {
                    Console.WriteLine(pC.Title);
                }
                else
                {
                    Console.WriteLine("?");
                }
            }

            return true;
        }

        protected virtual async Task<bool> CreateChatAsync()
        {
            if (ClientData.CurrentConnection is null) {
                Console.WriteLine("You are currently on no Server!");
                return true; 
            }

            var (socket, client, _) = ClientData.GetCurrentConnection();


            Console.WriteLine("Please Enter User(s) to invide by (,) devided (ID: #ZZZZ or Nick: Peter):");
            var input = Console.ReadLine();

            if(input is null or "")
            {
                Console.WriteLine("No user was enterd!");
                return true;
            }

            var userList = input.Split(',');

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

            // Check if users exist when not try to find out if user exists
            foreach (var particepent in particepents)
            {
                if (client.UserDB.ContainsKey(particepent)) continue;

                if (await Requests.TryGettingUserAsync(socket, client, particepent)) continue;

                Console.WriteLine($"User: #{Base36.Encode(particepent)} coudn't be found");

                particepents.Remove(particepent);
            }

            // Get ChatID from Server and Register it 
            var chatID = await Requests.GetChatID(socket, client.UserID);

            // Create Chat
            var Chatname = "";

            if (particepents.Count <= 3) {
                for (int j = 0; j < particepents.Count; j++)
                {
                    Chatname += client.UserDB[particepents[j]].Username;
                    if (j + 1 < particepents.Count)
                    {
                        Chatname += ", ";
                    }
                } 
            }else
            {
                Console.WriteLine("Please Enter a name for the group: ");
                Chatname = Console.ReadLine();
                if (Chatname is null or "") Chatname = "Group";
                Chatname = Chatname.Trim();
            }

            var chat = new PrivatChat(chatID, Chatname, particepents);
            
            client.TextChats.Add(chat);

            // Invite all remaining users
            foreach (var particepen in particepents)
            {
                var pubClient = client.UserDB[particepen];

                if (pubClient is null) throw new Exception("Public user was deletet while processing!");

                await Requests.InviteUserToGroupAsync(socket, client, pubClient, new(chat), DataHandling.REMOVE_AFTER_GET_ACK);
            }

            return true;
        }
    }
}
