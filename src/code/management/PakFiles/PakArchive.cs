using System.IO.Compression;

namespace Uniray.PakFiles
{
    /// <summary>Represents an instance of <see cref="PakArchive"/>.</summary>
    internal class PakArchive
    {
        /// <summary>Creates a .pak file by combining the files of a specific folder.</summary>
        /// <param name="folder">Folder to use.</param>
        /// <param name="filePath">Path to the future .pak file.</param>
        public static void CreatePakFile(UFolder folder, string filePath)
        {
            // Init entries list and start offset
            List<PakFileEntry> entries = new List<PakFileEntry>();
            long offset = 0;
            int entriesCount = 0;

            // Open pak file stream
            FileStream pakFile = new FileStream(filePath, FileMode.Create);
            using (BinaryWriter writer = new BinaryWriter(pakFile))
            {
                writer.Write(entriesCount); // Index placeholder (Int-32)
                writer.Write(offset); // Table offset placeholder (Int-64)
                // Move writer after placeholder
                offset += 12; // Int-32 + Int-64 = 12 bytes

                // Write actual file data
                foreach (UStorage unit in folder.Files)
                {
                    if (unit is UFile)
                    {
                        // Get raw data
                        byte[] fileData = File.ReadAllBytes(unit.Path);
                        // Compress data
                        byte[] compressedData = Compress(fileData);
                        // Create pak file entry
                        PakFileEntry entry = new PakFileEntry(unit.Name, ((UFile)unit).Extension, fileData.Length, compressedData.Length, offset);
                        entries.Add(entry);
                        // Write binary data to the pak archive
                        writer.Write(compressedData);
                        offset += compressedData.Length;
                    }
                }
                // Keep files ending offset
                long tableOffset = offset;
                // Write entries informations
                foreach (PakFileEntry entry in entries) 
                {
                    writer.Write(entry.FileName);
                    writer.Write(entry.FileType);
                    writer.Write(entry.OriginalSize);
                    writer.Write(entry.CompressedSize);
                    writer.Write(entry.Index);
                }
                // Go back to the beginning of the file to write entries number + table offset
                pakFile.Seek(0, SeekOrigin.Begin);
                writer.Write(entries.Count);
                writer.Write(tableOffset);

                /* Which gives us a similar file structure : 
                 * 
                 * Entries count
                 * Table offset
                 * Files data at specific location
                 * Entries informations (Name, size, index), aka information table
                 * */
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
    }
}