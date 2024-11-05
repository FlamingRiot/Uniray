namespace Uniray.PakFiles
{
    internal class PakArchive
    {
        public static void CreatePakFile(UFolder folder, string filePath)
        {
            // Init entries list and start offset
            List<PakFileEntry> entries = new List<PakFileEntry>();
            long offset = 0;

            // Open pak file stream
            FileStream pakFile = new FileStream(filePath, FileMode.Create);
            using (BinaryWriter writer = new BinaryWriter(pakFile))
            {
                writer.Write(offset); // Index placeholder

                // Loop over directory files
                foreach (UStorage unit in folder.Files)
                {
                    if (unit is UFile)
                    {
                        string relativePath = Path.GetRelativePath(folder.Path, filePath);
                        byte[] fileData = File.ReadAllBytes(unit.Path);

                        // Create file entry for the pak archive
                        PakFileEntry entry = new PakFileEntry(unit.Name, fileData.Length, offset);
                        entries.Add(entry);

                        // Write binary data to the pak archive
                        writer.Write(fileData);
                        offset += fileData.Length;
                    }
                }
                // Goes back to the beginning of the file to write entries count
                pakFile.Seek(0, SeekOrigin.Begin);
                writer.Write(entries.Count);

                // Writes index
                foreach (PakFileEntry entry in entries) 
                {
                    writer.Write(entry.FileName);
                    writer.Write(entry.FileSize);
                    writer.Write(entry.Index);
                }

                /* Which gives us a similar file structure : 
                 * 
                 * Number of entries
                 * Entries informations (Name, size, index)
                 * Files data at specific location
                 * */
            }
        }
    }
}
