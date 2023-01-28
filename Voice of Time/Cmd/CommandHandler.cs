using System.Runtime.InteropServices;
using Voice_of_Time.Cmd.Commands;
using VoTCore.Controll;
using VoTCore.Exeptions;

namespace Voice_of_Time.Cmd
{
    internal class CommandHandler : IDisposable
    {
        bool isCanceled = false;

        public CommandHandler() 
        {
            
        }

        public void Dispose()
        {
            isCanceled = true;
            [DllImport("kernel32.dll", SetLastError = true)]
            static extern IntPtr GetStdHandle(int nStdHandle);

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern bool CancelIoEx(IntPtr handle, IntPtr lpOverlapped);

            var handle = GetStdHandle(-10);
            CancelIoEx(handle, IntPtr.Zero);
        }

        public async Task Enable()
        {
            // Register default commands
            LoadDefaultCommands();

            while (!isCanceled)
            {
                string selectedServer = "";
                if(ClientData.CurrentConnection != null) 
                {
                    var connetion = ClientData.GetConnection((Guid)ClientData.CurrentConnection); 
                    if(connetion != null)
                    {
                        selectedServer =  connetion.GetIPAddress();
                    }
                }
                Console.Write(selectedServer + " > ");
                var input = Console.ReadLine();
                Console.WriteLine();
                await ProcessCommand(input);
                Console.WriteLine();
            }
        }

        private static void LoadDefaultCommands()
        {
            ClientData.TryRegisterCommand(new Help());
            ClientData.TryRegisterCommand(new Connect());
            ClientData.TryRegisterCommand(new Disconnect());
            ClientData.TryRegisterCommand(new Select());
            ClientData.TryRegisterCommand(new ListChats());
            ClientData.TryRegisterCommand(new Exit());
        }

        static async Task ProcessCommand(string? str)
        {
            if(str is null or "") return;
            // split the message
            var split = str.Split(" ");
            if (split.Length <= 0) return;
            // remove all not relativ parts
            split = split.Where(arg => arg != "" && arg != " ").ToArray();
            // Getting the command
            var commandExecuter = ClientData.GetCommandExecuter(split[0]);
            // check if commandExecuter is ok
            if (commandExecuter is null) {
                Console.WriteLine($"Command unknown \"{split[0]}\""); 
                return; 
            }
            var success = false;
            // Execute command
            if(commandExecuter is IConsoleCommandSync syncCE)
                success = syncCE.ExecuteCommand(split[0], split.Length > 1 ? split[1..] : Array.Empty<string>());
            else if(commandExecuter is IConsoleCommandAsync asyncCE)
                success = await asyncCE.ExecuteCommand(split[0], split.Length > 1 ? split[1..] : Array.Empty<string>());
            if (!success) Console.WriteLine($"Usage: {commandExecuter.Usage}");
        }
    }
}
