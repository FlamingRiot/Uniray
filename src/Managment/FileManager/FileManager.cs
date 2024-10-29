﻿using RayGUI_cs;
using static RayGUI_cs.RayGUI;
using Raylib_cs;
using System.Text;
using static Raylib_cs.Raylib;
using Raylib_cs.Complements;
using System.Numerics;

namespace Uniray
{
    /// <summary>Represents a <see cref="FileManager"/> instance.</summary>
    internal static unsafe class FileManager
    {
        public static UFolder ModelFolder = new UFolder();
        public static UFolder TextureFolder = new UFolder();
        public static UFolder SoundFolder = new UFolder();
        public static UFolder AnimationFolder = new UFolder();
        public static UFolder ScriptFolder = new UFolder();

        /// <summary>The currently displayed <see cref="UFolder"/>.</summary>
        internal static UFolder CurrentFolder = new UFolder();

        /// <summary>The currently clicked <see cref="UStorage"/> unit.</summary>
        internal static List<UStorage> ClickedUnits = new List<UStorage>();

        private static bool ClickOnSelection;

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

            // Future static position
            Vector2 staticSelectedPos = Vector2.Zero;

            List<UStorage> _units = CurrentFolder.Files;

            ClickOnSelection = true;

            // Test if clicked on selection
            for (int i = 0; i < _units.Count; i++) 
            {
                // Define file row
                short row = (short)(i / 10);
                // Define drawing position
                int xPos = Uniray.UI.Components["fileManager"].X + 150 * (i % 10 + 1) - 100;
                int yPos = Uniray.UI.Components["fileManager"].Y + 60 + row * 120;

                if (IsMouseButtonPressed(MouseButton.Left))
                {
                    if (!ClickedUnits.Contains(_units[i]) && Hover(xPos, yPos, 65, 65))
                    {
                        ClickOnSelection = false;
                    }
                }
            }

            // Loop over files
            for (int i = 0; i < _units.Count; i++) 
            {
                // Define file row
                short row = (short)(i / 10);
                // Define drawing position
                int xPos = Uniray.UI.Components["fileManager"].X + 150 * (i % 10 + 1) - 100;
                int yPos = Uniray.UI.Components["fileManager"].Y + 60 + row * 120;

                DrawStorageUnit(_units[i], xPos, yPos, 255);
                UpdateStorageUnit(_units[i], xPos, yPos);

                
                // Get selected item position
                if (UData.SelectedFiles.Count != 0)
                {
                    if (_units[i] == UData.SelectedFiles[0])
                    {
                        staticSelectedPos = new Vector2(xPos, yPos);
                    }
                }
            }

            // DON'T YOU DARE TOUCH THE CODE BELOW YOU PIECE OF SHIT!!!
            RaylibComplements.FirstLoopEntry = true;
            if (RaylibComplements.IsMouseButtonDoubleClicked(MouseButton.Left, "Program.cs"))
            {
                RaylibComplements.LastTimeButtonPressed = 0.0;
            }
            // now you can

            // Creates folder action
            if (IsKeyDown(KeyboardKey.LeftControl) && IsKeyDown(KeyboardKey.LeftShift) && IsKeyPressed(KeyboardKey.N))
            {
                UFolder folder = new UFolder(CurrentFolder.Path + "/new", new List<UStorage>());
                folder.UpstreamFolder = CurrentFolder;
                CurrentFolder.Files.Add(folder);
                Directory.CreateDirectory(CurrentFolder.Path + "/new");

                // Set clicked item to new folder
                ClickedUnits.Clear();
                ClickedUnits.Add(folder);
            }

            // Draw selected item
            Vector2 mouse = GetMousePosition();
            if (UData.SelectedFiles.Count != 0 && !Hover((int)staticSelectedPos.X, (int)staticSelectedPos.Y, 65, 65)) 
            {
                for (int i = 0; i < UData.SelectedFiles.Count; i++)
                {
                    DrawStorageUnit(UData.SelectedFiles[i], (int)mouse.X + i * 75, (int)mouse.Y, 122);
                }
            }

            // Update dropping action on selected item
            DropStorageUnit();
        }

        /// <summary>Draws a single storage unit to the file manager, according to its place in the list.</summary>
        /// <param name="unit">Storage unit to draw.</param>
        /// <param name="index">Index of the storage unit.</param>
        private static void DrawStorageUnit(UStorage unit, int x, int y, int alpha)
        {
            // Shorten the text
            string lbl;
            if (unit.Name.Length >= 10) { lbl = unit.Name.Remove(5) + "..."; }
            else lbl = unit.Name;

            // Draw the appropriate element
            if (unit is UFile file)
            {
                // Draw selection aura if selected
                if (ClickedUnits.Contains(unit))
                {
                    DrawRectangle(x - 5, y - 5, 75, 75, new Color(78, 211, 237, 75));
                }
                // Draw item texture and label
                switch (((Container)Uniray.UI.Components["fileManager"]).ExtensionFile)
                {
                    case "m3d":
                        DrawTexture(HardRessource.Textures["model_file"], x, y, new Color(255, 255, 255, alpha));
                        DrawLabel(new Label(x + 10 - lbl.Length / 2, y + HardRessource.Textures["model_file"].Height + 20, lbl));
                        break;
                    case "png":
                        // Prepare preview texture
                        Texture2D previewTexture = Uniray.Ressource.GetTexture((file).Name);
                        Rectangle srcRectangle = new Rectangle(0, 0, previewTexture.Width, previewTexture.Height);
                        Rectangle destRectangle = new Rectangle(x, y, 65, 65);

                        DrawTexturePro(previewTexture, srcRectangle, destRectangle, Vector2.Zero, 0, new Color(255, 255, 255, alpha));
                        DrawLabel(new Label(x + 10 - lbl.Length / 2, y + HardRessource.Textures["model_file"].Height + 20, lbl));
                        break;
                }
            }
            else if (unit is UFolder)
            {
                // Draw selection aura if selected
                if (ClickedUnits.Contains(unit))
                {
                    DrawRectangle(x - 5, y - 5, HardRessource.Textures["folder"].Width + 10, HardRessource.Textures["folder"].Height + 10, new Color(78, 211, 237, 75));
                }

                DrawTexture(HardRessource.Textures["folder"], x, y, new Color(255, 255, 255, alpha));
                DrawLabel(new Label(x + 10 + unit.Name.Length / 3, y + HardRessource.Textures["model_file"].Height + 20, lbl));
            }
        }

        private static void UpdateStorageUnit(UStorage unit, int x, int y)
        {
            // Delete action
            if (IsMouseButtonPressed(MouseButton.Middle))
            {
                if (Hover(x, y, HardRessource.Textures["model_file"].Width, HardRessource.Textures["model_file"].Height))
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

            // Selection action
            if (IsMouseButtonPressed(MouseButton.Left)) 
            {
                if (Hover(x, y, HardRessource.Textures["model_file"].Width, HardRessource.Textures["model_file"].Height))
                {
                    // Test if clicked on selection


                    if (IsKeyUp(KeyboardKey.LeftControl) && !ClickOnSelection) ClickedUnits.Clear();
                    if (!ClickedUnits.Contains(unit)) ClickedUnits.Add(unit);
                }
            }
         
            DrawText(ClickedUnits.Count.ToString(), 0, 0, 50, Color.Red);
            DrawText(UData.SelectedFiles.Count.ToString(), 0, 50, 50, Color.Blue);

            // Drag action
            if (IsMouseButtonDown(MouseButton.Left))
            {
                if (Hover(x, y, HardRessource.Textures["model_file"].Width, HardRessource.Textures["model_file"].Height))
                {
                    if (!UData.SelectedFiles.Contains(unit))
                    {
                        UData.Selection.Clear();
                        foreach (UStorage file in ClickedUnits)
                        {
                            if (UData.SelectedFiles.Count == 0 || ClickedUnits.Count > 1) UData.SelectedFiles.Add(file);
                        }
                    }
                    SetMouseCursor(MouseCursor.PointingHand);
                }
            }

            // Folder entering action
            if (RaylibComplements.IsMouseButtonDoubleClicked(MouseButton.Left, "FileManager"))
            {
                if (Hover(x, y, HardRessource.Textures["model_file"].Width, HardRessource.Textures["model_file"].Height))
                {
                    if (unit is UFolder folder)
                    {
                        // Set the new selected folder
                        CurrentFolder = folder;
                        ((Container)Uniray.UI.Components["fileManager"]).OutputFilePath += '/' + unit.Path.Split('/').Last();

                        // Reset selected item
                        UData.SelectedFiles.Clear();
                        ClickedUnits.Clear();
                    }
                }
            }
        }

        private static void DropStorageUnit()
        {
            if (IsMouseButtonReleased(MouseButton.Left))
            {
                if (UData.SelectedFiles.Count != 0)
                {
                    // Get mouse position
                    Vector2 mouse = GetMousePosition();
                    // Check if in 3D conceptor
                    if (mouse.X > Uniray.UI.Components["gameManager"].Width + 20 && mouse.Y < Uniray.UI.Components["fileManager"].Y - 10 && UData.SelectedFiles.Count == 1)
                    {
                        // Check if model
                        if (UData.SelectedFiles[0].Path.Split('.').Last() == "m3d")
                        {
                            string modelKey = UData.SelectedFiles[0].Name;
                            // Import model into the scene
                            if (Uniray.Ressource.ModelExists(modelKey))
                            {
                                UData.CurrentScene.AddGameObject(new UModel(
                                    "[New model]",
                                    Uniray.EnvCamera.Position + GetCameraForward(ref Uniray.EnvCamera) * 5,
                                    Uniray.Ressource.GetModel(modelKey).Meshes[0],
                                    modelKey));
                            }
                            else
                            {
                                // Reset materials
                                Model m = LoadModel(UData.SelectedFiles[0].Path);
                                for (int j = 0; j < m.Meshes[0].VertexCount * 4; j++)
                                    m.Meshes[0].Colors[j] = 255;
                                UpdateMeshBuffer(m.Meshes[0], 3, m.Meshes[0].Colors, m.Meshes[0].VertexCount * 4, 0);
                                // Add model
                                Uniray.Ressource.AddModel(m, modelKey);
                                UData.CurrentScene.AddGameObject(new UModel(
                                    "[New model]",
                                    Uniray.EnvCamera.Position + GetCameraForward(ref Uniray.EnvCamera) * 5,
                                    Uniray.Ressource.GetModel(modelKey).Meshes[0],
                                    modelKey));
                            }
                        }
                    }
                    // Check if component field
                    Component box = Uniray.UI.Components["textureTextbox"];
                    if (Hover(box.X, box.Y, box.Width, box.Height))
                    {
                        // Check if texture
                        if (UData.SelectedFiles[0].Path.Split('.').Last() == "png") 
                        {
                            if (UData.Selection.Count == 1 && UData.Selection.First() is UModel)
                            {
                                ((UModel)UData.Selection.First()).SetTexture(UData.SelectedFiles[0].Name, Uniray.Ressource.GetTexture(UData.SelectedFiles[0].Name));
                            }
                        }
                    }
                    // Check if over folder
                    foreach (UFolder folder in CurrentFolder.Files.Where(x => x is UFolder))
                    {
                        // Define index
                        int index = CurrentFolder.Files.IndexOf(folder);
                        // Define file row
                        short row = (short)(index / 10);
                        // Define drawing position
                        int xPos = Uniray.UI.Components["fileManager"].X + 150 * (index % 10 + 1) - 100;
                        int yPos = Uniray.UI.Components["fileManager"].Y + 60 + row * 120;

                        if (Hover(xPos, yPos, 65, 65))
                        {
                            // Move files
                            foreach (UFile file in UData.SelectedFiles.Where(x => x is UFile))
                            {
                                // Move virtual file
                                folder.AddFile(file);
                                CurrentFolder.DeleteFile(file);
                                File.Move(file.Path, folder.Path + '/' + file.FullName);
                                file.Path = folder.Path + '/' + file.FullName;
                            }

                            // Move folder
                            // To be continued...

                            break;
                        }
                    }
                    // Clear file selection
                    UData.SelectedFiles.Clear();
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
                        /*
                        case "m3d":
                            Uniray.Ressource.AddModel(LoadModel(newFile), file.Name);
                            break;*/
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
                                folder.AddFile(folder.UpstreamFolder);
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
                        folder.AddFile(folder.UpstreamFolder);
                    }
                }
                files.Add(storage);
            }
            UnloadDirectoryFiles(list);
            return files;
        }

        /// <summary>Goes up one folder from the current one</summary>
        public static void BackFolder()
        {
            if (CurrentFolder.UpstreamFolder is not null)
            {
                CurrentFolder = CurrentFolder.UpstreamFolder;

                string? path = ((Container)Uniray.UI.Components["fileManager"]).OutputFilePath;
                int lastSlashIndex = path.LastIndexOf('/');
                path = path.Substring(0, lastSlashIndex);

                ((Container)Uniray.UI.Components["fileManager"]).OutputFilePath = path;
            }
        }
    }
}