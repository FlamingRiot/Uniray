namespace Uniray.DatFiles
{
    /// <summary>Represents an instance of <see cref="DatFileEntry"/>.</summary>
    internal class DatFileEntry
    {
        /// <summary>Name ofthe entry.</summary>
        public string Name;
        /// <summary>Index of the entry in the file.</summary>
        public int Index;
        /// <summary>Size of the entry in the file.</summary>
        public int Size;

        /// <summary>Creates an instance of <see cref="DatFileEntry"/>.</summary>
        /// <param name="name">Name of the entry.</param>
        /// <param name="index">Index of the entry.</param>
        /// <param name="size">Size of the entry.</param>
        public DatFileEntry(string name, int index, int size)
        {
            Name = name;
            Index = index;
            Size = size;
        }

        /// <summary>Returns informations about the current instance.</summary>
        /// <returns>Informations as a <see langword="string"/>.</returns>
        public override string ToString()
        {
            return $"Name: {Name}, {Size} Ko";
        }
    }
}
