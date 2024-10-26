namespace Uniray
{
    /// <summary>
    /// IRenamable interface
    /// </summary>
    public interface IRenamable
    {
        /// <summary>
        /// Rename the unit storage
        /// </summary>
        /// <param name="name">The new name to give</param>
        public void Rename(string name);
    }
    public class UStorage
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
        /// The displayed name in the file manager
        /// </summary>
        public string Path { get { return path; } set { path = value; } }
        /// <summary>
        /// The displayed name in the file manager
        /// </summary>
        public string Name { get { return name; } set { name = value; } }

        /// <summary>Creates an empty instance of <see cref="UStorage"/>.</summary>
        public UStorage()
        {
            path = "";
            name = "";
        }

        /// <summary>Creates an instance of <see cref="UStorage"/>.</summary>
        /// <param name="path">Path to the storage unit</param>
        public UStorage(string path) 
        { 
            this.path = path;
            // Wait for a specific constructor to correctly cut the name out of the path
            name = "";
        }

        /// <summary>
        /// Stringed storage unit
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return $"This storage unit is located at: {path}";
        }
    }
}