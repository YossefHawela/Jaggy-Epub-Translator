using GTranslatorAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jaggy_Epub_Translator.Modules.Translators
{
    internal abstract class Translator
    {
        public enum TranslatorType
        {
            GoogleTranslate,
        }

        public abstract TranslatorType Type { get; }

        public abstract Task<Translation> TranslateAsync(string text, Languages sourceLanguage, Languages targetLanguage);
    }
}
