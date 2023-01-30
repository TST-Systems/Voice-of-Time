/**
 * @author      - Timeplex
 * 
 * @created     - 27.01.2023
 * 
 * @last_change - 27.01.2023
 */
namespace VoTCore.Exeptions
{
    public class EntryAlreadyExistsExeption : Exception
    {
        public EntryAlreadyExistsExeption()               : base("Error: Entry already Exists") { }
        public EntryAlreadyExistsExeption(string message) : base($"Error: Entry already Exists\n{message}") { }
    }
}
