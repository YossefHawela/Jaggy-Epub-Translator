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
    }
}
