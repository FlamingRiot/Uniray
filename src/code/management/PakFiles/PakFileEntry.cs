namespace Uniray.PakFiles
{
    /// <summary>Represents an instance of <see cref="PakFileEntry"/>.</summary>
    internal class PakFileEntry
    {
        /// <summary>File name of the entry.</summary>
        public string FileName;
        /// <summary>File original size of the entry.</summary>
        public long FileSize;
        /// <summary>File index in archive.</summary>
        public long Index;

        /// <summary>Creates an instance of <see cref="PakFileEntry"/>.</summary>
        /// <param name="name">Name of the file.</param>
        /// <param name="size"></param>
        public PakFileEntry(string name, long size, long index)
        {
            FileName = name;
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