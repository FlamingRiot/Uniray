using Microsoft.Win32;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        string appName = "Uniray";
        string fileExtension = ".uproj";
        string appPath = Directory.GetCurrentDirectory(); // Get current directory
        appPath = appPath.Replace($"uniray\\ext\\RegistryKey", $"uniray\\src").Replace("net8.0", "net7.0") + $"\\Uniray.exe"; // Get path to .exe
        string commandDir = appPath.Split("\\bin\\Debug\\net7.0").First(); // Get directory to application source
        string iconPath = commandDir + "\\data\\logo.ico"; // Get application icon
        // Build application 
        string command = $"/C cd \"{commandDir}\" && dotnet build";
        Process.Start("CMD.exe", command); // Build Unirays

        //Si le "Ouvrir avec" ne marche pas commente ce bloc de code et décommente celui du dessus
        RegistryKey menuKey = Registry.ClassesRoot.OpenSubKey(@"*\shell", true);
        RegistryKey openWithKey = menuKey.CreateSubKey(appName);
        openWithKey.SetValue("", $"Ouvrir avec {appName}");
        openWithKey.SetValue("icon", iconPath);
        RegistryKey openKey = openWithKey.CreateSubKey("command");
        openKey.SetValue("", $"\"{appPath}\" \"%1\"");


        // Création des clés de registre pour l'extension de fichier
        Registry.SetValue($@"HKEY_CLASSES_ROOT\{fileExtension}", "", appName);
        Registry.SetValue($@"HKEY_CLASSES_ROOT\{appName}\DefaultIcon", "", iconPath);
        Registry.SetValue($@"HKEY_CLASSES_ROOT\{appName}\shell\open\command", "", $"\"{appPath}\" \"%1\"");

        // Notification au système de mettre à jour les associations de fichier
        SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
    }

    [System.Runtime.InteropServices.DllImport("shell32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
    private static extern void SHChangeNotify(int wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);
}