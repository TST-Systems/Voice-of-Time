/**
 * @author      - Timeplex
 * 
 * @created     - 01.02.2023
 * 
 * @last_change - 01.02.2023
 */
namespace VoTCore.Algorithms
{
    /// <summary>
    /// <para>Functions for convertig numbers into base36 numbers.</para>
    /// <para>The used symbols are 0-9 and case insesitiv A-Z/a-z</para>
    /// </summary>
    public static class Base36
    {
        const string BaseAlphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const char   Filler       = '='; // TODO: replace flling with 0s

        /// <summary>
        /// Encode an POSITIV number into Base36
        /// </summary>
        /// <param name="value">Positiv number</param>
        /// <returns>BAse36 string</returns>
        public static string Encode(long value)
        {
            string result  = string.Empty;
            int baseLenght = BaseAlphabet.Length;

            if (value < 0) return "";

            do
            {
                result = BaseAlphabet[(int)(value % baseLenght)] + result;
                value /= baseLenght;
            } while (value > 0);

            return result;
        }

        /// <summary>
        /// Encode an POSITIV number into Base36
        /// </summary>
        /// <param name="value">Positiv number</param>
        /// <param name="minLenght">Fill up to at least have a minimum lenght</param>
        /// <returns>BAse36 string</returns>
        public static string Encode(long value, int minLenght) //TODO: Rework by filling it with 0 at the front like numbers do
        {
            string result = Encode(value);

            for(int i = minLenght - result.Length; i > 0; i--)
            {
                result += Filler;
            }

            return result;
        }

        /// <summary>
        /// Decode a Base36 string back into a numnber
        /// </summary>
        /// <param name="value">Base36 string</param>
        /// <returns>Number</returns>
        public static long Decode(string value)
        {
            value = value.ToUpper();

            value = new string(value.Where(x => BaseAlphabet.Contains(x)).ToArray()) ?? "";

            long result    = 0;
            int baseLenght = BaseAlphabet.Length;

            for (int i = 0; i < value.Length; i++)
            {
                result = result * baseLenght + BaseAlphabet.IndexOf(value[i]);
            }

            return result;
        }

    }
}
