using GTranslatorAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jaggy_Epub_Translator.Modules.Translators
{
    internal class GoogleTranslator:Translator
    {
        public override TranslatorType Type => TranslatorType.GoogleTranslate;

        private GTranslatorAPIClient translatorAPIClient = new GTranslatorAPIClient();
        public override async Task<Translation> TranslateAsync(string text, Languages sourceLanguage, Languages targetLanguage)
        {
           

                if (text.Length > 1500)
                {
                    var parts = Tools.SplitByLength(text, 1500);
                    var translations = new List<Translation>();
                    foreach (var part in parts)
                    {
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
                        return await translatorAPIClient.TranslateAsync(sourceLanguage, targetLanguage, text);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Translation {text.Length} failed: {ex.Message}");

                        await Task.Delay(1000);

                        return await TranslateAsync(text, sourceLanguage, targetLanguage);

                    }
        }

          
        }
    }
}
