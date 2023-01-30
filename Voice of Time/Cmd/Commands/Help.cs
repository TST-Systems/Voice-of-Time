using VoTCore.Controll;

/**
 * @author      - Timeplex
 * 
 * @created     - 27.01.2023
 * 
 * @last_change - 28.01.2023
 */
namespace Voice_of_Time.Cmd.Commands
{
    internal class Help : IConsoleCommandSync, ICommandHelp
    {
        public string Command => "help";

        private readonly string[] aliases = { "h" };
        public string[] Aliases => aliases;

        public string Usage => "help [page] [-c <COMMAND>]";


        public string[] commandHelp = {
                "help [page] [-c <COMMAND>]\n" +
                "Parameter:\n" +
                "page :int    -> Switch to a specific Help page\n" +
                "-c <Command> -> Show the help context of a command"
        };
        public string[] CommandHelp => commandHelp;

        public bool ExecuteCommand(string command, string[] args)
        {
            int?    page = null;
            string? parC = null;
            // getting all args
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-c" && parC is null && args.Length - i > 1)
                {
                    parC= args[++i];
                    continue;
                }
                if(page is null)
                {
                    try
                    {
                        page = int.Parse(args[i]); //TODO
                    }
                    catch(Exception) { }
                }
            }
            if (page is null)
            {
                page = 1;
            }
            // execute help for speifc command
            if (parC is not null)
            {
                var getCommand = ClientData.GetCommandExecuter(parC);
                if(getCommand is null)
                {
                    Console.WriteLine($"Command unknown: {parC}");
                    return true;
                }
                if (getCommand is ICommandHelp iCH)
                {
                    if (page > iCH.CommandHelp.Length || page <= 0)
                    {
                        page = 1;
                    }
                    Console.WriteLine(iCH.CommandHelp[(int)page  - 1]);
                    Console.WriteLine($"Page {page}/{iCH.CommandHelp.Length}");
                    return true;
                }
                Console.WriteLine(getCommand.Usage);
                return true;
            }
            // Show a list of all commands
            int entrysPerSite = 10;
            // Get all commands and sort them
            var Commands = ClientData.GetAllComands().OrderBy(x => x.Command).ToArray();
            if(page > Math.Ceiling((double) Commands.Length / (double) entrysPerSite))
            {
                page = 1;
            }

            Console.WriteLine("LIST OF ALL COMMANDS");
            Console.WriteLine("Use help -c <command> for more informations");
            Console.WriteLine();
            for (int i = ((int)page - 1) * 10; i < Commands.Length && i < page * entrysPerSite; i++)
            {
                Console.WriteLine(string.Format("{0,-16} -> {1,-50}", Commands[i].Command.ToUpper(), Commands[i].Usage));
            }
            Console.WriteLine();
            Console.WriteLine($"Page {page}/{Math.Ceiling((double) Commands.Length / (double) entrysPerSite)}");
            //
            return true;
        }
    }

    /// <summary>
    /// Addon for client console commands for showing a seperat helping text when "help -c [command]" is called
    /// </summary>
    internal interface ICommandHelp
    {
        /// <summary>
        /// 
        /// </summary>
        public string[] CommandHelp { get; }
    }
}
