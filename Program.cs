// See https://aka.ms/new-console-template for more information

using HtmlAgilityPack;
using Jaggy_Epub_Translator.Modules.Archive;
using Jaggy_Epub_Translator.Modules.Epub;
using Jaggy_Epub_Translator.Modules.Html;

string path = @"F:\Temp\test\COPE__Classroom_Of_Political_Elites.epub";

EpubReader epubReader = new EpubReader(path);

await epubReader.ReadEpubAsync(p => Console.Write($"\rReading file: {p}%"));
Console.WriteLine();

//epubReader.EpubArchive?.Entries.ToList().ForEach(entry => Console.WriteLine(entry.FullName));

epubReader.ScanContent();

Console.WriteLine();

HtmlDocument[] documents = epubReader.GetXHtmlDocuments();


HtmlDocument[] TranslatedDocuments = await HtmlTools.HtmlTranslator(documents, precet =>
{
    Console.Write($"\rTranslating HTML: {precet}%");
});


//HtmlDocument[] TranslatedDocuments = documents;

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