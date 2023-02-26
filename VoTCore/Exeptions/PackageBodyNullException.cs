/**
 * @author      - Timeplex, SalzstangeManga
 * 
 * @created     - 11.02.2022
 * 
 * @last_change - 11.02.2023
 */
namespace VoTCore.Exeptions
{
    /// <summary>
    /// Exception for an VOTP that contains non body a body was expected
    /// </summary>
    public class PackageBodyNullException : Exception
    {
        public PackageBodyNullException() : base() { }

        public PackageBodyNullException(string message) : base(message) { }
    }
}
