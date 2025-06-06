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
    internal class EpubReader
    {
        private string _filePath;

        public ZipArchive EpubArchive { get; private set; } = null!;

        public List<ZipArchiveEntry> XHtmlEntries { get; private set; } = new();
        public EpubReader(string filePath)
        {
            _filePath = filePath;
        }

        public async Task ReadEpubAsync(Action<float> progressAction, float precentageChange = 1)
        {
            EpubArchive = await Archive.ArchiveTools.GetArchiveAsync(_filePath, progressAction, precentageChange);

        }
        public async Task ReadEpubAsync()
        {
            EpubArchive = await Archive.ArchiveTools.GetArchiveAsync(_filePath);

        }
        public void ReadEpub()
        {
            EpubArchive = Archive.ArchiveTools.GetArchive(_filePath);

        }

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
