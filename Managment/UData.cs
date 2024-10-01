namespace Uniray
{
    public static class UData
    {
        /// <summary>The currently displayed <see cref="UFolder"/>.</summary>
        internal static UFolder? CurrentFolder;

        /// <summary>The currently loaded <see cref="Project"/>.</summary>
        internal static Project? CurrentProject;

        /// <summary>The currently selected <see cref="UFile"/>.</summary>
        internal static string? SelectedFile;

        /// <summary>The currently opened <see cref="Modal"/>.</summary>
        internal static string? CurrentModal;

        /// <summary>The exit code of the latest closed <see cref="Modal"/>.</summary>
        internal static int? LastModalExitCode;
    }
}