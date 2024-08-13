namespace Uniray
{
    public class UFile : UStorage, IRenamable
    {
        /// <summary>
        /// UFile constructor
        /// </summary>
        /// <param name="path">The absolute path to the file</param>
        public UFile(string path) : base(path)
        {
            string[] slashed = path.Split('/');
            Name = slashed.Last().Split('.').First();
        }
        /// <summary>
        /// Rename the file
        /// </summary>
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