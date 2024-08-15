namespace Uniray
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
        /// Currently opened modal
        /// </summary>
        private string? currentModal;
        /// <summary>
        /// Last exit code of the modal
        /// </summary>
        private int? lastModalExitCode;
        /// <summary>
        /// The currently displayed folder
        /// </summary>
        public UFolder CurrentFolder;
        /// <summary>
        /// Currently loaded project in the application
        /// </summary>
        public Project? CurrentProject { get { return currentProject; } set { currentProject = value; } }
        /// <summary>
        /// Currently selected file in the file manager
        /// </summary>
        public string? SelectedFile { get { return selectedFile; } set { selectedFile = value; } }
        /// <summary>
        /// Currently opened modal
        /// </summary>
        public string? CurrentModal { get { return currentModal; } set { currentModal = value; } }
        /// <summary>
        /// Last exit code of the modal
        /// </summary>
        public int? LastModalExitCode { get { return lastModalExitCode; } set { lastModalExitCode = value; } }
        /// <summary>
        /// UData constructor
        /// </summary>
        /// <param name="folder">The default folder to display from the assets</param>
        public UData(UFolder folder) 
        {
            CurrentFolder = folder;
        }
    }
}