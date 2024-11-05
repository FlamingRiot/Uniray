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
        public long FileSize;
        /// <summary>File index in archive.</summary>
        public long Index;

        /// <summary>Creates an instance of <see cref="PakFileEntry"/>.</summary>
        /// <param name="name">File name of the entry.</param>
        /// <param name="fileType">File type of the entry</param>
        /// <param name="size"></param>
        public PakFileEntry(string name, string fileType, long size, long index)
        {
            FileName = name;
            FileType = fileType;
            FileSize = size;
            Index = index;
        }

        /// <summary>Returns informations about the current instance.</summary>
        /// <returns>Informations as a <see langword="string"/>.</returns>
        public override string ToString()
        {
            return $"Name: {FileName}, {FileSize} Ko";
        }
    }
}