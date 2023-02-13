using VoTCore.User;
/**
 * @author      - Timeplex
 * 
 * @created     - 06.02.2023
 * 
 * @last_change - 06.02.2023
 */
namespace VoTCore.Exeptions
{
    public class PublicKeyMissingExeption : Exception
    {
        public PublicKeyMissingExeption() : base() { }
        public PublicKeyMissingExeption(string message) : base(message) { }

        public PublicKeyMissingExeption(PublicClient target) : this("No Public Key found:\n" + target.ToString() + "") {}
    }
}
