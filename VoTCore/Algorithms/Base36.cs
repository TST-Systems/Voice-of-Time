using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoTCore.Algorithms
{
    public static class Base36
    {
        const string BaseAlphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const char   Filler       = '=';

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

        public static string Encode(long value, int minLenght)
        {
            string result = Encode(value);

            for(int i = minLenght - result.Length; i > 0; i--)
            {
                result += Filler;
            }

            return result;
        }


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
