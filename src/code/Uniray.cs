// Raylib
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;
using static Raylib_cs.Rlgl;
using Raylib_cs;
// RayGUI
using static RayGUI_cs.RayGUI;
using RayGUI_cs;
// Other
using System.Numerics;
using Uniray.DatFiles;
using static Uniray.UData;
using Uniray;
using System.Reflection;
using System.IO;
using System.Xml.Linq;
using Uniray.PakFiles;
using Raylib_cs.Complements;

namespace Uniray
{
    /// <summary>Represents an instance of the Uniray application.</summary>
    public static unsafe class Uniray
    {
        // -----------------------------------------------------------
        // Constants and static instances
        // -----------------------------------------------------------
        /// <summary>Base FOV of the environment camera.</summary>
        internal const int CAMERA_FOV = 100;

        /// <summary>Current state of the program.</summary>
        internal static ProgramState State;

        public static readonly Color BACKGROUND_COLOR = new(34, 34, 34, 255);

        /// <summary>Primary color of the application.</summary>
        public static readonly Color APPLICATION_COLOR = new(41, 41, 41, 255);

        /// <summary>Special color of the application.</summary>
        public static readonly Color SPECIAL_COLOR = new(51, 51, 51, 255);

        /// <summary>Secondary color of the application.</summary>
        public static readonly Color FOCUS_COLOR = new(25, 25, 25, 255);

        /// <summary>Secondary color of the application.</summary>
        public static readonly Color TOOLBAR_COLOR = new(27, 27, 27, 255);

        /// <summary>Instance containing the ressources of the loaded project.</summary>
        public static Resource Ressource = new Resource();

        /// <summary>Global UI of the application.</summary>
        public static UI UI = new UI();

        /// <summary>Built-in shaders of the engine.</summary>
        public static UShaders Shaders = new UShaders();

        // -----------------------------------------------------------
        // Private and internal instances
        // -----------------------------------------------------------
        private static RenderTexture2D _cameraView;
        private static Rectangle _cameraViewRec;
        private static List<GameObject3D> _clipboard = new List<GameObject3D>();

        /// <summary>Initializes the <see cref="Uniray"/> engine.</summary>
        public static void Init()
        {
            // Load hard ressources
            HardRessource.Init();

            // Init the 3D conceptor
            Conceptor3D.Init();

            // Intitialize the default scene and the render camera for Uniray
            CurrentScene = new Scene("", new List<GameObject3D>(), Skybox.DEFAULT_CUBEMAP_TEXTURE);

            // Create the render texture for preview of a camera's POV
            _cameraView = LoadRenderTexture(Program.Width / 2, Program.Height / 2);
            _cameraViewRec = new Rectangle(0, 0, Program.Width / 2, -(Program.Height / 2));

            // Simulation View
            GenSimulationView();

            // Set default asset folder
            FileManager.CurrentFolder = FileManager.ModelFolder;

            //ComponentFieldValidator.Validate<UCamera>();
        }

        /// <summary>Draws the currently displayed <see cref="Scene"/>.</summary>
        public static void DrawScene()
        {
            // -----------------------------------------------------------
            // 3D graphics rendering
            // -----------------------------------------------------------

            // Render the outlined selected GameObjects and deactivate the Raylib culling to make it possible
            EnableBackfaceCulling();
            if (Selection.Count == 1)
            {
                Selection.First().DrawOutline();
            }
            DisableBackfaceCulling();

            // Draw 3-Dimensional models of the current scene and check for a hypothetical new selected object
            int index = -1;
            CurrentScene.GameObjects.ForEach(go =>
            {
                go.Draw();
                int i = Conceptor3D.CheckCollisionScreenToWorld(go);
                index = Conceptor3D.CheckDistance(index, i);
            });

            // Assign the newly selected object to the according variable
            if (index != -1)
            {
                if (IsKeyDown(KeyboardKey.LeftShift))
                {
                    Selection.Add(CurrentScene.GameObjects[index]);
                }
                else
                {
                    GameObject3D obj = CurrentScene.GameObjects[index];

                    Conceptor3D.ClearSelection();
                    Selection.Add(obj);
                    // Add UI component
                    Type type = obj.GetType();
                    PropertyInfo[] props = type.GetProperties().Where(prop => prop.IsDefined(typeof(ComponentField), false)).ToArray();
                    UI.DisplayComponents(props, obj); // Display components informations
                }
            }
            
            // Manage Game Objects selection interaction
            if (Selection.Count != 0) 
            {
                // -----------------------------------------------------------
                // Game Objects Management actions
                // -----------------------------------------------------------

                // Deletion action on the active selection of game objects
                if (IsKeyPressed(KeyboardKey.Delete))
                {
                    foreach (GameObject3D go in Selection)
                    {
                        CurrentScene.GameObjects.Remove(go);
                        Conceptor3D.ClearSelection();
                        break;
                    }
                }
                // Escape action on the active selection of game objects
                if (IsKeyPressed(KeyboardKey.Escape))
                {
                    Conceptor3D.ClearSelection();
                }
                // Copy action on the active selection of game objects
                if (IsKeyDown(KeyboardKey.LeftControl) && IsKeyPressed(KeyboardKey.C))
                {
                    _clipboard = Selection;
                }
                // Paste action on the active selection of game objects
                if (IsKeyDown(KeyboardKey.LeftControl) && IsKeyPressed(KeyboardKey.V))
                {
                    Vector2 mouse = GetMousePosition();
                    if (mouse.X > UI.Components["gameManager"].X + UI.Components["gameManager"].Width && mouse.Y < UI.Components["fileManager"].Y - 10)
                    {
                        Vector3 dir = GetCameraForward(ref Conceptor3D.EnvCamera) * 5f;

                        _clipboard.Where(x => x is IClonable<GameObject3D>).ToList().ForEach(go =>
                        {
                            var clone = ((IClonable<GameObject3D>)go).Clone();
                            clone.Position = Conceptor3D.EnvCamera.Position + dir;
                            CurrentScene.GameObjects.Add(clone);
                        });

                        Conceptor3D.ClearSelection();
                    }   
                }

                // -----------------------------------------------------------
                // Game Objects transformative actions
                // -----------------------------------------------------------

                // Translate the currently selected object, indpendently of its type
                if (IsKeyDown(KeyboardKey.G))
                {
                    // Translate a neutral vector for selection uniformity
                    Vector3 newPos = Conceptor3D.TranslateObject(Vector3.Zero);
                    foreach (GameObject3D go in Selection)
                    {
                        // Add the position of the current object
                        Vector3 finalPos = Vector3Add(newPos, go.Position);
                        // Call individual properties to trigger UI Components
                        go.Position = finalPos;
                    }
                }
                // Rotate the currently selected game object
                else if (IsKeyDown(KeyboardKey.R))
                {
                    Selection.Where(x => x is IRotation).ToList().ForEach(go => Conceptor3D.RotateObject((IRotation)go));
                    Selection.Where(x => x is UCamera).ToList().ForEach(cam => Conceptor3D.RotateCamera((UCamera)cam));
                    HideCursor();
                }
                else ShowCursor();
            }

            // -----------------------------------------------------------
            // 3D graphics rendering for camera preview
            // -----------------------------------------------------------
            if (Selection.Count == 1 && Selection.First() is UCamera cam)
            {
                EndMode3D();

                BeginTextureMode(_cameraView);

                ClearBackground(new Color(70, 70, 70, 255));

                BeginMode3D(cam.Camera);

                // Draw the external skybox 
                RaylibComplements.DrawSkybox(CurrentScene.Skybox);

                Shaders.Update(cam.Camera.Position);

                Conceptor3D.DrawGrid();
                // Draw Models
                foreach (GameObject3D go in CurrentScene.GameObjects.Where(x => x is UModel || x is UShape)) go.Draw();

                EndMode3D();
                EndTextureMode();

                BeginMode3D(Conceptor3D.EnvCamera);
            }
            // Update shaders
            Shaders.Update(Conceptor3D.EnvCamera.Position);
        }

        /// <summary>Draws 2-Dimensional user interface.</summary>
        public static void DrawUI()
        {
            // -----------------------------------------------------------
            // 2D graphics rendering (User Interface)
            // -----------------------------------------------------------

            // Check if the window has been resized to adjust the size of the UI
            if (IsWindowResized())
            {
                Resize();
            }
            // Draw the outline rectangles that appear behind the main panels
            DrawRectangle(0, 0, (int)(Program.Width - Program.Width / 1.25f), Program.Height, BACKGROUND_COLOR);
            DrawRectangle(0, 0, Program.Width, 30, TOOLBAR_COLOR);
            DrawRectangle(0, Program.Height - Program.Height / 3 - 10, Program.Width, Program.Height - (Program.Height - Program.Height / 3) + 10, BACKGROUND_COLOR);

            // Draw the entire UI and handle the events related to it
            UI.Draw();

            // Tick the error handler for the errors to potentially disappear
            ErrorHandler.Tick();

            // Draw and update file manager
            FileManager.Draw();

            // Render the selected camera POV to the top right corner of the screen
            if (Selection.Count == 1 && Selection.First() is UCamera cam && UData.GameSimulation is null)
            {
                DrawRectangleLinesEx(new Rectangle(Program.Width - Program.Width / 5 - 11, 49, Program.Width / 5 + 2, Program.Height / 5 + 2), 2, Color.White);
                DrawTexturePro(_cameraView.Texture, _cameraViewRec, new Rectangle(Program.Width - Program.Width / 5 - 10, 50, Program.Width / 5, Program.Height / 5), Vector2.Zero, 0, Color.White);
            }
            if (UData.GameSimulation is not null)
            {
                DrawTexturePro(GameSimulation.GameSimView.Texture, GameSimulation.SourceRectangle, GameSimulation.DestinationRectangle, Vector2.Zero, 0, Color.White);
            }

            // Draw game debugger if displayed
            if (UData.GameSimulation is not null)
            {
                GameDebugger.Update();
            }

            // Draw the currently displayed modal and define its state
            if (CurrentModal is not null)
            {
                // If the user closed the modal without proceeding
                if (LastModalExitCode == 0)
                {
                    CurrentModal = null;
                    LastModalExitCode = null;
                }
                // If the user closed the modal by proceeding
                else if (LastModalExitCode == 1)
                {
                    switch (CurrentModal)
                    {
                        // Open project modal
                        case "openProjectModal":
                            LoadProject(((Textbox)UI.Modals[CurrentModal].Components["openProjectTextbox"]).Text);
                            CurrentModal = null;
                            LastModalExitCode = null;
                            break;
                        // Create project modal
                        case "newProjectModal":
                            string path = ((Textbox)UI.Modals[CurrentModal].Components["newProjectTextbox1"]).Text;
                            string name = ((Textbox)UI.Modals[CurrentModal].Components["newProjectTextbox2"]).Text;
                            CreateProject(path, name);
                            CurrentModal = null;
                            LastModalExitCode = null;
                            break;
                    }
                }
            }
        }

        /// <summary>Loads project from .uproj file.</summary>
        /// <param name="path">path to the .uproj file</param>
        public static void LoadProject(string path)
        {
            try
            {
                path = path.Replace('\\', '/'); // Linux parsing
                // Read project informations from .uproj
                FileStream stream = new FileStream(path, FileMode.Open);
                using BinaryReader reader = new BinaryReader(stream);
                string _projectName = reader.ReadString();
                byte[] _aesKey = reader.ReadBytes(DatEncoder.AES_KEY_LENGTH);
                byte[] _aesIv = reader.ReadBytes(DatEncoder.AES_IV_LENGTH);
                string _launchScene = reader.ReadString();

                // Set Project info in application
                string? directory = Path.GetDirectoryName(path);

                // Load assets from the given project's assets folder
                if (directory is not null)
                {
                    FileManager.Init(directory);
                    directory = directory.Replace('\\', '/'); // Linux parsing
                }
                else throw new Exception($"Could not load the assets of project {path}");

                // Change to default assets page of the manager container
                ((Container)UI.Components["fileManager"]).ExtensionFile = "m3d";
                ((Container)UI.Components["fileManager"]).Tag = "models";
                ((Container)UI.Components["fileManager"]).OutputFilePath = directory + "/assets/models";
                ((Label)UI.Components["ressourceInfoLabel"]).Text = "File type : .m3d";

                // Load scenes along with their game objects
                CurrentProject = new Project(_projectName, path, LoadProjectScenes(directory, _aesKey, _aesIv), _aesKey, _aesIv);
                CurrentProject.LaunchScene = _launchScene;
                CurrentScene = CurrentProject.GetScene(0); // Set default scene
                ((Label)UI.Components["projectNameLabel"]).Text = "Uniray - " + _projectName;

                TraceLog(TraceLogLevel.Info, "Project has been loaded successfully !");
            }
            catch
            {
                ErrorHandler.Send(new Error(2, "Project could not be loaded !"));
            }
        }

        /// <summary>Saves the currently loaded project.</summary>
        public static void SaveProject()
        {
            if (CurrentProject is not null)
            {
                foreach (Scene scene in CurrentProject.Scenes)
                {
                    DatEncoder.EncodeScene(scene); // Encode project scenes
                }

                FileStream stream = new FileStream(CurrentProject.ProjectFile, FileMode.Create);
                using BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(CurrentProject.Name); // Write project name
                writer.Write(CurrentProject.EncryptionKey); // Write project key
                writer.Write(CurrentProject.SymmetricalVector); // Write project symmetrical vector
                writer.Write(CurrentProject.LaunchScene); // Write default launch scene

                TraceLog(TraceLogLevel.Info, "Project was saved successfully !");
            }
            else
            {
                // Open modal for the user to create a new project
                CurrentModal = "newProjectModal";
            }
        }

        /// <summary>Creates a new Uniray project.</summary>
        /// <param name="path">Path to the target directory.</param>
        public static void CreateProject(string path, string name)
        {
            try
            {
                // Create directories (assets, locs, etc.)
                Directory.CreateDirectory(path + "/assets");

                Directory.CreateDirectory(path + "/assets/animations");
                Directory.CreateDirectory(path + "/assets/models");
                Directory.CreateDirectory(path + "/assets/scripts");
                Directory.CreateDirectory(path + "/assets/sounds");
                Directory.CreateDirectory(path + "/assets/textures");

                Directory.CreateDirectory(path + "/scenes");

                // Unzip default Visual Studio project from internal data
                System.IO.Compression.ZipFile.ExtractToDirectory("data/default_VS_Project.zip", path);

                // Create project 32 bytes encryption key
                byte[] _aesKey = new byte[DatEncoder.AES_KEY_LENGTH];
                for (int i = 0; i < _aesKey.Length; i++) Random.Shared.NextBytes(_aesKey);
                // Create project 16 byte symmetrical vector (for encryption)
                byte[] _aesIv = new byte[DatEncoder.AES_IV_LENGTH];
                for (int i = 0; i < _aesIv.Length; i++) Random.Shared.NextBytes(_aesIv);

                // Create .uproj file
                FileStream stream = new FileStream($"{path}/{name}.uproj", FileMode.Create);
                using BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(name); // Write project name
                writer.Write(_aesKey); // Write project key
                writer.Write(_aesIv); // Write project symmetrical vector
                writer.Write("PLACEHOLDER"); // Write default launch scene

                // Set file manager output to new project directory
                ((Container)UI.Components["fileManager"]).OutputFilePath = path + "/assets/models/";
                // Set new window title
                ((Label)UI.Components["projectNameLabel"]).Text = "Uniray - " + name;

                // Create new empty scene (except camera)
                UCamera ucamera = new UCamera("Default camera", UCamera.DefaultCamera);
                Scene defaultScene = new Scene("PLACEHOLDER", new List<GameObject3D> { ucamera }, Skybox.DEFAULT_CUBEMAP_TEXTURE);
                List<Scene> scenes = new() { defaultScene };
                CurrentProject = new Project(name, path + "/" + name + ".uproj", scenes, _aesKey, _aesIv); // Create project

                Ressource = new Resource(); // Init ressource lists
                CurrentScene = CurrentProject.GetScene(0); // Set current scene to default
                DatEncoder.EncodeScene(CurrentScene); // Encode newly created scene to .DAT files

                TraceLog(TraceLogLevel.Info, "Project \"" + name + "\" has been created");
            }
            catch
            {
                ErrorHandler.Send(new Error(1, "Project " + name + " could not be created"));
            }
        }

        /// <summary>Builds the project to run outside of Uniray.</summary>
        public static void PublishProject()
        {
            if (CurrentProject is not null)
            {
                
                string command = $"/C cd \"{CurrentProject.ProjectFolder}\" && dotnet publish -c Release -r win-x64 --self-contained -o publish uniray_Project.csproj";
                System.Diagnostics.Process.Start("CMD.exe", command); // Publish dotnet project
                while (!Directory.Exists($"{CurrentProject.ProjectFolder}/publish")) { } // Wait till publish completes
                // Create configuration file
                FileStream stream = new FileStream($"{CurrentProject.ProjectFolder}/publish/project.conf", FileMode.Create);
                using BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(CurrentProject.Name); // Write project name
                writer.Write(CurrentProject.EncryptionKey); // Write project key
                writer.Write(CurrentProject.SymmetricalVector); // Write project symmetrical vector
                writer.Write(CurrentScene.Name); // Write default launch scene
                // Copy raylib dlls
                File.Copy("data/libraries/Raylib/raylib.dll", $"{CurrentProject.ProjectFolder}/publish/raylib.dll", true);
                File.Copy("data/libraries/Raylib/Raylib-cs.dll", $"{CurrentProject.ProjectFolder}/publish/Raylib-cs.dll", true);
                // Create resource files
                Directory.CreateDirectory($"{CurrentProject.ProjectFolder}/publish/res");
                PakArchive.CreatePakFile(FileManager.ModelFolder, $"{CurrentProject.ProjectFolder}/publish/res/MODELS.pak");
                PakArchive.CreatePakFile(FileManager.TextureFolder, $"{CurrentProject.ProjectFolder}/publish/res/TEXTURES.pak");
                PakArchive.CreatePakFile(FileManager.SoundFolder, $"{CurrentProject.ProjectFolder}/publish/res/SOUNDS.pak");
            }
            else
            {
                ErrorHandler.Send(new Error(4, "No project loaded, unable to publish"));
            }
        }

        /// <summary>Loads the scenes from a given directory.</summary>
        /// <param name="directory">Directory to retrives scenes from.</param>
        /// <returns>The list of scenes contained in the given directory.</returns>
        private static List<Scene> LoadProjectScenes(string directory, byte[] key, byte[] iv)
        {
            string[] scenesPath = Directory.GetFiles(directory + "/scenes"); // Gets all .DAT files paths
            List<Scene> scenes = new List<Scene>();
            for (int i = 0; i < scenesPath.Length; i++)
            {   
                Scene scene = DatEncoder.DecodeScene(scenesPath[i], key, iv);
                // Prepare the models
                foreach (UModel model in scene.GameObjects.Where(x => x is UModel))
                {
                    if (model.ModelID != "")
                    {
                        model.LoadMeshes();
                        model.LoadMaterials();
                    }
                }
                scenes.Add(scene);
            }
            return scenes;
        }

        /// <summary>Resizes the UI of the application.</summary>
        public static void Resize()
        {
            Program.Width = GetScreenWidth();
            Program.Height = GetScreenHeight();
            UI = new UI();
            GenSimulationView();

            // Project tool bar info
            string txtInfo;
            if (CurrentProject is null) txtInfo = "Uniray - Empty";
            else txtInfo = $"Uniray - {CurrentProject.Name}";
            ((Label)UI.Components["projectNameLabel"]).Text = txtInfo;

            // Re-Init error handler
            ErrorHandler.Init(RayGUI.Font, new Vector2(UI.Components["fileManager"].X + UI.Components["fileManager"].Width / 2 - 150, UI.Components["fileManager"].Y - 60));

        }

        /// <summary>Generates the simulation view for game simulation.</summary>
        private static void GenSimulationView()
        {
            // Unload if loaded
            if (IsRenderTextureReady(GameSimulation.GameSimView)) UnloadRenderTexture(GameSimulation.GameSimView);
            // Game simulation view
            GameSimulation.SourceRectangle = new Rectangle(0, 0, Program.Width, -Program.Height);
            int renderHeight = Program.Height - (UI.Components["fileManager"].Height + 20);
            float renderRatio = (float)Program.Height / renderHeight;
            float renderWidth = Program.Width / (float)renderRatio;
            GameSimulation.GameSimView = LoadRenderTexture(Program.Width, Program.Height);
            GameSimulation.DestinationRectangle = new Rectangle(UI.Components["gameManager"].Width + 20, 33,
                renderWidth,
                renderHeight - 33);
        }

        /// <summary>Starts the game simulation.</summary>
        public static void StartGameSimulation()
        {
            if (CurrentProject is not null)
            {
                UData.GameSimulation = new GameSimulation(); // Start game simulation
                GameDebugger.Init(RayGUI.Font, new Vector2(GameSimulation.DestinationRectangle.X + GameSimulation.DestinationRectangle.Width, 35)); // Init game debugger
            }
            else
            {
                ErrorHandler.Send(new Error(3, "No project loaded, unabled to start simulation"));
            }
        }

        /// <summary>Stops the game simulation.</summary>
        public static void StopGameSimulation()
        {
            GameDebugger.Close();
            UData.GameSimulation = null;
        }

        /// <summary>Runs the current project's simulation.</summary>
        public static void RunGameSimulation()
        {
            // Draw default terrain
            Conceptor3D.DrawGrid();
            // Draw scene game objects
            CurrentScene.GameObjects.Where(go => go is not UCamera).ToList().ForEach(go => go.Draw());
        }
    }
}

namespace Raylib_cs.Complements
{
    /// <summary>Represents an instance of the extension class of <see cref="Raylib"/>.</summary>
    public static class RaylibComplements
    {
        // Public fields
        public static double LastTimeButtonPressed;
        public static bool FirstLoopEntry = true;

        /// <summary>Checks if a mouse button has been double clicked.</summary>
        /// <param name="button">Mouse button to check.</param>
        /// <returns></returns>
        public static bool IsMouseButtonDoubleClicked(MouseButton button)
        {
            double currentFrame = GetTime();

            if (IsMouseButtonPressed(button))
            {
                if (FirstLoopEntry && (currentFrame - LastTimeButtonPressed) >= 0.25)
                {
                    LastTimeButtonPressed = currentFrame;
                    FirstLoopEntry = false;
                }
                if ((currentFrame - LastTimeButtonPressed) <= 0.25 && (currentFrame - LastTimeButtonPressed) >= 0.05)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        /// <summary>Loads a skybox object and configures its attributes.</summary>
        /// <param name="path">Path to .hdr skybox texture.</param>
        /// <param name="facing">Direction the skybox is facing (North by default).</param>
        /// <returns>The configured skybox object.</returns>
        public static Skybox LoadSkybox(string path, EnvDirection facing)
        {
            Skybox skybox = new Skybox();
            skybox.CubemapMaterial = LoadMaterialDefault();
            skybox.CubemapMaterial.Shader = Uniray.Uniray.Shaders.SkyboxShader;

            Texture2D panorama = LoadTexture(path);
            SetSkyboxTexture(skybox, panorama);
            UnloadTexture(panorama);

            return skybox;
        }

        /// <summary>Draws a skybox object to the screen.</summary>
        /// <param name="skybox">Skybox object to display.</param>
        public static void DrawSkybox(Skybox skybox)
        {
            DisableBackfaceCulling();
            DisableDepthMask();
            DrawMesh(Skybox.Mesh, skybox.CubemapMaterial, MatrixIdentity());
            EnableBackfaceCulling();
            EnableDepthMask();
        }

        /// <summary>Modifies a skybox object's cubemap texture.</summary>
        /// <param name="skybox">Skybox to modify</param>
        /// <param name="texture">Texture to apply. (NON-CUBEMAP)</param>
        public static void SetSkyboxTexture(Skybox skybox, Texture2D texture)
        {
            Texture2D cubemap = Uniray.Uniray.Shaders.GenTexureCubemap(texture, 256, PixelFormat.UncompressedR8G8B8A8);
            SetMaterialTexture(ref skybox.CubemapMaterial, MaterialMapIndex.Cubemap, cubemap);
        }

        /// <summary>Unloads a <see cref="UModel"/>'s ressources from the vRAM.</summary>
        /// <param name="model"></param>
        public static void UnloadUModel(UModel model)
        {
            // Unload meshes
            for (int i = 0; i < model.MeshCount; i++) 
            {
                UnloadMesh(model.Meshes[i]);
            }
            // Unload materials
            for (int i = 0; i < model.Materials.Length; i++) 
            { 
                UnloadMaterial(model.Materials[i]);
            }
        }
    }
}