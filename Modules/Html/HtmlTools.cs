using GTranslatorAPI;
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
        public static async Task<HtmlDocument> HtmlTranslator(HtmlDocument document,Languages sourceLanguage,Languages TargetLanguage, Action<float> progressAction = null!)
        {
            var nodes = document.DocumentNode
                .Descendants()
                .Where(n => n.NodeType == HtmlNodeType.Text && !string.IsNullOrWhiteSpace(n.InnerText))
                .ToList();

            int total = nodes.Count;

            for (int i = 0; i < total; i++)
            {
                var node = nodes[i];
                string originalText = node.InnerText.Trim();

                var translation = await Translator.TranslateAsync(
                    originalText,
                    GTranslatorAPI.Languages.en,
                    GTranslatorAPI.Languages.ar,
                    subPercent =>
                    {
                        float percent = ((i / (float)total) * 100) + (subPercent * (1f / total));
                        progressAction?.Invoke(percent);
                    });

                node.InnerHtml = translation.TranslatedText;
            }

            progressAction?.Invoke(100);

            return document;

        }

        public static async Task<HtmlDocument[]> HtmlTranslator(HtmlDocument[] htmlDocuments, Languages sourceLanguage, Languages TargetLanguage, Action<float> progressAction = null!)
        {

            HtmlDocument[] translatedDocuments = new HtmlDocument[htmlDocuments.Length];

            for (int i = 0; i < htmlDocuments.Length; i++)
            {

                HtmlDocument? document = htmlDocuments[i];

                translatedDocuments[i] = await HtmlTranslator(document, sourceLanguage,TargetLanguage, SubPrecentage =>
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
