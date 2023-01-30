/**
 * @author      - Timeplex
 * 
 * @created     - 27.01.2023
 * 
 * @last_change - 27.01.2023
 */
namespace VoTCore.Exeptions
{
    public class UnauthorizedCharExeption : Exception
    {
        public UnauthorizedCharExeption() : base("Error: Unauthorized char was found") { }
        public UnauthorizedCharExeption(string message) : base($"Error: Unauthorized char was found\n{message}") { }
    }
}
