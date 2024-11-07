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

        // -----------------------------------------------------------
        // Private and internal instances
        // -----------------------------------------------------------
        private static RenderTexture2D _cameraView;
        private static Rectangle _cameraViewRec;
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
            }
            // Draw the outline rectangles that appear behind the main panels
            DrawRectangle(0, 0, (int)(Program.Width - Program.Width / 1.25f), Program.Height, new Color(40, 40, 40, 255));
            DrawRectangle(0, Program.Height - Program.Height / 3 - 10, Program.Width, Program.Height - (Program.Height - Program.Height / 3) + 10, new Color(40, 40, 40, 255));

            // Draw the entire UI and handle the events related to it
            UI.Draw();

            // Tick the error handler for the errors to potentially disappear
            ErrorHandler.Tick();

            // Draw and update file manager
            FileManager.Draw();

            // Render the selected camera POV to the top right corner of the screen
            if (Selection.Count == 1 && Selection.First() is UCamera cam)
            {
                DrawRectangleLinesEx(new Rectangle(Program.Width - Program.Width / 5 - 11, 9, Program.Width / 5 + 2, Program.Height / 5 + 2), 2, Color.White);
                DrawTexturePro(_cameraView.Texture, _cameraViewRec, new Rectangle(Program.Width - Program.Width / 5 - 10, 10, Program.Width / 5, Program.Height / 5), Vector2.Zero, 0, Color.White);
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
            // Check the shortcut for game building
            if (IsKeyPressed(KeyboardKey.F5) && CurrentProject is not null)
            {
                BuildProject(CurrentProject.Path);
            }
        }

        /// <summary>Loads project from .uproj file.</summary>
        /// <param name="path">path to the .uproj file</param>
        public static void LoadProject(string path)
        {
            try
            {
                string project_name = "";
                StreamReader stream = new StreamReader(path);
                if (stream.ReadLine() == "<Project>")
                {
                    string? line = stream.ReadLine();
                    if (line is not null) project_name = line.Split('>')[1].Split('<')[0];
                    else throw new Exception("The given project file was empty, or contained wrong information");
                }
                stream.Close();

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
                CurrentProject = new Project(project_name, path, LoadScenes(directory));
                CurrentScene = CurrentProject.GetScene(0);
                SetWindowTitle("Uniray - " + project_name);

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
            if (CurrentProject != null)
            {
                //DatabaseConnection connection = new DatabaseConnection(CurrentScene)

                string[] jsons = JsonfyGos(CurrentScene.GameObjects);
                string? path;
                if (CurrentProject.Path.Contains('.'))
                {
                    path = Path.GetDirectoryName(CurrentProject.Path);
                }
                else
                {
                    path = CurrentProject.Path;
                }
                
                StreamWriter stream = new (path + "/scenes/new_scene/locs.json", false);
                stream.Write(jsons[0]);
                stream.Close();

                StreamWriter camStream = new (path + "/scenes/new_scene/camera.json", false);
                camStream.Write(jsons[1]);
                camStream.Close();

                TraceLog(TraceLogLevel.Info, "Project was saved successfully !");
            }
            else
            {
                // Open modal for the user to create a new project
                UData.CurrentModal = "newProjectModal";
            }
        }

        /// <summary>
        /// Create project (.uproj file)
        /// </summary>
        /// <param name="path">path to the target directory</param>
        public static void CreateProject(string path, string name)
        {
            try
            {
                // Create directories (assets, locs, etc.)
                Directory.CreateDirectory(path + "\\assets");

                Directory.CreateDirectory(path + "\\assets\\animations");
                Directory.CreateDirectory(path + "\\assets\\models");
                Directory.CreateDirectory(path + "\\assets\\scripts");
                Directory.CreateDirectory(path + "\\assets\\sounds");
                Directory.CreateDirectory(path + "\\assets\\textures");

                Directory.CreateDirectory(path + "\\scenes\\new_scene\\");
                StreamWriter default_locs = new (path + "\\scenes\\new_scene\\locs.json");
                default_locs.WriteLine("[");
                default_locs.WriteLine("    {");
                default_locs.WriteLine("    }");
                default_locs.WriteLine("]");
                default_locs.Close();

                // Unzip default Visual Studio project from internal data
                System.IO.Compression.ZipFile.ExtractToDirectory("data/default_VS_Project.zip", path);

                // Create .uproj file
                StreamReader read = new ("data\\project_template.txt");
                string content = "";
                while (!read.EndOfStream) 
                {
                    content += read.ReadLine() + '$';
                }
                read.Close();
                // Replace specific elements
                content = content.Replace("{ASSETS_PATH}", path + "\\assets");
                content = content.Replace("{JSON_PATH}", path + "\\scenes\\new_scene\\locs.json");
                content = content.Replace("{PROJECT_NAME}", name);
                string[] file = content.Split('$');
                StreamWriter write = new (path + "\\" + name + ".uproj");
                for (int i = 0; i < file.Length; i++)
                {
                    write.WriteLine(file[i]);
                }
                write.Close();
                ((Container)UI.Components["fileManager"]).OutputFilePath = path + "\\assets\\models\\";

                SetWindowTitle("Uniray - " + name);

                Camera3D camera = new()
                {
                    Position = Vector3.Zero,
                    Target = Vector3.Zero,
                    Up = Vector3.UnitY,
                    Projection = CameraProjection.Perspective,
                    FovY = 90
                };
                UCamera ucamera = new UCamera("Default camera", camera);
                Matrix4x4 transform = Matrix4x4.Identity;

                StreamWriter cs = new StreamWriter(path + "\\scenes\\new_scene\\camera.json");
                string newCam = JsonConvert.SerializeObject(ucamera);
                cs.Write(newCam);
                cs.Close();

                Scene defaultScene = new Scene("PLACEHOLDER", new List<GameObject3D> { ucamera });
                List<Scene> scenes = new() { defaultScene };
                CurrentProject = new Project(name, path + "\\" + name + ".uproj", scenes);
                Ressource = new Ressource();
                CurrentScene = CurrentProject.GetScene(0);
                Selection.Clear();

                TraceLog(TraceLogLevel.Warning, "Project \"" + name + "\" has been created");
            }
            catch
            {
                ErrorHandler.Send(new Error(1, "Project " + name + " could not be created"));
            }
        }

        /// <summary>Converts the informations of a <see cref="List{T}"/> of <see cref="GameObject3D"/> to a JSON stream.</summary>
        /// <param name="gos">Game objects list</param>
        /// <returns>The JSON stream containing the informations.</returns>
        private static string[] JsonfyGos(List<GameObject3D> gos)
        {
            // Open the jsons
            string modelsJson = "[";
            string cameraJson = "[";
            // Go through every element of the scene's list
            foreach(GameObject3D go in gos)
            {
                if (go is UModel model)
                {
                    modelsJson += "{" + "X: " + model.X + ",Y: " + model.Y + ",Z: " + model.Z + ",Yaw: " + model.Yaw +
                    ",Pitch: " + model.Pitch + ",Roll: " + model.Roll + ",ModelID: \"" + model.ModelID + "\",TextureID: \"" + model.TextureID + "\", Transform:";
                    modelsJson += JsonConvert.SerializeObject(model.Transform);
                    modelsJson += "}, ";
                }
                else if (go is UCamera camera)
                {
                    cameraJson += JsonConvert.SerializeObject(camera);
                    cameraJson += ",";
                }

            }
            // Delete the last comma of the jsons
            
            if (modelsJson != "[") modelsJson = modelsJson.Substring(0, modelsJson.LastIndexOf(','));
            if (cameraJson != "[") cameraJson = cameraJson.Substring(0, cameraJson.LastIndexOf(','));

            // Close the jsons
            modelsJson += "]";
            cameraJson += "]";
            return new string[] { modelsJson, cameraJson };
        }

        /// <summary>
        /// Load all the scenes from a project directory
        /// </summary>
        /// <returns></returns>
        public static List<Scene> LoadScenes(string directory)
        {
            string[] scenesPath = Directory.GetDirectories(directory + "\\scenes");
            List<Scene> scenes = new();
            for (int i = 0; i < scenesPath.Length; i++)
            {
                // Import stored camera
                StreamReader rCam = new(scenesPath[i] + "\\camera.json");
                string camJson = rCam.ReadToEnd();
                List<UCamera>? ucameras = JsonConvert.DeserializeObject<List<UCamera>>(camJson);
                rCam.Close();
                // Import stored game objects
                StreamReader rGos = new(scenesPath[i] + "\\locs.json");
                string gosJson = rGos.ReadToEnd();
                List<UModel>? umodels = JsonConvert.DeserializeObject<List<UModel>>(gosJson);
                rGos.Close();

                List<GameObject3D> gos = new List<GameObject3D>();
                // Prepare the models for insertion
                if (umodels is not null)
                {
                    foreach (UModel go in umodels)
                    {
                        if (go.ModelID != "" && go.ModelID is not null)
                        {
                            go.Mesh = Ressource.GetModel(go.ModelID).Meshes[0];
                            if (go.TextureID != "") go.SetTexture(go.TextureID, Ressource.GetTexture(go.TextureID));
                        }
                    }
                    gos.AddRange(umodels);
                }

                if (ucameras is not null)
                {
                    foreach (UCamera uCamera in ucameras)
                    {
                        Camera3D camera = new Camera3D()
                        {
                            Position = uCamera.Position,
                            Target = Vector3.Zero,
                            Up = Vector3.UnitY,
                            FovY = 45f,
                            Projection = CameraProjection.Perspective
                        };

                        uCamera.Camera = camera;
                    }
                    gos.AddRange(ucameras);
                }
                scenes.Add(new Scene("PLACEHOLDER", gos));
            }
            return scenes;
        }

        /// <summary>Builds the currently load project.</summary>
        public static void BuildProject(string path)
        {
            // Build and run project here...
            string? projectPath = Path.GetDirectoryName(path);
            string commmand = "/C cd " + projectPath + " && dotnet run --project uniray_Project.csproj";
            System.Diagnostics.Process.Start("CMD.exe", commmand);
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