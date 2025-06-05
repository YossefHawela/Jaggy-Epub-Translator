using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jaggy_Epub_Translator.Modules.Archive
{
    internal static class ArchiveTools
    {
        /// <summary>
        /// reurns a ZipArchive from the given file path asynchronously.
        /// </summary>
        /// <param name="archivePath"></param>
        /// <returns></returns>
        public static async Task<ZipArchive> GetArchiveAsync(string archivePath)
        {
            var archiveBytes = await File.ReadAllBytesAsync(archivePath);

            using var memoryStream = new MemoryStream(archiveBytes);

            return new ZipArchive(memoryStream);

        }


        /// <summary>
        /// gets a ZipArchive from the given file path asynchronously and reports progress.
        /// </summary>
        /// <param name="archivePath"></param>
        /// <param name="progressAction"></param>
        /// <param name="precentageForChange"></param>
        /// <returns></returns>
        public static async Task<ZipArchive> GetArchiveAsync(string archivePath,Action<float> progressAction, float precentageForChange = 1)
        {
            
            const int bufferSize = 81920; // 80 KB buffer size
            var fileInfo = new FileInfo(archivePath);
            long totalBytes = fileInfo.Length;
            long bytesRead = 0;

            using var fileStream = new FileStream(archivePath,FileMode.Open,
                FileAccess.Read, FileShare.Read, bufferSize, useAsync: true);

            var memoryStream = new MemoryStream();

            byte[] buffer = new byte[bufferSize];

            int read;
            int lastProgressPercentage = 0;

            while((read = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await memoryStream.WriteAsync(buffer, 0, read);
                bytesRead += read;

                int currentPercent = (int)((bytesRead * 100f) / totalBytes);

                if(currentPercent - lastProgressPercentage >= precentageForChange)
                {
                    lastProgressPercentage = currentPercent;
                    progressAction?.Invoke(currentPercent);
                }

            }

            memoryStream.Seek(0, SeekOrigin.Begin);

            return new ZipArchive(memoryStream);
        }


        /// <summary>
        /// returns a ZipArchive from the given file path.
        /// </summary>
        /// <param name="archivePath"></param>
        /// <returns></returns>
        public static ZipArchive GetArchive(string archivePath)
        {
            var archiveBytes = File.ReadAllBytes(archivePath);

            using var memoryStream = new MemoryStream(archiveBytes);

            return new ZipArchive(memoryStream);

        }



        /// <summary>
        /// gets a specific entry from the ZipArchive by its name.
        /// </summary>
        /// <param name="archive"></param>
        /// <param name="entryName"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static ZipArchiveEntry GetArchiveEntry(ZipArchive archive, string entryName)
        {
            var entry = archive.GetEntry(entryName);
            if (entry == null)
            {
                throw new FileNotFoundException($"The entry '{entryName}' was not found in the archive.");
            }
            return entry;
        }

        /// <summary>
        /// reads the content of a ZipArchiveEntry as a string asynchronously.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static async Task<string> ReadEntryAsStringAsync(ZipArchiveEntry entry, Encoding encoding = null!)
        {
      
            encoding ??= Encoding.UTF8; // Default to UTF-8 if no encoding is specified
            using var stream = entry.Open();
            using var reader = new StreamReader(stream, encoding);
            return await reader.ReadToEndAsync();
  
        }

        /// <summary>
        /// reads the content of a ZipArchiveEntry as a string.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ReadEntryAsString(ZipArchiveEntry entry, Encoding encoding = null!)
        {
            encoding ??= Encoding.UTF8; // Default to UTF-8 if no encoding is specified
            using var stream = entry.Open();
            using var reader = new StreamReader(stream, encoding);
            return  reader.ReadToEnd();
        }

        public static Dictionary<string, Stream> GetAllEntriesAsDictionary(ZipArchive archive)
        {
            var entries = new Dictionary<string, Stream>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in archive.Entries)
            {
                if (!entries.ContainsKey(entry.FullName))
                {
                    entries.Add(entry.FullName, entry.Open());
                }
            }
            return entries;
        }

        public static byte[] CreateZipArchive(Dictionary<string, Stream> files)
        {
            using var outputStream = new MemoryStream();
            using (var archive = new ZipArchive(outputStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (var kvp in files)
                {
                    var entry = archive.CreateEntry(kvp.Key);
                    using var entryStream = entry.Open();
                    kvp.Value.CopyTo(entryStream);
                }
            }
            return outputStream.ToArray();
        }

    }
}
