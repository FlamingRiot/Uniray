namespace Uniray.PakFiles
{
    /// <summary>Represents an instance of <see cref="PakFileEntry"/>.</summary>
    internal class PakFileEntry
    {
        /// <summary>File name of the entry.</summary>
        public string FileName;
        /// <summary>File type of the entry.</summary>
        public string FileType;
        /// <summary>File original size of the entry.</summary>
        public long OriginalSize;
        /// <summary>File compressed size of the entry.</summary>
        public long CompressedSize;
        /// <summary>File index in archive.</summary>
        public long Index;

        /// <summary>Creates an instance of <see cref="PakFileEntry"/>.</summary>
        /// <param name="name">File name of the entry.</param>
        /// <param name="fileType">File type of the entry</param>
        /// <param name="size"></param>
        public PakFileEntry(string name, string fileType, long originalSize, long compressedSize, long index)
        {
            FileName = name;
            FileType = fileType;
            OriginalSize = originalSize;
            CompressedSize = compressedSize;
            Index = index;
        }

        /// <summary>Returns informations about the current instance.</summary>
        /// <returns>Informations as a <see langword="string"/>.</returns>
        public override string ToString()
        {
            return $"Name: {FileName}, {CompressedSize} Ko ({OriginalSize} Ko uncompressed";
        }
    }
}