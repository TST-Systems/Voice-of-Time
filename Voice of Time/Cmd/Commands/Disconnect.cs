using VoTCore.Controll;

/**
 * @author      - Timeplex
 * 
 * @created     - 27.01.2023
 * 
 * @last_change - 27.01.2023
 */
namespace Voice_of_Time.Cmd.Commands
{
    internal class Disconnect : IConsoleCommandSync
    {
        public virtual string Command => "disconnect";

        private readonly string[] aliases = { "d", "dct" };
        public virtual string[] Aliases => aliases;

        public virtual string Usage => "disconnect [-all]";

        public virtual bool ExecuteCommand(string command, string[] args)
        {
            var disconnectAll = false;
            if(args.Where(x => x.ToLower() == "-all").Any())
            {
                disconnectAll = true;
            }

            if (disconnectAll)
            {
                return CloseAllConections();
            }

            //
            var currentConnectionID = ClientData.CurrentConnection;
            if (currentConnectionID is null) 
            {
                Console.WriteLine("No connection selected!");
                return true;
            }
            ClientData.CloseConnection((Guid)currentConnectionID);
            if(ClientData.CurrentConnection is null) { Console.WriteLine("Connection was successfully closed"); } else { Console.WriteLine("An Error ocured!"); }
            return true;
        }

        protected static bool CloseAllConections()
        {
            var allConections = ClientData.GetAllConnectionIDs();
            foreach (var connection in allConections)
            {
                ClientData.CloseConnection(connection);
            }
            Console.WriteLine("Closed all connections");
            return true;
        }
    }
}
