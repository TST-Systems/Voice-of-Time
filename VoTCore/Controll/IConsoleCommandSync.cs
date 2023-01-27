using Voice_of_Time;

namespace VoTCore.Controll
{
    public interface IConsoleCommandSync : IConsoleCommand
    {
        /// <summary>
        /// Function which will be called, when a command fits the command or an alias
        /// </summary>
        /// <param name="command">command/alias which triggert this function</param>
        /// <param name="args">
        /// <para>All args besides the command</para>
        /// <br>cmd:help 0:-a 1:-c 2:--alpha 3:chess</br>
        /// </param>
        /// <returns>Command executed successfully</returns>
        public bool ExecuteCommand(string command, string[] args);
    }
}
