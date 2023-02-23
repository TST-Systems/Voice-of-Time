/**
 * @author      - Timeplex
 * 
 * @created     - 27.01.2023
 * 
 * @last_change - 27.01.2023
 */
namespace VoTCore.Exeptions
{
    /// <summary>
    /// Exception for using a charactor that is not allowed in the declared alphabet
    /// </summary>
    public class UnauthorizedCharExeption : Exception
    {
        public UnauthorizedCharExeption() : base("Error: Unauthorized char was found") { }
        public UnauthorizedCharExeption(string message) : base($"Error: Unauthorized char was found\n{message}") { }
    }
}
