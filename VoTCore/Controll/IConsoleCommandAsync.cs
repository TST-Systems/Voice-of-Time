/**
 * @author      - Timeplex
 * 
 * @created     - 27.01.2023
 * 
 * @last_change - 27.01.2023
 */
namespace VoTCore.Controll
{
    /// <summary>
    /// Interface for async command executer
    /// </summary>
    public interface IConsoleCommandAsync : IConsoleCommand
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
        public Task<bool> ExecuteCommand(string command, string[] args);
    }
}
