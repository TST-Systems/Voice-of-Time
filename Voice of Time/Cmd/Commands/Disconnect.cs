using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoTCore.Controll;

namespace Voice_of_Time.Cmd.Commands
{
    internal class Disconnect : IConsoleCommandSync
    {
        public string Command => "disconnect";

        private readonly string[] aliases = { "d", "dct" };
        public string[] Aliases => aliases;

        public string Usage => "disconnect [-all]";

        public bool ExecuteCommand(string command, string[] args)
        {
            var disconnectAll = false;
            if(args.Where(x => x.ToLower() == "-all").Any())
            {
                disconnectAll = true;
            }

            if (disconnectAll)
            {
                var allConections = ClientData.GetAllConnectionIDs();
                foreach (var connection in allConections)
                {
                    ClientData.CloseConnection(connection);
                }
                Console.WriteLine("Closed all connections");
                return true;
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
    }
}
