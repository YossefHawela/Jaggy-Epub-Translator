using HtmlAgilityPack;
using Jaggy_Epub_Translator.Modules.Archive;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
namespace Jaggy_Epub_Translator.Modules.Epub
{
    /// <summary>
    /// Provides functionality to read and parse EPUB files, extracting their content and HTML documents.
    /// </summary>
    internal class EpubReader
    {
        private string _filePath;

        /// <summary>
        /// Gets the ZIP archive containing the EPUB file contents.
        /// </summary>
        public ZipArchive EpubArchive { get; private set; } = null!;

        /// <summary>
        /// Gets the list of XHTML entries found in the EPUB file.
        /// </summary>
        public List<ZipArchiveEntry> XHtmlEntries { get; private set; } = new();

        /// <summary>
        /// Initializes a new instance of the EpubReader class.
        /// </summary>
        /// <param name="filePath">The path to the EPUB file to read.</param>
        public EpubReader(string filePath)
        {
            _filePath = filePath;
        }

        /// <summary>
        /// Reads the EPUB file asynchronously with progress reporting.
        /// </summary>
        /// <param name="progressAction">Callback to report progress (0-100).</param>
        /// <param name="precentageChange">Minimum percentage change required to trigger progress report.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ReadEpubAsync(Action<float> progressAction, float precentageChange = 1)
        {
            EpubArchive = await Archive.ArchiveTools.GetArchiveAsync(_filePath, progressAction, precentageChange);
        }

        /// <summary>
        /// Reads the EPUB file asynchronously without progress reporting.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ReadEpubAsync()
        {
            EpubArchive = await Archive.ArchiveTools.GetArchiveAsync(_filePath);
        }

        /// <summary>
        /// Reads the EPUB file synchronously.
        /// </summary>
        public void ReadEpub()
        {
            EpubArchive = Archive.ArchiveTools.GetArchive(_filePath);
        }

        /// <summary>
        /// Scans the EPUB content to identify and collect XHTML entries.
        /// This method must be called after reading the EPUB file.
        /// </summary>
        public void ScanContent()
        {
            var metaContainerEntry = ArchiveTools.GetArchiveEntry(EpubArchive, "META-INF/container.xml");

            XmlReader metaContainerXml = XmlReader.Create(metaContainerEntry.Open());

            ZipArchiveEntry contentEntry = null!;

            string ContainerFolderPath = "";

            while (metaContainerXml.Read())
            {
                if (metaContainerXml.NodeType == XmlNodeType.Element && metaContainerXml.Name == "rootfile")
                {
                    string fullPath = metaContainerXml.GetAttribute("full-path")!;

                    ContainerFolderPath = Path.GetDirectoryName(fullPath)!;

                    contentEntry = ArchiveTools.GetArchiveEntry(EpubArchive, fullPath);

                    break;
                }
            }

            XmlReader ContentXml = XmlReader.Create(contentEntry.Open());

            while (ContentXml.Read())
            {

                if (ContentXml.NodeType == XmlNodeType.Element && ContentXml.Name == "item" &&
                    ContentXml.GetAttribute("media-type") == "application/xhtml+xml")
                {

                    string fullPath = $"{ContainerFolderPath}/{ContentXml.GetAttribute("href")}";

                    var XHtmlEntry = ArchiveTools.GetArchiveEntry(EpubArchive, fullPath);

                    XHtmlEntries.Add(XHtmlEntry);

                }


            }
        }

        /// <summary>
        /// Gets an HTML document from the EPUB file at the specified index.
        /// </summary>
        /// <param name="index">The index of the XHTML entry to retrieve.</param>
        /// <returns>An HtmlDocument containing the parsed XHTML content.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is outside the valid range.</exception>
        public HtmlDocument GetXHtmlDocument(int index)
        {
            if (index < 0 || index >= XHtmlEntries.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }
            var stream = XHtmlEntries[index].Open();
            HtmlDocument document = new HtmlDocument();
            document.Load(stream);
            return document;
        }

        /// <summary>
        /// Gets all HTML documents from the EPUB file with optional progress reporting.
        /// </summary>
        /// <param name="progressAction">Optional callback to report progress (0-100).</param>
        /// <returns>An array of HtmlDocument objects containing all parsed XHTML content.</returns>
        public HtmlDocument[] GetXHtmlDocuments(Action<float> progressAction = null!)
        {
            HtmlDocument[] documents = new HtmlDocument[XHtmlEntries.Count];
            for (int i = 0; i < XHtmlEntries.Count; i++)
            {
                documents[i] = GetXHtmlDocument(i);
                progressAction?.Invoke(i / (float)XHtmlEntries.Count * 100);
            }

            progressAction?.Invoke(100);

            return documents;

        }
    }

}
