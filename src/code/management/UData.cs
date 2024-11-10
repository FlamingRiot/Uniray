namespace Uniray
{
    /// <summary>Represents an instance of <see cref="UData"/>.</summary>
    public static class UData
    {
        /// <summary>The currently loaded <see cref="Project"/>.</summary>
        internal static Project? CurrentProject;

        /// <summary>The currently selected <see cref="UFile"/>s.</summary>
        internal static List<UStorage> SelectedFiles = new List<UStorage>();

        /// <summary>The currently opened <see cref="Modal"/>.</summary>
        internal static string? CurrentModal;

        /// <summary>The currently renaming field.</summary>
        internal static UStorage? CurrentRenaming;

        /// <summary>The currently displayed scene.</summary>
        internal static Scene CurrentScene = new Scene();

        /// <summary>The currently selected game objects.</summary>
        internal static List<GameObject3D> Selection = new List<GameObject3D>();

        /// <summary>The currently running game simulation.</summary>
        internal static GameSimulation? GameSimulation;

        /// <summary>The exit code of the latest closed <see cref="Modal"/>.</summary>
        internal static int? LastModalExitCode;
    }
}