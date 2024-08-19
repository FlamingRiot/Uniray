namespace Uniray
{
    public class UFolder : UStorage, IRenamable
    {
        /// <summary>
        /// The files contained inside of the folder
        /// </summary>
        public List<UStorage> Files;
        /// <summary>
        /// The upstream folder according to this one
        /// </summary>
        private UFolder? upstreamFolder;
        /// <summary>
        /// The upstream folder according to this one
        /// </summary>
        public UFolder? UpstreamFolder { get { return upstreamFolder; } set { upstreamFolder = value; } }
        /// <summary>
        /// UFolder constructor
        /// </summary>
        /// <param name="path">The absolute path to the folder</param>
        public UFolder(string path, List<UStorage> files) : base(path)
        {
            Name = path.Split('/').Last();
            this.Files = files;
        }
        /// <summary>
        /// Add a file to the folder
        /// </summary>
        /// <param name="file">The file to add</param>
        public void AddFile(UStorage file)
        {
            Files.Add(file);
        }
        /// <summary>
        /// Delete a file from the folder
        /// </summary>
        /// <param name="file"></param>
        public void DeleteFile(UStorage file)
        {
            Files.Remove(file);
        }
        /// <summary>
        /// Rename the folder
        /// </summary>
        /// <param name="name">The new name to give</param>
        public void Rename(string name)
        {
            string oldPath = Path;
            Path = Path.Replace($"{this.Name}", $"{name}");
            Name = name;
            Directory.Move(oldPath, Path);
        }
    }
}