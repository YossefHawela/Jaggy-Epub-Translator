using HtmlAgilityPack;
using Jaggy_Epub_Translator.Modules.Translators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Jaggy_Epub_Translator.Modules.Html
{
    internal static class HtmlTools
    {
        public static Translator Translator { get; set; } = new GoogleTranslator();
        public static async Task<HtmlDocument> HtmlTranslator(HtmlDocument document, Action<float> progressAction = null!)
        {
            string content = document.DocumentNode.InnerText;

            string Html = document.DocumentNode.InnerHtml;

            string[] pragraphs = content.Split(new[] { "\r\n", "\r", "\n","." }, StringSplitOptions.RemoveEmptyEntries);
           

            // Remove empty paragraphs
            pragraphs = pragraphs.Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

            for (int i = 0; i < pragraphs.Length; i++)
            {
                string? prag = pragraphs[i];

                var translation = await Translator.TranslateAsync(prag.Trim(), GTranslatorAPI.Languages.en, GTranslatorAPI.Languages.ar);
                Html = Html.Replace(prag.Trim(), translation.TranslatedText);


                progressAction?.Invoke((i / (float)pragraphs.Length) * 100);

            }

            progressAction?.Invoke(100);


            var TranslatedDocument = new HtmlDocument();


            TranslatedDocument.LoadHtml(Html);

            return TranslatedDocument;

        }

        public static async Task<HtmlDocument[]> HtmlTranslator(HtmlDocument[] htmlDocuments,Action<float> progressAction = null!)
        {

            HtmlDocument[] translatedDocuments = new HtmlDocument[htmlDocuments.Length];

            for (int i = 0; i < htmlDocuments.Length; i++)
            {

                HtmlDocument? document = htmlDocuments[i];

                translatedDocuments[i] = await HtmlTranslator(document, SubPrecentage =>
                {
                    float Precentage = ((i / (float)htmlDocuments.Length) * 100) + (SubPrecentage * (1 / (float)htmlDocuments.Length));

                   
                    progressAction?.Invoke(Precentage);
                });
            }

            progressAction?.Invoke(100);

            return translatedDocuments;
        }


        public static Stream GetStreamFromHtmlDocument(HtmlDocument document)
        {
            string htmlString = document.DocumentNode.OuterHtml;

            byte[] byteArray = Encoding.UTF8.GetBytes(htmlString);

            MemoryStream stream = new MemoryStream(byteArray);

            return stream;

        }

        public static Stream[] GetStreamsFromHtmlDocuments(HtmlDocument[] documents)
        {
            Stream[] streams = new Stream[documents.Length];
            for (int i = 0; i < documents.Length; i++)
            {
                streams[i] = GetStreamFromHtmlDocument(documents[i]);
            }
            return streams;
        }
    }
}
