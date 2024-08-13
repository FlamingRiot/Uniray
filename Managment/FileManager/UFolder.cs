namespace Uniray
{
    public class UFolder : UStorage, IRenamable
    {
        /// <summary>
        /// The files contained inside of the folder
        /// </summary>
        List<UStorage> files;
        /// <summary>
        /// Return a readonly list
        /// </summary>
        public List<UStorage> Files { get { return files; } }
        /// <summary>
        /// UFolder constructor
        /// </summary>
        /// <param name="path">The absolute path to the folder</param>
        public UFolder(string path, List<UStorage> files) : base(path)
        {
            Name = path.Split('/').Last();
            this.files = files;
        }
        /// <summary>
        /// Add a file to the folder
        /// </summary>
        /// <param name="file">The file to add</param>
        public void AddFile(UStorage file)
        {
            files.Add(file);
        }
        /// <summary>
        /// Delete a file from the folder
        /// </summary>
        /// <param name="file"></param>
        public void DeleteFile(UStorage file)
        {
            files.Remove(file);
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