#pragma warning disable CA1416

using Microsoft.Win32;

namespace Uniray
{
    /// <summary>Represents an instance of <see cref="UData"/>.</summary>
    internal static class UData
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

    /// <summary>Represents data internal to the engine.</summary>
    internal static class InternalData
    {
        internal static string? _vs2022Path;
    
        /// <summary>Looks for the required data into the user's system.</summary>
        internal static void LookUp()
        {
            // Visual Studio 2022 lookup

            string[] editions = { "Enterprise", "Professional", "Community" };
            string baseKey = @"SOFTWARE\WOW6432Node\Microsoft\VisualStudio\SxS\VS7";
            string? vsPath;

            for (int i = 0; i < editions.Length; i++) 
            {
                using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(baseKey))
                {
                    if (key != null)
                    {
                        vsPath = key.GetValue(editions[i] + "2022") as string;
                        if (!string.IsNullOrEmpty(vsPath))
                        {
                            vsPath = Path.Combine(vsPath, "Common7\\IDE\\devenv.exe");
                            if (File.Exists(vsPath))
                            {
                                _vs2022Path = vsPath;
                                break;
                            }
                        }
                    }
                }            
            }
            if (_vs2022Path == null) UDebugger.Send("No suitable Visual Studio version installed", ConsoleColor.Red);
        }
    }    
}