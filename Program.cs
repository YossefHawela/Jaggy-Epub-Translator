// See https://aka.ms/new-console-template for more information

using GTranslatorAPI;
using HtmlAgilityPack;
using Jaggy_Epub_Translator.Modules.Archive;
using Jaggy_Epub_Translator.Modules.Epub;
using Jaggy_Epub_Translator.Modules.Html;


if (args.Length < 3)
{
    Console.WriteLine("Usage: <program> <filePath> <sourceLang> <targetLang>");
    return;
}

string path = args[0];
Languages sourceLanguage = Enum.Parse<Languages>(args[1], true);
Languages targetLanguage = Enum.Parse<Languages>(args[2], true);

EpubReader epubReader = new EpubReader(path);

await epubReader.ReadEpubAsync(p => Console.Write($"\rReading file: {p}%"));
Console.WriteLine();


epubReader.ScanContent();

Console.WriteLine();

HtmlDocument[] documents = epubReader.GetXHtmlDocuments();


HtmlDocument[] TranslatedDocuments = await HtmlTools.HtmlTranslator(documents, sourceLanguage,targetLanguage, precet =>
{
    Console.Write($"\rTranslating EPUB: {precet.ToString("0.00")}%");
});

Console.WriteLine();



Stream[] streams = HtmlTools.GetStreamsFromHtmlDocuments(TranslatedDocuments);


var AllFiles = ArchiveTools.GetAllEntriesAsDictionary(epubReader.EpubArchive);

var xhtmlEntries = epubReader.XHtmlEntries;


for (int i = 0; i < TranslatedDocuments.Length; i++)
{
    var entry = xhtmlEntries[i];
    AllFiles[entry.FullName] = streams[i];
}


var ArchiveBytes = ArchiveTools.CreateZipArchive(AllFiles);

string targetPath = Path.Combine(Path.GetDirectoryName(path)!, "Translated.epub");

File.WriteAllBytes(targetPath,ArchiveBytes);

Console.WriteLine("Done!");