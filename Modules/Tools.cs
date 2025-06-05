using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jaggy_Epub_Translator.Modules
{
    internal static class Tools
    {
        public static List<string> SplitByLength(string input, int chunkSize)
        {
            if (chunkSize <= 0) throw new ArgumentException("Chunk size must be positive, genius.");
            var result = new List<string>();
            for (int i = 0; i < input.Length; i += chunkSize)
            {
                int length = Math.Min(chunkSize, input.Length - i);
                result.Add(input.Substring(i, length));
            }
            return result;
        }

        public static List<string> SplitEveryN(string input, string separator, int groupSize)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(separator) || groupSize <= 0)
                return new List<string>();

            string[] parts = input.Split(new string[] { separator }, StringSplitOptions.None);
            List<string> result = new();

            for (int i = 0; i < parts.Length; i += groupSize)
            {
                var chunk = parts.Skip(i).Take(groupSize);
                result.Add(string.Join(separator, chunk));
            }

            return result;
        }

        public static List<string> SplitEveryN(string input, string[] separators, int groupSize)
        {
            if (string.IsNullOrEmpty(input) || separators == null || separators.Length == 0 || groupSize <= 0)
                return new List<string>();

            string[] parts = input.Split(separators, StringSplitOptions.None);
            List<string> result = new();

            for (int i = 0; i < parts.Length; i += groupSize)
            {
                var chunk = parts.Skip(i).Take(groupSize);
                result.Add(string.Join(separators[0], chunk)); // joins with first separator
            }

            return result;
        }





    }
}
