using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jaggy_Epub_Translator.Modules
{
    /// <summary>
    /// Provides utility methods for string manipulation and processing.
    /// </summary>
    internal static class Tools
    {
        /// <summary>
        /// Splits a string into chunks of specified length.
        /// </summary>
        /// <param name="input">The string to split.</param>
        /// <param name="chunkSize">The maximum length of each chunk.</param>
        /// <returns>A list of string chunks, each with maximum length of chunkSize.</returns>
        /// <exception cref="ArgumentException">Thrown when chunkSize is less than or equal to 0.</exception>
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

        /// <summary>
        /// Splits a string by a separator and groups the results into chunks of specified size.
        /// </summary>
        /// <param name="input">The string to split.</param>
        /// <param name="separator">The separator to split the string by.</param>
        /// <param name="groupSize">The number of elements in each group.</param>
        /// <returns>A list of strings, where each string contains groupSize number of original elements joined by the separator.</returns>
        /// <remarks>Returns an empty list if input is null/empty, separator is null/empty, or groupSize is less than or equal to 0.</remarks>
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

        /// <summary>
        /// Splits a string by multiple separators and groups the results into chunks of specified size.
        /// </summary>
        /// <param name="input">The string to split.</param>
        /// <param name="separators">An array of separators to split the string by.</param>
        /// <param name="groupSize">The number of elements in each group.</param>
        /// <returns>A list of strings, where each string contains groupSize number of original elements joined by the first separator.</returns>
        /// <remarks>
        /// Returns an empty list if input is null/empty, separators is null/empty, or groupSize is less than or equal to 0.
        /// When joining the chunks back together, only the first separator from the array is used.
        /// </remarks>
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
