// See https://aka.ms/new-console-template for more information

using GTranslatorAPI;
using HtmlAgilityPack;
using Jaggy_Epub_Translator.Modules.Archive;
using Jaggy_Epub_Translator.Modules.Epub;
using Jaggy_Epub_Translator.Modules.Html;

if (args.Length < 4)
{
    Console.WriteLine("Usage: <program> <filePath> <sourceLang> <targetLang> <outputPath>");
    return;
}

string filePath = args[0];

Languages sourceLanguage = Enum.Parse<Languages>(args[1], true);
Languages targetLanguage = Enum.Parse<Languages>(args[2], true);

string outputPath = args[3];

if (!File.Exists(filePath))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"ERROR: Input file '{filePath}' does not exist.");
    Console.ResetColor();
    return;
}

string outputDir;
string inputFileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
string inputFileExt = Path.GetExtension(filePath);

if (string.IsNullOrWhiteSpace(outputPath))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("ERROR: Output path is empty. Process aborted.");
    Console.ResetColor();
    return;
}

if (Path.HasExtension(outputPath))
{
    outputDir = Path.GetDirectoryName(outputPath)!;
    if (string.IsNullOrEmpty(outputDir))
    {
        outputDir = Directory.GetCurrentDirectory();
        outputPath = Path.Combine(outputDir, outputPath);
    }
}
else
{
    outputDir = outputPath;
    if (!outputDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
        outputDir += Path.DirectorySeparatorChar;

    if (!Directory.Exists(outputDir))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("WARNING: Output directory does not exist. Process aborted.");
        Console.ResetColor();
        return;
    }

    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
    string defaultFileName = $"{inputFileNameWithoutExt}_{timestamp}{inputFileExt}";
    outputPath = Path.Combine(outputDir, defaultFileName);
}

if (!Directory.Exists(outputDir))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("WARNING: Output directory does not exist. Process aborted.");
    Console.ResetColor();
    return;
}


EpubReader epubReader = new EpubReader(filePath);

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

string targetPath = outputPath;

File.WriteAllBytes(targetPath,ArchiveBytes);

Console.WriteLine("Done!");