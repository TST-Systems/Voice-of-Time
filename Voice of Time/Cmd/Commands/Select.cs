using VoTCore.Controll;

/**
 * @author      - Timeplex
 * 
 * @created     - 28.01.2023
 * 
 * @last_change - 28.01.2023
 */
namespace Voice_of_Time.Cmd.Commands
{
    internal class Select : IConsoleCommandSync
    {
        public string Command => "select";

        private readonly string[] aliases = { "s", "sel" };
        public string[] Aliases => aliases;

        public string Usage => "select [id]";

        public bool ExecuteCommand(string command, string[] args)
        {
            int id = -1;
            if (args.Length > 0)
            {
                try
                {
                    id = Int32.Parse(args[0]);
                }
                catch
                {
                    Console.WriteLine($"\"{args[0]}\" can not be converted to an int");
                    return false;
                }
            }

            var allConnections = ClientData.GetConnectionRegisterCopy();
            var ConnectionList = allConnections.Keys.ToList();

            Guid serverToChose = Guid.Empty;

            if(ConnectionList.Count == 0) 
            {
                Console.WriteLine("No connections open!");
                return true;
            }

            if (!(id >= 0 && id < ConnectionList.Count))
            {
                Console.WriteLine("Please select a connection:");
                Console.WriteLine("");
                for (int i = 0; i < ConnectionList.Count; i++)
                {
                    Console.WriteLine($"[{i}] - {allConnections[ConnectionList[i]].GetIPAddress(true)}");
                }
                Console.WriteLine("");
                Console.Write("ID: ");
                var newID = Console.ReadLine();

                if (newID is null or "")
                {
                    Console.WriteLine("No id was enterd.");
                    return false;
                }

                try
                {
                    id = Int32.Parse(newID);
                }
                catch
                {
                    Console.WriteLine($"\"{args[0]}\" can not be converted to an id");
                    return false;
                }
            }

            if (id >= 0 && id < ConnectionList.Count)
            {
                serverToChose = ConnectionList[id];
                var success = ClientData.SelectConnection(serverToChose);
                if (success) Console.WriteLine($"Server was selected");
                else Console.WriteLine("An unknown error ocured");
                return true;
            }

            Console.WriteLine("ID unknown!");
            return false;
        }
    }
}
