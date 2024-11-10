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
using Newtonsoft.Json;
using Uniray.DatFiles;
using static Uniray.UData;

namespace Uniray
{
    /// <summary>Represents an instance of the Uniray application.</summary>
    public static unsafe class Uniray
    { 
        // -----------------------------------------------------------
        // Constants and static instances
        // -----------------------------------------------------------
        /// <summary>Base FOV of the environment camera.</summary>
        internal const int CAMERA_FOV = 90;

        /// <summary>Current state of the program.</summary>
        internal static ProgramState State;

        public static readonly Color BACKGROUND_COLOR = new(34, 34, 34, 255);

        /// <summary>Primary color of the application.</summary>
        public static readonly Color APPLICATION_COLOR = new(41, 41, 41, 255);

        /// <summary>Secondary color of the application.</summary>
        public static readonly Color FOCUS_COLOR = new(25, 25, 25, 255);

        /// <summary>Instance containing the ressources of the loaded project.</summary>
        public static Ressource Ressource = new Ressource();

        /// <summary>Global UI of the application.</summary>
        public static UI UI = new UI();

        /// <summary>Built-in shaders of the engine.</summary>
        public static UShaders Shaders = new UShaders();

        /// <summary>Defines whether a project is running or not.</summary>
        public static bool RunningProject = false;

        /// <summary>Render texture of the game simulation.</summary>
        public static RenderTexture2D GameSimView;

        /// <summary>Render rectangle of the game simulation.</summary>
        public static Rectangle GameSimDestRectangle;

        // -----------------------------------------------------------
        // Private and internal instances
        // -----------------------------------------------------------
        private static RenderTexture2D _cameraView;
        private static Rectangle _cameraViewRec;
        private static Rectangle _gameSimSourceRec;
        private static List<GameObject3D> _clipboard = new List<GameObject3D>();

        /// <summary>Initializes the <see cref="Uniray"/> engine.</summary>
        public static void Init()
        {
            // Intitialize the Uniray shaders
            Shaders = new UShaders();
            Texture2D panorama = LoadTexture("data/shaders/skyboxes/industrial.hdr");
            Texture2D cubemap = Shaders.GenTexureCubemap(panorama, 256, PixelFormat.UncompressedR8G8B8A8);
            Shaders.SetCubemap(cubemap);

            // Unload useless texture
            UnloadTexture(panorama);

            // Load hard ressources
            HardRessource.Init();

            // Init error handler
            ErrorHandler.Init(RayGUI.Font, new Vector2(UI.Components["fileManager"].X + UI.Components["fileManager"].Width / 2 - 150, UI.Components["fileManager"].Y - 60));

            // Init the 3D conceptor
            Conceptor3D.Init();

            // Intitialize the default scene and the render camera for Uniray
            CurrentScene = new Scene("", new List<GameObject3D>());

            // Create the render texture for preview of a camera's POV
            _cameraView = LoadRenderTexture(Program.Width / 2, Program.Height / 2);
            _cameraViewRec = new Rectangle(0, 0, Program.Width / 2, -(Program.Height / 2));

            // Simulation View
            GenSimulationView();

            // Set default asset folder
            FileManager.CurrentFolder = FileManager.ModelFolder;
        }

        /// <summary>Draws the currently displayed <see cref="Scene"/>.</summary>
        public static void DrawScene()
        {
            // -----------------------------------------------------------
            // 3D graphics rendering
            // -----------------------------------------------------------

            // Define a mouse ray for collision check
            Conceptor3D.UpdateMouseRay();

            // Render the outlined selected GameObjects and deactivate the Raylib culling to make it possible
            SetCullFace(ZERO);
            foreach (GameObject3D go in Selection)
            {
                switch (go)
                {
                    case UModel model:
                        DrawMesh(model.Mesh, Shaders.OutlineMaterial, model.Transform);
                        break;
                    case UCamera camera:
                        DrawMesh(HardRessource.Models["camera"].Meshes[0], Shaders.OutlineMaterial, camera.Transform);
                        break;
                }
            }
            SetCullFace(ONE);

            // Draw 3-Dimensional models of the current scene and check for a hypothetical new selected object
            int index = -1;
            foreach (GameObject3D go in CurrentScene.GameObjects)
            {
                // Manage objects drawing + object selection (according to the object type)
                if (go is UModel)
                {
                    DrawMesh(((UModel)go).Mesh, ((UModel)go).Material, ((UModel)go).Transform);
                    int i = Conceptor3D.CheckCollisionScreenToWorld(go, ((UModel)go).Mesh, ((UModel)go).Transform);
                    index = Conceptor3D.CheckDistance(index, i);
                }
                else if (go is UCamera)
                {
                    DrawMesh(HardRessource.Models["camera"].Meshes[0], HardRessource.Materials["camera"], ((UCamera)go).Transform);
                    int i = Conceptor3D.CheckCollisionScreenToWorld(go, HardRessource.Models["camera"].Meshes[0], ((UCamera)go).Transform);
                    index = Conceptor3D.CheckDistance(index, i);
                }
            }

            // Assign the newly selected object to the according variable
            if (index != -1)
            {
                if (IsKeyDown(KeyboardKey.LeftShift))
                {
                    Selection.Add(CurrentScene.GameObjects[index]);
                }
                else
                {
                    Selection.Clear();
                    Selection.Add(CurrentScene.GameObjects[index]);
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
                        Selection.Clear();
                        break;
                    }
                }
                // Escape action on the active selection of game objects
                if (IsKeyPressed(KeyboardKey.Escape))
                {
                    Selection.Clear();
                }
                // Copy action on the active selection of game objects
                if (IsKeyDown(KeyboardKey.LeftControl) && IsKeyPressed(KeyboardKey.C))
                {
                    _clipboard = Selection;
                }
                // Paste action on the active selection of game objects
                if (IsKeyDown(KeyboardKey.LeftControl) && IsKeyPressed(KeyboardKey.V))
                {
                    Vector3 dir = GetCameraForward(ref Conceptor3D.EnvCamera) * 5f;
                    for (int i = 0; i < _clipboard.Count; i++)
                    {
                        UModel go = new UModel(
                            _clipboard[i].Name,
                            _clipboard[i].Position,
                            Ressource.GetModel(((UModel)_clipboard[i]).ModelID).Meshes[0],
                            ((UModel)_clipboard[i]).ModelID,
                            ((UModel)_clipboard[i]).TextureID);
                        // Set the rotations of the model
                        go.SetRotation(((UModel)_clipboard[i]).Pitch, ((UModel)_clipboard[i]).Yaw, ((UModel)_clipboard[i]).Roll);
                        // Set final position of the model
                        go.Position -= dir;
                        // Add the copied object to the list
                        CurrentScene.GameObjects.Add(go);
                    }
                    Selection.Clear();
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
                        go.Position = finalPos;
                    }
                }
                // Rotate the currently selected game object
                else if (IsKeyDown(KeyboardKey.R))
                {
                    foreach (GameObject3D go in Selection)
                    {
                        // Cast the object to apply the appropriate rotation effects
                        if (go is UModel)
                        {
                            UModel model = (UModel)CurrentScene.GameObjects.ElementAt(CurrentScene.GameObjects.IndexOf(go));
                            Conceptor3D.RotateObject(ref model);
                        }
                        else if (go is UCamera)
                        {
                            UCamera camera = (UCamera)CurrentScene.GameObjects.ElementAt(CurrentScene.GameObjects.IndexOf(go));
                            Conceptor3D.RotateObject(ref camera);
                        }
                    }
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

                DrawGrid(10, 10);

                // Draw the external skybox 
                DisableBackfaceCulling();
                DisableDepthMask();
                DrawMesh(Program.Skybox, Shaders.SkyboxMaterial, MatrixIdentity());
                EnableBackfaceCulling();
                EnableDepthMask();

                foreach (UModel go in CurrentScene.GameObjects.Where(x => x is UModel))
                {
                     DrawMesh(go.Mesh, go.Material, go.Transform);
                }

                EndMode3D();
                
                EndTextureMode();

                BeginMode3D(Conceptor3D.EnvCamera);
            }
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
                Program.Width = GetScreenWidth();
                Program.Height = GetScreenHeight();
                UI = new UI();
                GenSimulationView();
            }
            // Draw the outline rectangles that appear behind the main panels
            DrawRectangle(0, 0, (int)(Program.Width - Program.Width / 1.25f), Program.Height, BACKGROUND_COLOR);
            DrawRectangle(0, Program.Height - Program.Height / 3 - 10, Program.Width, Program.Height - (Program.Height - Program.Height / 3) + 10, BACKGROUND_COLOR);

            // Draw the entire UI and handle the events related to it
            UI.Draw();

            // Tick the error handler for the errors to potentially disappear
            ErrorHandler.Tick();

            // Draw and update file manager
            FileManager.Draw();

            // Render the selected camera POV to the top right corner of the screen
            if (Selection.Count == 1 && Selection.First() is UCamera cam && !RunningProject)
            {
                DrawRectangleLinesEx(new Rectangle(Program.Width - Program.Width / 5 - 11, 9, Program.Width / 5 + 2, Program.Height / 5 + 2), 2, Color.White);
                DrawTexturePro(_cameraView.Texture, _cameraViewRec, new Rectangle(Program.Width - Program.Width / 5 - 10, 10, Program.Width / 5, Program.Height / 5), Vector2.Zero, 0, Color.White);
            }
            if (RunningProject)
            {
                DrawTexturePro(GameSimView.Texture, _gameSimSourceRec, GameSimDestRectangle, Vector2.Zero, 0, Color.White);
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
                // Read project informations from .uproj
                FileStream stream = new FileStream(path, FileMode.Open);
                using BinaryReader reader = new BinaryReader(stream);
                string _projectName = reader.ReadString();
                byte[] _aesKey = reader.ReadBytes(DatEncoder.AES_KEY_LENGTH);
                byte[] _aesIv = reader.ReadBytes(DatEncoder.AES_IV_LENGTH);

                // Set Project info in application
                string? directory = Path.GetDirectoryName(path);

                // Load assets from the given project's assets folder
                if (directory is not null) FileManager.Init(directory);
                else throw new Exception($"Could not load the assets of project {path}");

                // Change to default assets page of the manager container
                ((Container)UI.Components["fileManager"]).ExtensionFile = "m3d";
                ((Container)UI.Components["fileManager"]).Tag = "models";
                ((Container)UI.Components["fileManager"]).OutputFilePath = directory + "/assets/models";
                ((Label)UI.Components["ressourceInfoLabel"]).Text = "File type : .m3d";

                // Load scenes along with their game objects
                CurrentProject = new Project(_projectName, path, LoadProjectScenes(directory, _aesKey, _aesIv), _aesKey, _aesIv);
                CurrentScene = CurrentProject.GetScene(0); // Set default scene
                SetWindowTitle("Uniray - " + _projectName);

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

                // Set file manager output to new project directory
                ((Container)UI.Components["fileManager"]).OutputFilePath = path + "/assets/models/";
                // Set new window title
                SetWindowTitle("Uniray - " + name);

                // Create new empty scene (except camera)
                UCamera ucamera = new UCamera("Default camera", UCamera.DefaultCamera);
                Scene defaultScene = new Scene("PLACEHOLDER", new List<GameObject3D> { ucamera });
                List<Scene> scenes = new() { defaultScene };
                CurrentProject = new Project(name, path + "/" + name + ".uproj", scenes, _aesKey, _aesIv); // Create project

                Ressource = new Ressource(); // Init ressource lists
                CurrentScene = CurrentProject.GetScene(0); // Set current scene to default
                DatEncoder.EncodeScene(CurrentScene); // Encode newly created scene to .DAT files

                TraceLog(TraceLogLevel.Info, "Project \"" + name + "\" has been created");
            }
            catch
            {
                ErrorHandler.Send(new Error(1, "Project " + name + " could not be created"));
            }
        }

        /// <summary>Loads the scenes from a given directory.</summary>
        /// <param name="directory">Directory to retrives scenes from.</param>
        /// <returns>The list of scenes contained in the given directory.</returns>
        public static List<Scene> LoadProjectScenes(string directory, byte[] key, byte[] iv)
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
                        model.Mesh = Ressource.GetModel(model.ModelID).Meshes[0];
                        if (model.TextureID != "") model.LoadTexture();
                    }
                }
                scenes.Add(scene);
            }
            return scenes;
        }

        /// <summary>Generates the simulation view for game simulation.</summary>
        private static void GenSimulationView()
        {
            // Game simulation view
            _gameSimSourceRec = new Rectangle(0, 0, Program.Width, -Program.Height);
            int renderHeight = Program.Height - (UI.Components["fileManager"].Height + 20);
            float renderRatio = (float)Program.Height / renderHeight;
            float renderWidth = Program.Width / (float)renderRatio;
            GameSimView = LoadRenderTexture(Program.Width, Program.Height);
            GameSimDestRectangle = new Rectangle(UI.Components["gameManager"].Width + 20, 0,
                renderWidth,
                renderHeight);
        }

        /// <summary>Runs the current project's game.</summary>
        public static void RunGame()
        {
            // Draw scene game objects
            foreach (UModel model in CurrentScene.GameObjects.Where(x => x is UModel))
            {
                DrawMesh(model.Mesh, model.Material, model.Transform);
            }
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

        // Fonction pour détecter un double-clic
        public static bool IsMouseButtonDoubleClicked(MouseButton button, string source)
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
    }
}