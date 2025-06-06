using GTranslatorAPI;
using HtmlAgilityPack;
using Jaggy_Epub_Translator.Modules.Archive;
using Jaggy_Epub_Translator.Modules.Epub;
using Jaggy_Epub_Translator.Modules.Html;

// Check for required arguments
if (args.Length < 4)
{
    Console.WriteLine("Usage: <program> <filePath> <sourceLang> <targetLang> <outputPath>");
    return;
}

string filePath = args[0];

Languages sourceLanguage = Enum.Parse<Languages>(args[1], true);
Languages targetLanguage = Enum.Parse<Languages>(args[2], true);

string outputPath = args[3];

// Validate input file existence
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

// Validate output path
if (string.IsNullOrWhiteSpace(outputPath))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("ERROR: Output path is empty. Process aborted.");
    Console.ResetColor();
    return;
}

// Determine output directory and file name based on outputPath argument
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
    // No extension: treat as directory or filename without extension
    if (Directory.Exists(outputPath))
    {
        outputDir = outputPath;
        if (!outputDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
            outputDir += Path.DirectorySeparatorChar;

        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        string defaultFileName = $"{inputFileNameWithoutExt}_{timestamp}{inputFileExt}";
        outputPath = Path.Combine(outputDir, defaultFileName);
    }
    else
    {
        // Treat as file name without extension in current directory
        outputDir = Directory.GetCurrentDirectory();
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        outputPath = Path.Combine(outputDir, $"{outputPath}_{timestamp}{inputFileExt}");
    }
}

// Warn if output directory does not exist
if (!Directory.Exists(outputDir))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("WARNING: Output directory does not exist.");
    Console.ResetColor();
    return;
}

// Read and scan the EPUB file
EpubReader epubReader = new EpubReader(filePath);

await epubReader.ReadEpubAsync(p => Console.Write($"\rReading file: {p}%"));
Console.WriteLine();

epubReader.ScanContent();
Console.WriteLine();

// Extract XHTML documents from the EPUB
HtmlDocument[] documents = epubReader.GetXHtmlDocuments();

// Translate all XHTML documents
HtmlDocument[] TranslatedDocuments = await HtmlTools.HtmlTranslator(documents, sourceLanguage, targetLanguage, precet =>
{
    Console.Write($"\rTranslating EPUB: {precet.ToString("0.00")}%");
});

Console.WriteLine();

// Convert translated documents to streams
Stream[] streams = HtmlTools.GetStreamsFromHtmlDocuments(TranslatedDocuments);

// Get all files from the EPUB archive
var AllFiles = ArchiveTools.GetAllEntriesAsDictionary(epubReader.EpubArchive);
var xhtmlEntries = epubReader.XHtmlEntries;

// Replace original XHTML entries with translated versions
for (int i = 0; i < TranslatedDocuments.Length; i++)
{
    var entry = xhtmlEntries[i];
    AllFiles[entry.FullName] = streams[i];
}

// Create new EPUB archive with translated content
var ArchiveBytes = ArchiveTools.CreateZipArchive(AllFiles);

string targetPath = outputPath;

// Write the new EPUB file to disk
File.WriteAllBytes(targetPath, ArchiveBytes);

Console.WriteLine($"\nOutput saved to: {outputPath}");
