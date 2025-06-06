﻿/// <summary>
/// Provides utility methods for translating and processing HTML documents using a specified Translator.
/// Includes methods for translating single or multiple HtmlDocument instances, as well as converting them to streams.
/// </summary>
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
    /// <summary>
    /// Static helper class for HTML translation and conversion operations.
    /// </summary>
    internal static class HtmlTools
    {
        /// <summary>
        /// Gets or sets the Translator used for translating HTML content.
        /// </summary>
        public static Translator Translator { get; set; } = new GoogleTranslator();

        /// <summary>
        /// Asynchronously translates all text nodes in the given HtmlDocument from the source language to the target language.
        /// Reports progress via the provided action.
        /// </summary>
        /// <param name="document">The HTML document to translate.</param>
        /// <param name="sourceLanguage">The source language.</param>
        /// <param name="TargetLanguage">The target language.</param>
        /// <param name="progressAction">Optional action to report translation progress (0-100).</param>
        /// <returns>The translated HtmlDocument.</returns>
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

        /// <summary>
        /// Asynchronously translates all text nodes in an array of HtmlDocument objects.
        /// Reports overall progress via the provided action.
        /// </summary>
        /// <param name="htmlDocuments">Array of HTML documents to translate.</param>
        /// <param name="sourceLanguage">The source language.</param>
        /// <param name="TargetLanguage">The target language.</param>
        /// <param name="progressAction">Optional action to report translation progress (0-100).</param>
        /// <returns>Array of translated HtmlDocument objects.</returns>
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

        /// <summary>
        /// Converts an HtmlDocument to a UTF-8 encoded memory stream.
        /// </summary>
        /// <param name="document">The HTML document to convert.</param>
        /// <returns>A stream containing the HTML content.</returns>
        public static Stream GetStreamFromHtmlDocument(HtmlDocument document)
        {
            string htmlString = document.DocumentNode.OuterHtml;

            byte[] byteArray = Encoding.UTF8.GetBytes(htmlString);

            MemoryStream stream = new MemoryStream(byteArray);

            return stream;

        }

        /// <summary>
        /// Converts an array of HtmlDocument objects to an array of UTF-8 encoded memory streams.
        /// </summary>
        /// <param name="documents">Array of HTML documents to convert.</param>
        /// <returns>Array of streams containing the HTML content.</returns>
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
