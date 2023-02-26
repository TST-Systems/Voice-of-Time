using System.Runtime.InteropServices;
using Voice_of_Time.Cmd.Commands;
using Voice_of_Time.Shared;
using VoTCore.Controll;

/**
 * @author      - Timeplex
 * 
 * @created     - 27.01.2023
 * 
 * @last_change - 12.02.2023
 */
namespace Voice_of_Time.Cmd
{
    /// <summary>
    /// Console command handler
    /// </summary>
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
            // Close the Console.ReadLine() command
            var handle = GetStdHandle(-10);
            CancelIoEx(handle, IntPtr.Zero);
        }

        /// <summary>
        /// Only called once. Enables the command listener
        /// </summary>
        public async Task Enable()
        {
            // Register default commands
            LoadDefaultCommands();

            while (!isCanceled)
            {
                string selectedServer = "";
                // Get and Print the current Server IP like "127.0.0.1 > "
                if(ClientData.CurrentConnection != null) 
                {
                    var connetion = ClientData.GetConnection((Guid)ClientData.CurrentConnection); 
                    if(connetion != null)
                    {
                        selectedServer =  connetion.Address;
                    }
                }
                Console.Write(selectedServer + " > ");
                var input = Console.ReadLine();
                Console.WriteLine();
                await ProcessCommand(input);
                Console.WriteLine();

                // Save data after each command
                ClientData.SaveData();
            }
        }

        /// <summary>
        /// Default commands
        /// </summary>
        protected virtual void LoadDefaultCommands()
        {
            ClientData.TryRegisterCommand(new Help());
            ClientData.TryRegisterCommand(new Connect());
            ClientData.TryRegisterCommand(new Disconnect());
            ClientData.TryRegisterCommand(new Select());
            ClientData.TryRegisterCommand(new Exit());
            ClientData.TryRegisterCommand(new Chat());
            ClientData.TryRegisterCommand(new Users());
            ClientData.TryRegisterCommand(new Stash());
        }

        /// <summary>
        /// Command processing
        /// </summary>
        /// <param name="str">command string</param>
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
            if(commandExecuter is IConsoleCommandSync syncCE) // syncrone
                success = syncCE.ExecuteCommand(split[0], split.Length > 1 ? split[1..] : Array.Empty<string>());
            else if(commandExecuter is IConsoleCommandAsync asyncCE) // asyncrone
                success = await asyncCE.ExecuteCommand(split[0], split.Length > 1 ? split[1..] : Array.Empty<string>());

            if (!success) Console.WriteLine($"Usage: {commandExecuter.Usage}");
        }
    }
}
