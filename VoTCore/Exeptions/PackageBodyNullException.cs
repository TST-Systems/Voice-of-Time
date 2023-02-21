/**
 * @author      - Timeplex, SalzstangeManga
 * 
 * @created     - 11.02.2022
 * 
 * @last_change - 11.02.2023
 */
namespace VoTCore.Exeptions
{
    public class PackageBodyNullException : Exception
    {
        public PackageBodyNullException() : base() { }

        public PackageBodyNullException(string message) : base(message) { }
    }
}
