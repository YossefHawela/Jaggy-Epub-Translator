/// <summary>
/// Provides translation services using the Google Translate API.
/// This class implements the Translator interface, handling text translation requests,
/// including splitting large texts and reporting progress. It manages retries on failure
/// and configures the underlying GTranslatorAPIClient for optimal translation performance.
/// </summary>
using GTranslatorAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jaggy_Epub_Translator.Modules.Translators
{
    /// <summary>
    /// Translator implementation utilizing Google Translate via GTranslatorAPIClient.
    /// Handles large text by splitting, supports progress reporting, and retries on errors.
    /// </summary>
    internal class GoogleTranslator:Translator
    {
        /// <summary>
        /// Gets the type of translator (GoogleTranslate).
        /// </summary>
        public override TranslatorType Type => TranslatorType.GoogleTranslate;

        private GTranslatorAPIClient translatorAPIClient = new GTranslatorAPIClient();

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleTranslator"/> class,
        /// configuring the translation client settings.
        /// </summary>
        public GoogleTranslator()
        {
            translatorAPIClient.Settings.SplitStringBeforeTranslate = false;
            translatorAPIClient.Settings.ParallelizeTranslationOfSegments = true;
            translatorAPIClient.Settings.NetworkQueryTimeout = 15000;

        }

        /// <summary>
        /// Asynchronously translates the specified text from the source language to the target language.
        /// Splits large texts, reports progress, and retries on failure.
        /// </summary>
        /// <param name="text">The text to translate.</param>
        /// <param name="sourceLanguage">The source language.</param>
        /// <param name="targetLanguage">The target language.</param>
        /// <param name="progressAction">Optional action to report translation progress (0-100).</param>
        /// <returns>A <see cref="Task{Translation}"/> representing the asynchronous translation operation.</returns>
        public override async Task<Translation> TranslateAsync(string text, Languages sourceLanguage, Languages targetLanguage,Action<float> progressAction=null!)
        {
           

            if (text.Length > 1500)
            {
                var parts = Tools.SplitByLength(text, 1500);
                var translations = new List<Translation>();

                for (int i = 0; i < parts.Count; i++)
                {

                    progressAction?.Invoke((i / (float)parts.Count) * 100);


                    string? part = parts[i];
                    var translation = await TranslateAsync(part, sourceLanguage, targetLanguage);
                    translations.Add(translation);
                }

                return new Translation
                {
                    OriginalText = text,
                    TranslatedText = string.Join("", translations)
                };

            }
            else
            {
                try
                {
                    progressAction?.Invoke(100);

                    var translation = await translatorAPIClient.TranslateAsync(sourceLanguage, targetLanguage, text);


                    return translation;
                        
                }
                catch
                {
                    await Task.Delay(1000);

                    return await TranslateAsync(text, sourceLanguage, targetLanguage);

                }
            }

          
        }
    }
}
