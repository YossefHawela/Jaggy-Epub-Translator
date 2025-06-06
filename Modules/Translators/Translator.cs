using GTranslatorAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jaggy_Epub_Translator.Modules.Translators
{
    /// <summary>
    /// Base abstract class for translation services. Provides a common interface for different translation implementations.
    /// </summary>
    internal abstract class Translator
    {
        /// <summary>
        /// Defines the available types of translation services.
        /// </summary>
        public enum TranslatorType
        {
            /// <summary>
            /// Google Translate service implementation.
            /// </summary>
            GoogleTranslate,
        }

        /// <summary>
        /// Gets the type of translator implementation.
        /// </summary>
        public abstract TranslatorType Type { get; }

        /// <summary>
        /// Translates the specified text from source language to target language asynchronously.
        /// </summary>
        /// <param name="text">The text to translate.</param>
        /// <param name="sourceLanguage">The language to translate from.</param>
        /// <param name="targetLanguage">The language to translate to.</param>
        /// <param name="progressAction">Optional callback to report translation progress (0-100).</param>
        /// <returns>A Task containing the translation result.</returns>
        public abstract Task<Translation> TranslateAsync(string text, Languages sourceLanguage, Languages targetLanguage, Action<float> progressAction = null!);
    }
}
