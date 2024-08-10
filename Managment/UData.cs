namespace Uniray.Managment
{
    public class UData
    {
        /// <summary>
        /// Currently loaded project in the application
        /// </summary>
        private Project? currentProject;
        /// <summary>
        /// Currently selected file in the file manager
        /// </summary>
        private string? selectedFile;
        /// <summary>
        /// Currently loaded project in the application
        /// </summary>
        public Project? CurrentProject { get { return currentProject; } set { currentProject = value; } }
        /// <summary>
        /// Currently selected file in the file manager
        /// </summary>
        public string? SelectedFile { get { return selectedFile; } set { selectedFile = value; } }
        public UData() 
        {
            // Code here..
        }
    }
}
