namespace Uniray
{
    /// <summary>Represents a <see cref="UFile"/> instance.</summary>
    public class UFile : UStorage, IRenamable
    {
        /// <summary>Extension of the file.</summary>
        public string Extension;

        /// <summary>Returns the full name (including extension of the file.</summary>
        public string FullName { get { return Name + '.' + Extension; } }

        /// <summary>Creates an instance of a <see cref="UFile"/> object.</summary>
        /// <param name="path">The absolute path to the file.</param>
        public UFile(string path) : base(path)
        {
            string[] slashed = path.Split('/');
            Name = slashed.Last().Split('.').First();
            Extension = path.Split('.').Last();
        }

        /// <summary>Renames the file both virtually and effectively.</summary>
        /// <param name="name">The new name of the file</param>
        public void Rename(string name)
        {
            string oldPath = Path;
            Path = Path.Replace($"{Name}", $"{name}");
            Name = name;
            File.Move(oldPath, Path);
        }
    }
}