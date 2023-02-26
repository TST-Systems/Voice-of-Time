/**
 * @author      - Timeplex
 * 
 * @created     - 28.01.2023
 * 
 * @last_change - 28.01.2023
 */
namespace Voice_of_Time.Cmd.Commands
{
    /// <summary>
    /// Closes all Connections corectly and closes the programm
    /// </summary>
    internal class Exit : Disconnect
    {
        public override string Command => "exit";

        private readonly string[] aliases = { "e" };
        public override string[] Aliases => aliases;

        public override string Usage => "exit";

        public override bool ExecuteCommand(string command, string[] args)
        {
            CloseAllConections(); 
            Environment.Exit(0);
            return true;
        }
    }
}
