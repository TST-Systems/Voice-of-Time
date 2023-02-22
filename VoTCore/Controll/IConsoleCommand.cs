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
    /// Interface as base for console-command-executer
    /// </summary>
    public interface IConsoleCommand
    {
        /// <summary>
        /// <br>Main command as case insesitiv</br>
        /// <br>Will also be used in 'help' command</br>
        /// Allowed charakters: a-z, 0-9, _ 
        /// </summary>
        public string Command { get; }
        /// <summary>
        /// Aliases of command wich also calls this ConsoleCommand
        /// </summary>
        public string[] Aliases { get; }
        /// <summary>
        /// Helping text to show when the commandexecuter returns false
        /// </summary>
        public string Usage { get; }
    }
}