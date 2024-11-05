using Raylib_cs;
namespace Uniray.PakFiles
{
    /// <summary>Represents an instance of <see cref="PakReader"/>.</summary>
    internal class PakReader
    {
        private readonly Dictionary<string, PakFileEntry> entries = new Dictionary<string, PakFileEntry>();
        private readonly string _pakFilePath;
        private readonly long _tableOffset;
        private readonly int _fileCount;

        /// <summary>Creates a connection to .pak file.</summary>
        /// <param name="pakFilePath"></param>
        public PakReader(string pakFilePath)
        {
            _pakFilePath = pakFilePath;

            // Open and read pak file
            FileStream pakFile = new FileStream(_pakFilePath, FileMode.Open);
            using (BinaryReader reader = new BinaryReader(pakFile))
            {
                // Read entries count
                _fileCount = reader.ReadInt32(); // 4 bytes entry
                // Read table offset
                _tableOffset = reader.ReadInt64(); // 8 bytes entry
                // Move to table offset
                pakFile.Seek(_tableOffset, SeekOrigin.Begin);
                // Loop over different file entries
                for (int i = 0; i < _fileCount; i++)
                {
                    // Read entry informations
                    string fileName = reader.ReadString();
                    long fileSize = reader.ReadInt64();
                    long offset = reader.ReadInt64();

                    entries.Add(fileName, new PakFileEntry(fileName, fileSize, offset));
                }
            }
        }

        /// <summary>Loads a file from a .pak file from file name.</summary>
        /// <param name="fileName">File name to load.</param>
        /// <returns><see langword="byte[]"/> containing the file's bytes.</returns>
        /// <exception cref="Exception">File not found.</exception>
        public byte[] LoadFile(string fileName)
        {
            // Check if file exists in archive entries
            if (!entries.ContainsKey(fileName))
                throw new Exception($"File {fileName}not found in .pak archive");
            // Get pak entry
            PakFileEntry entry = entries[fileName];

            using (FileStream pakFile = new FileStream(_pakFilePath, FileMode.Open))
            {
                // Look for file index
                pakFile.Seek(entry.Index, SeekOrigin.Begin);
                // Read data from pak file
                byte[] buffer = new byte[entry.FileSize];
                pakFile.Read(buffer, 0, buffer.Length);
                // Return loaded data
                return buffer;
            }
        }

        /// <summary>Loads texture from .pak file from file name.</summary>
        /// <param name="fileName">File name to load.</param>
        /// <returns>Loaded <see cref="Texture2D"/> of the specified file.</returns>
        public Texture2D LoadTextureFromPack(string fileName)
        {
            // Read data of the file
            byte[] data = LoadFile(fileName);
            data.Where(x => x == 137).ToList().ForEach(x =>
            {
                Console.WriteLine("Ceci est une nouvelle section : ");
                for (int i = Array.IndexOf(data, x); i < Array.IndexOf(data, x) + 10; i++)
                {
                    Console.WriteLine(data[i]);
                }
            });
            File.WriteAllBytes("temp_image.png", data);
            // Load image from data
            Image image = Raylib.LoadImageFromMemory(".png", data);
            // Convert to Texture2D
            Texture2D texture = Raylib.LoadTextureFromImage(image);
            // Unload image
            Raylib.UnloadImage(image);
            return texture;
        }
    }
}