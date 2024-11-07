using System.IO.Compression;

namespace Uniray.PakFiles
{
    /// <summary>Represents an instance of <see cref="PakArchive"/>.</summary>
    internal class PakArchive
    {
        // -----------------------------------------------------------
        // Private and internal instances
        // -----------------------------------------------------------

        private static List<PakFileEntry> _entries = new List<PakFileEntry>();
        private static long _offset = 0;

        /// <summary>Creates a .pak file by combining the files of a specific folder.</summary>
        /// <param name="folder">Folder to use.</param>
        /// <param name="filePath">Path to the future .pak file.</param>
        public static void CreatePakFile(UFolder folder, string filePath)
        {
            int entriesCount = 0;

            // Open pak file stream
            FileStream pakFile = new FileStream(filePath, FileMode.Create);
            using (BinaryWriter writer = new BinaryWriter(pakFile))
            {
                // Write placeholder data to the beginning of the file
                writer.Write(entriesCount); // Index placeholder (Int-32)
                writer.Write(_offset); // Table offset placeholder (Int-64)
                // Move writer after placeholder
                _offset += 12; // Int-32 + Int-64 = 12 bytes

                // Write actual file data
                WriteFolderToPak(writer, folder);

                // Keep files ending offset
                long tableOffset = _offset;
                // Write entries informations
                foreach (PakFileEntry entry in _entries) 
                {
                    writer.Write(entry.FileName);
                    writer.Write(entry.FileType);
                    writer.Write(entry.OriginalSize);
                    writer.Write(entry.CompressedSize);
                    writer.Write(entry.Index);
                }
                // Go back to the beginning of the file to write entries number + table offset
                pakFile.Seek(0, SeekOrigin.Begin);
                writer.Write(_entries.Count);
                writer.Write(tableOffset);

                /* Which gives us a similar file structure : 
                 * 
                 * Entries count
                 * Table offset
                 * Files data at specific location
                 * Entries informations (Name, size, index), aka information table
                 * */
                // Reset private attributes
                _entries.Clear();
                _offset = 0;
            }
        }

        /// <summary>Compresses an array of bytes with the GZIP algorithm.</summary>
        /// <param name="data">Data to compress.</param>
        /// <returns>Compressed data.</returns>
        internal static byte[] Compress(byte[] data)
        {
            MemoryStream memoryStream = new MemoryStream();
            using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            {
                gzipStream.Write(data, 0, data.Length);
                gzipStream.Close();
                return memoryStream.ToArray();
            }
        }

        /// <summary>Decompresses an array of bytes with the GZIP algorithm</summary>
        /// <param name="data">Data to decompress.</param>
        /// <returns>Decompressed data.</returns>
        internal static byte[] Decompress(byte[] data) 
        {
            MemoryStream memoryStream = new MemoryStream(data);
            GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
            using (MemoryStream resultStream = new MemoryStream())
            {
                gzipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }

        /// <summary>Writes the data of a file to a .pak archive.</summary>
        /// <param name="ufolder">UFolder containing the files to read data from.</param>
        private static void WriteFolderToPak(BinaryWriter writer, UFolder ufolder)
        {
            // Loop over folder files
            foreach (UStorage unit in ufolder.Files)
            {
                // Write file
                if (unit is UFile file)
                {
                    // Get raw file data
                    byte[] data = File.ReadAllBytes(file.Path);
                    // Compress data
                    byte[] compressedData = Compress(data);
                    // Create pak file entry
                    PakFileEntry entry = new PakFileEntry(file.Name, file.Extension, data.Length, compressedData.Length, _offset);
                    _entries.Add(entry);
                    // Write binary data to .pak archive
                    writer.Write(compressedData);
                    _offset += compressedData.Length;
                }
                // Enter new folder
                else if (unit is UFolder folder)
                {
                    WriteFolderToPak(writer, folder);
                }
            }
        }
    }
}