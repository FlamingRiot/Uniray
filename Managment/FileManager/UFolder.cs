namespace Uniray
{
    public class UFolder
    {
        /// <summary>
        /// The absolute path to the folder 
        /// </summary>
        private string path;
        /// <summary>
        /// The displayed name in the file manager
        /// </summary>
        private string name;
        /// <summary>
        /// The files contained inside of the folder
        /// </summary>
        List<UFile> files;
        /// <summary>
        /// The displayed name in the file manager
        /// </summary>
        public string Path { get { return path; } set { path = value; } }
        /// <summary>
        /// The displayed name in the file manager
        /// </summary>
        public string Name { get { return name; } set { name = value; } }
        /// <summary>
        /// UFolder constructor
        /// </summary>
        /// <param name="path">The absolute path to the folder</param>
        public UFolder(string path, List<UFile> files)
        {
            this.path = path;
            this.name = path.Split('/').Last();
            this.files = files;
        }
        /// <summary>
        /// Add a file to the folder
        /// </summary>
        /// <param name="file">The file to add</param>
        public void AddFile(UFile file)
        {
            files.Add(file);
        }
        /// <summary>
        /// Delete a file from the folder
        /// </summary>
        /// <param name="file"></param>
        public void DeleteFile(UFile file)
        {
            files.Remove(file);
        }
        /// <summary>
        /// Rename the folder
        /// </summary>
        /// <param name="name">The new name to give</param>
        public void Rename(string name)
        {
            string oldPath = path;
            path = path.Replace($"{this.name}", $"{name}");
            this.name = name;
            Directory.Move(oldPath, path);
        }
        /// <summary>
        /// Stringed folder
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return $"This folder is called: {name} and is located at: {path}";
        }
    }
}