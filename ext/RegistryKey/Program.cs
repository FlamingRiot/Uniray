using Microsoft.Win32;
using System.Diagnostics;

class Program
{
    //public const string APP_DIRECTORY = @"C:\Program Files\Uniray\";

    static void Main()
    {
        string appName = "Uniray";
        //string appPath =  APP_DIRECTORY + @"bin\Uniray.exe";
        string appPath = @"C:\Users\ComtesseE1\Desktop\uniray\src\bin\Debug\net7.0\Uniray.exe";
        //string iconPath = APP_DIRECTORY + @"share\Uniray.ico";
        string iconPath = @"C:\Users\ComtesseE1\Desktop\uniray\src\data\logo.ico";

        using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"*\shell", true))
        {
            if (key != null)
            {
                using (RegistryKey newKey = key.CreateSubKey(appName))
                {
                    newKey.SetValue("", $"Ouvrir avec Uniray");
                    newKey.SetValue("Icon", iconPath);
                    using (RegistryKey commandKey = newKey.CreateSubKey("command"))
                    {
                        commandKey.SetValue("", $"\"{appPath}\" \"%1\"");
                    }
                }
            }
        }
    }
}