namespace Uniray
{
    public class UFile
    {
        /// <summary>
        /// The absolute path to the file 
        /// </summary>
        private string path;
        /// <summary>
        /// The displayed name in the file manager
        /// </summary>
        private string name;
        /// <summary>
        /// The displayed name in the file manager
        /// </summary>
        public string Path { get { return path; } set { path = value; } }
        /// <summary>
        /// The displayed name in the file manager
        /// </summary>
        public string Name { get { return name; } set { name = value; } }
        /// <summary>
        /// UFile constructor
        /// </summary>
        /// <param name="path">The absolute path to the file</param>
        public UFile(string path)
        {
            this.path = path;
            string[] slashed = path.Split('/');
            this.name = slashed.Last().Split('.').First();
        }
        /// <summary>
        /// Rename the file
        /// </summary>
        /// <param name="name">The new name of the file</param>
        public void Rename(string name)
        {
            string oldPath = path;
            path = path.Replace($"{this.name}", $"{name}");
            this.name = name;
            File.Move(oldPath, path);
        }
        /// <summary>
        /// Stringed object
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return $"This file is called: {name} and is located at: {path}";
        }
    }
}