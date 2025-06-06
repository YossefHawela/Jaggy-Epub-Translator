using GTranslatorAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jaggy_Epub_Translator.Modules.Translators
{
    internal class GoogleTranslator:Translator
    {
        public override TranslatorType Type => TranslatorType.GoogleTranslate;

        private GTranslatorAPIClient translatorAPIClient = new GTranslatorAPIClient();

        public GoogleTranslator()
        {
            translatorAPIClient.Settings.SplitStringBeforeTranslate = false;
            translatorAPIClient.Settings.ParallelizeTranslationOfSegments = true;
            translatorAPIClient.Settings.NetworkQueryTimeout = 15000;

        }
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
