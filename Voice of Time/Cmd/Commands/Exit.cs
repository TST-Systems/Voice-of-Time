namespace Voice_of_Time.Cmd.Commands
{
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
