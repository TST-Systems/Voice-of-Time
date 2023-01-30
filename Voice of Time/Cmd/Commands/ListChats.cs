using VoTCore.Controll;

namespace Voice_of_Time.Cmd.Commands
{
    internal class ListChats : IConsoleCommandSync
    {
        public string Command => "listchats";

        private readonly string[] aliases = { "lsct" };
        public string[] Aliases => aliases;

        public string Usage => "listchats";

        public bool ExecuteCommand(string command, string[] args)
        {
            if(ClientData.CurrentConnection is null)
            {
                Console.WriteLine("You are currently not connected to any server.");
                return true;
            }

            var currentClientInstace = ClientData.GetServerCient((Guid)ClientData.CurrentConnection);

            if(currentClientInstace is null)
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
    }
}
