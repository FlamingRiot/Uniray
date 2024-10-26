﻿using RayGUI_cs;
using static RayGUI_cs.RayGUI;
using Raylib_cs;
using System.Text;
using static Raylib_cs.Raylib;
using System;
using System.Numerics;

namespace Uniray
{
    internal static unsafe class FileManager
    {
        public static UFolder ModelFolder = new UFolder();
        public static UFolder TextureFolder = new UFolder();
        public static UFolder SoundFolder = new UFolder();
        public static UFolder AnimationFolder = new UFolder();
        public static UFolder ScriptFolder = new UFolder();

        /// <summary>The currently displayed <see cref="UFolder"/>.</summary>
        internal static UFolder CurrentFolder = new UFolder();

        /// <summary>Loads the entire folder architecture of the project.</summary>
        public static void Init(string directory)
        {
            // Set assets folder paths
            ModelFolder.Path = directory + "/assets/models";
            TextureFolder.Path = directory + "/assets/textures";
            SoundFolder.Path = directory + "/assets/sounds";
            AnimationFolder.Path = directory + "/assets/animations";
            ScriptFolder.Path = directory + "/assets/scripts";

            // Load assets
            LoadAssets();
        }

        /// <summary>Displays the content of the current folder into the file manager.</summary>
        public static void Draw()
        {
            InjectFiles();

            List<UStorage> _units = CurrentFolder.Files;
            for (int i = 0; i < _units.Count; i++) 
            {
                // Define file row
                short row = (short)(i / 10);
                // Define drawing position
                int xPos = Uniray.UI.Components["fileManager"].X + 150 * (i % 10 + 1) - 100;
                int yPos = Uniray.UI.Components["fileManager"].Y + 60 + row * 120;

                DrawStorageUnit(_units[i], xPos, yPos, i);
                UpdateStorageUnit(_units[i], xPos, yPos);
            }
        }

        /// <summary>Draws a single storage unit to the file manager, according to its place in the list.</summary>
        /// <param name="unit">Storage unit to draw.</param>
        /// <param name="index">Index of the storage unit.</param>
        private static void DrawStorageUnit(UStorage unit, int x, int y, int index)
        {
            // Shorten the text
            string lbl = "";
            if (unit.Name.Length >= 10) { lbl = unit.Name.Remove(5) + "..."; }
            else lbl = unit.Name;

            // Draw the appropriate element
            if (unit is UFile)
            {
                DrawPanel(new Panel(x, y, 1, 0, HardRessource.Textures["file"], ""));
                DrawLabel(new Label(x + 10 - lbl.Length / 2, y + HardRessource.Textures["file"].Height + 20, lbl));
            }
            else if (unit is UFolder)
            {
                DrawPanel(new Panel(x, y, 1, 0, HardRessource.Textures["folder"], ""));
                DrawLabel(new Label(x + 10 + unit.Name.Length / 3, y + HardRessource.Textures["file"].Height + 20, lbl));
            }
        }

        private static void UpdateStorageUnit(UStorage unit, int x, int y)
        {
            // Delete action
            if (IsMouseButtonPressed(MouseButton.Middle))
            {
                if (Hover(x, y, HardRessource.Textures["file"].Width, HardRessource.Textures["file"].Height))
                {
                    if (unit is UFolder)
                    {
                        // Delte the folder along with all its content
                        Directory.Delete(unit.Path, true);
                    }
                    else
                    {
                        // Delete the physical file from the folder
                        File.Delete(unit.Path);
                        // Delete the loaded ressource of the file
                        switch (unit.Path.Split('.').Last())
                        {
                            case "m3d":
                                Uniray.Ressource.DeleteModel(unit.Name);
                                break;
                            case "png":
                                Uniray.Ressource.DeleteTexture(unit.Name);
                                break;
                            case "wav":
                                Uniray.Ressource.DeleteSound(unit.Name);
                                break;
                            case "cs":
                                break;
                        }
                    }
                    // Remove virtual storage unit from current folder
                    CurrentFolder.Files.Remove(unit);
                }
            }
        }

        /// <summary>Checks for new dropped files and injects them in the current folder.</summary>
        private static void InjectFiles()
        {
            if (UData.CurrentProject is not null)
            {
                // Check for new files
                string newFile = ((Container)Uniray.UI.Components["fileManager"]).GetLastFile();
                if (newFile != "")
                {
                    Console.WriteLine(newFile);
                }
                if (newFile != ((Container)Uniray.UI.Components["fileManager"]).LastFile)
                {
                    // New file detected, injecting...
                    newFile = newFile.Replace('\\', '/');
                    UFile file = new UFile(newFile);
                    CurrentFolder.AddFile(file);
                    switch (newFile.Split('.').Last())
                    {
                        case "m3d":
                            Uniray.Ressource.AddModel(LoadModel(newFile), file.Name);
                            break;
                        case "png":
                            Uniray.Ressource.AddTexture(LoadTexture(newFile), file.Name);
                            break;
                    }
                    // Reset dropped files...
                    ((Container)Uniray.UI.Components["fileManager"]).ClearFiles();
                    ((Container)Uniray.UI.Components["fileManager"]).LastFile = "";
                }
            }
        }

        private static void LoadAssets()
        {
            // Load data paths from directories
            byte[] modelPath = Encoding.UTF8.GetBytes(ModelFolder.Path);
            byte[] texturePath = Encoding.UTF8.GetBytes(TextureFolder.Path);
            byte[] soundPath = Encoding.UTF8.GetBytes(SoundFolder.Path);
            byte[] animationPath = Encoding.UTF8.GetBytes(AnimationFolder.Path);
            byte[] scriptPath = Encoding.UTF8.GetBytes(ScriptFolder.Path);
            int maxFiles = 30;
            unsafe
            {
                sbyte*[] paths = new sbyte*[5];
                fixed (byte* p = modelPath)
                {
                    sbyte* sp = (sbyte*)p;
                    paths[0] = sp;
                }
                fixed (byte* p = texturePath)
                {
                    sbyte* sp = (sbyte*)p;
                    paths[1] = sp;
                }
                fixed (byte* p = soundPath)
                {
                    sbyte* sp = (sbyte*)p;
                    paths[2] = sp;
                }
                fixed (byte* p = animationPath)
                {
                    sbyte* sp = (sbyte*)p;
                    paths[3] = sp;
                }
                fixed (byte* p = scriptPath)
                {
                    sbyte* sp = (sbyte*)p;
                    paths[4] = sp;
                }
                for (int i = 0; i < paths.Length; i++)
                {
                    FilePathList pathList = LoadDirectoryFiles(paths[i], (int*)maxFiles);
                    for (int j = 0; j < pathList.Count; j++)
                    {
                        string path = new((sbyte*)pathList.Paths[j]);
                        path = path.Replace('\\', '/');

                        // Pre-create the storage unit
                        UStorage storage;
                        if (path.Split('.').Length > 1) storage = new UFile(path);
                        else
                        {
                            // Set upstream folder for folders
                            storage = new UFolder(path, LoadFolderArchitecture(path));
                            foreach (UFolder folder in ((UFolder)storage).Files.Where(x => x is UFolder))
                            {
                                folder.UpstreamFolder = (UFolder)storage;
                            }
                        }
                        switch (i)
                        {
                            case 0:
                                ModelFolder.AddFile(storage); break;
                            case 1:
                                TextureFolder.AddFile(storage); break;
                            case 2:
                                SoundFolder.AddFile(storage); break;
                            case 3:
                                AnimationFolder.AddFile(storage); break;
                            case 4:
                                ScriptFolder.AddFile(storage); break;
                        }
                    }
                    // Set upstream folder for folders
                    switch (i)
                    {
                        case 0:
                            foreach (UFolder folder in ModelFolder.Files.Where(x => x is UFolder))
                            {
                                folder.UpstreamFolder = ModelFolder;
                            }
                            break;
                        case 1:
                            foreach (UFolder folder in TextureFolder.Files.Where(x => x is UFolder))
                            {
                                folder.UpstreamFolder = TextureFolder;
                            }
                            break;
                        case 2:
                            foreach (UFolder folder in SoundFolder.Files.Where(x => x is UFolder))
                            {
                                folder.UpstreamFolder = SoundFolder;
                            }
                            break;
                        case 3:
                            foreach (UFolder folder in AnimationFolder.Files.Where(x => x is UFolder))
                            {
                                folder.UpstreamFolder = AnimationFolder;
                            }
                            break;
                        case 4:
                            foreach (UFolder folder in ScriptFolder.Files.Where(x => x is UFolder))
                            {
                                folder.UpstreamFolder = ScriptFolder;
                            }
                            break;
                    }
                    UnloadDirectoryFiles(pathList);
                }
                Uniray.Ressource = new Ressource(
                    TextureFolder.Files,
                    SoundFolder.Files,
                    ModelFolder.Files
                    );
            }
            Console.WriteLine(Uniray.Ressource.ToString());
        }

        private static List<UStorage> LoadFolderArchitecture(string path)
        {
            List<UStorage> files = new List<UStorage>();
            // Convert to sbyte*
            sbyte* sPath;
            byte[] bytes = Encoding.UTF8.GetBytes(path);
            fixed (byte* p = bytes)
            {
                sbyte* sp = (sbyte*)p;
                sPath = sp;
            }
            // Load directory files
            FilePathList list = LoadDirectoryFiles(sPath, (int*)30);
            for (int i = 0; i < list.Count; i++)
            {
                string filePath = new((sbyte*)list.Paths[i]);
                filePath = filePath.Replace('\\', '/');

                // Pre-create the storage unit
                UStorage storage;
                if (filePath.Split('.').Length > 1) storage = new UFile(filePath);
                else
                {
                    storage = new UFolder(filePath, LoadFolderArchitecture(filePath));
                    foreach (UFolder folder in ((UFolder)storage).Files.Where(x => x is UFolder))
                    {
                        folder.UpstreamFolder = (UFolder)storage;
                    }
                }
                files.Add(storage);
            }
            UnloadDirectoryFiles(list);
            return files;
        }
    }
}