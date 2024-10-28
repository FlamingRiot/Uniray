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
    public unsafe struct Uniray
    { 
        /// <summary>Base FOV of the environment camera.</summary>
        internal const int CAMERA_FOV = 90;

        /// <summary>Current state of the program.</summary>
        internal static ProgramState State;

        /// <summary>Primary color of the application.</summary>
        public static readonly Color APPLICATION_COLOR = new(30, 30, 30, 255);
        /// <summary>Secondary color of the application.</summary>
        public static readonly Color FOCUS_COLOR = new(60, 60, 60, 255);

        /// <summary>
        /// The object containing all the assets that have been loaded in the RAM
        /// </summary>
        public static Ressource Ressource;

        /// <summary>Global UI of the application.</summary>
        public static UI UI = new UI();

        public static Camera3D EnvCamera;

        /// <summary>
        /// RenderTexture used for the selected camera's POV
        /// </summary>
        private RenderTexture2D cameraView;
        /// <summary>
        /// The rectangle used to render the selected camera's POV
        /// </summary>
        private Rectangle cameraViewRec;
        /// <summary>
        /// The error handler system of the application
        /// </summary>
        private ErrorHandler errorHandler;

        // File storage related attributes
        /// <summary>
        /// The texture used for rendering files in the file manager
        /// </summary>
        private Texture2D fileTex;
        /// <summary>
        /// The texture used for rendering folders in the file manager
        /// </summary>
        private Texture2D folderTex;

        // 3D related attributes
        /// <summary>
        /// The clipboard of the Game Objects
        /// </summary>
        private List<GameObject3D> clipboard;
        /// <summary>
        /// The Ray used to check collisions between the 2D and 3D world
        /// </summary>
        private Ray mouseRay;
        /// <summary>
        /// The collision detection used for the GameObjects
        /// </summary>
        private RayCollision goCollision;
        /// <summary>
        /// 3D model of the displayed scene camera
        /// </summary>
        public Model cameraModel;
        /// <summary>
        /// Material of the generic camera model used for the application
        /// </summary>
        public Material cameraMaterial;

        // Shader related attributes
        /// <summary>
        /// The built-in shaders of Uniray
        /// </summary>
        private UShaders shaders;
        /// <summary>
        /// 3D model of the skybox
        /// </summary>
        private Mesh skybox;
        /// <summary>
        /// Skybox panorama
        /// </summary>
        private Texture2D panorama;
        /// <summary>
        /// Skybox cubemap
        /// </summary>
        private Texture2D cubemap;
        /// <summary>
        /// The last time a button was pressed
        /// </summary>
        private double lastTimeButtonPressed;

        // Properties
        /// <summary>
        /// 3D model of the skybox
        /// </summary>
        public Mesh Skybox { get { return skybox; } set { skybox = value; } }
        /// <summary>
        /// The built-in shaders of Uniray
        /// </summary>
        public UShaders Shaders { get { return shaders; } }
        /// <summary>
        /// Uniray constructor
        /// </summary>
        /// <param name="WWindow">The width of the window</param>
        /// <param name="HWindow">The height of the window</param>
        /// <param name="scene">A default scene to be used until the user loads/create a project</param>
        public Uniray(Scene scene)
        {
            // Intitialize the Uniray shaders
            shaders = new UShaders();
            panorama = LoadTexture("data/shaders/skyboxes/industrial.hdr");
            cubemap = shaders.GenTexureCubemap(panorama, 256, PixelFormat.UncompressedR8G8B8A8);
            shaders.SetCubemap(cubemap);

            // Load hard ressources
            HardRessource.Init();

            // Unload useless texture
            UnloadTexture(panorama);

            // Intitialize the default scene and the render camera for Uniray
            CurrentScene = scene;
            EnvCamera = new Camera3D();

            // Init the clipboard list
            clipboard = new List<GameObject3D>();

            // Create the render texture for preview of a camera's POV
            cameraView = LoadRenderTexture(Program.Width / 2, Program.Height / 2);
            cameraViewRec = new Rectangle(0, 0, Program.Width / 2, -(Program.Height / 2));

            // 3D-2D Collision variables
            mouseRay = new Ray();
            goCollision = new RayCollision();

            // Load the Uniray camera model and apply its texture
            cameraModel = LoadModel("data/camera.m3d");
            cameraMaterial = LoadMaterialDefault();
            SetMaterialTexture(ref cameraMaterial, MaterialMapIndex.Diffuse, LoadTexture("data/cameraTex.png"));
            // Load some required textures
            fileTex = LoadTexture("data/img/file.png");
            folderTex = LoadTexture("data/img/folder.png");
            // Load the skybox model
            skybox = GenMeshCube(1.0f, 1.0f, 1.0f);

            // Initialize the volatile Data
            FileManager.CurrentFolder = FileManager.ModelFolder;
            // Initialize the error handler
            errorHandler = new ErrorHandler(new Vector2((UI.Components["fileManager"].X + UI.Components["fileManager"].Width / 2) - 150, UI.Components["fileManager"].Y - 60), RayGUI.Font);

        }
        public void DrawScene()
        {
            // =========================================================================================================================================================
            // ================================================================ MANAGE 3D DRAWING ======================================================================
            // =========================================================================================================================================================

            // Define a mouse ray for collision check
            Vector2 mousePos = GetMousePosition();
            mouseRay = GetMouseRay(mousePos, EnvCamera);

            // Render the outlined selected GameObjects and deactivate the Raylib culling to make it possible
            SetCullFace(ZERO);
            foreach (GameObject3D go in Selection)
            {
                switch (go)
                {
                    case UModel model:
                        DrawMesh(model.Mesh, shaders.OutlineMaterial, model.Transform);
                        break;
                    case UCamera camera:
                        DrawMesh(cameraModel.Meshes[0], shaders.OutlineMaterial, camera.Transform);
                        break;
                }
            }
            SetCullFace(ONE);

            // Draw 3-Dimensional models of the current scene and check for a hypothetical new selected object
            short index = -1;
            foreach (GameObject3D go in CurrentScene.GameObjects)
            {
                // Manage objects drawing + object selection (according to the object type)
                if (go is UModel)
                {
                    DrawMesh(((UModel)go).Mesh, ((UModel)go).Material, ((UModel)go).Transform);
                    short i = (short)CheckCollisionScreenToWorld(go, ((UModel)go).Mesh, mousePos, ((UModel)go).Transform);
                    index = CheckDistance(index, i);
                }
                else if (go is UCamera)
                {
                    DrawMesh(cameraModel.Meshes[0], cameraMaterial, ((UCamera)go).Transform);
                    short i = (short)CheckCollisionScreenToWorld(go, cameraModel.Meshes[0], mousePos, ((UCamera)go).Transform);
                    index = CheckDistance(index, i);
                }
            }

            // Assign the newly selected object to the according variable
            if (index != -1)
            {
                if (IsKeyDown(KeyboardKey.LeftShift))
                {
                    Selection.Add(CurrentScene.GetGameObject(index));
                }
                else
                {
                    Selection.Clear();
                    Selection.Add(CurrentScene.GetGameObject(index));
                }
            }
            
            // Manage GameObjects interaction
            if (Selection.Count != 0) 
            {
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
                    clipboard = Selection;
                }
                // Paste action on the active selection of game objects
                if (IsKeyDown(KeyboardKey.LeftControl) && IsKeyPressed(KeyboardKey.V))
                {
                    Vector3 dir = GetCameraForward(ref EnvCamera) * 5f;
                    for (int i = 0; i < clipboard.Count; i++)
                    {
                        UModel go = new UModel(
                            clipboard[i].Name,
                            clipboard[i].Position,
                            Ressource.GetModel(((UModel)clipboard[i]).ModelID).Meshes[0],
                            ((UModel)clipboard[i]).ModelID,
                            ((UModel)clipboard[i]).TextureID);
                        // Set the rotations of the model
                        go.SetRotation(((UModel)clipboard[i]).Pitch, ((UModel)clipboard[i]).Yaw, ((UModel)clipboard[i]).Roll);
                        // Set final position of the model
                        go.Position -= dir;
                        // Add the copied object to the list
                        CurrentScene.AddGameObject(go);
                    }
                    Selection.Clear();
                }

                // Manage GameObjects transformations effects
                if (Selection.Count != 0)
                {
                    // Translate the currently selected object, indpendently of its type
                    if (IsKeyDown(KeyboardKey.G))
                    {
                        // Translate a neutral vector for selection uniformity
                        Vector3 newPos = TranslateObject(Vector3.Zero);
                        foreach (GameObject3D go in Selection)
                        {
                            // Add the position of the current object
                            Vector3 finalPos = Vector3Add(newPos, go.Position);
                            CurrentScene.SetGameObjectPosition(CurrentScene.GameObjects.IndexOf(go), finalPos);
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
                                RotateObject(ref model);
                            }
                            else if (go is UCamera)
                            {
                                UCamera camera = (UCamera)CurrentScene.GameObjects.ElementAt(CurrentScene.GameObjects.IndexOf(go));
                                RotateObject(ref camera);
                            }
                        }
                        HideCursor();
                    }
                    else ShowCursor();
                }
            }

            // Draw the scene all over again for the camera render, if activated
            if (Selection.Count == 1 && Selection.First() is UCamera cam)
            {
                EndMode3D();

                BeginTextureMode(cameraView);

                ClearBackground(new Color(70, 70, 70, 255));

                BeginMode3D(cam.Camera);

                DrawGrid(10, 10);

                // Draw the external skybox 
                DisableBackfaceCulling();
                DisableDepthMask();
                DrawMesh(skybox, shaders.SkyboxMaterial, MatrixIdentity());
                EnableBackfaceCulling();
                EnableDepthMask();

                foreach (GameObject3D go in CurrentScene.GameObjects)
                {
                    // Manage objects drawing + object selection (according to the object type)
                    if (go is UModel)
                    {
                        DrawMesh(((UModel)go).Mesh, ((UModel)go).Material, ((UModel)go).Transform);
                    }
                }

                EndMode3D();
                
                EndTextureMode();

                BeginMode3D(EnvCamera);
            }
        }
        /// <summary>
        /// Draw user interface of the application
        /// </summary>
        public void DrawUI()
        {
            // =========================================================================================================================================================
            // ================================================================ MANAGE 2D DRAWING ======================================================================
            // =========================================================================================================================================================
            // Check if the window has been resized to adjust the size of the UI
            if (IsWindowResized())
            {
                Program.Width = GetScreenWidth();
                Program.Height = GetScreenHeight();
                UI = new UI();   
            }
            // Draw the outline rectangles that appear behind the main panels
            DrawRectangle(0, 0, (int)(Program.Width - Program.Width / 1.25f), Program.Height, new Color(20, 20, 20, 255));
            DrawRectangle(0, Program.Height - Program.Height / 3 - 10, Program.Width, Program.Height - (Program.Height - Program.Height / 3) + 10, new Color(20, 20, 20, 255));

            // Draw the entire UI and handle the events related to it
            UI.Draw();

            // Tick the error handler for the errors to potentially disappear
            errorHandler.Tick();

            // Draw and update file manager
            FileManager.Draw();

            // Render the selected camera POV to the top right corner of the screen
            if (Selection.Count == 1 && Selection.First() is UCamera cam)
            {
                DrawRectangleLinesEx(new Rectangle(Program.Width - Program.Width / 5 - 11, 9, Program.Width / 5 + 2, Program.Height / 5 + 2), 2, Color.White);
                DrawTexturePro(cameraView.Texture, cameraViewRec, new Rectangle(Program.Width - Program.Width / 5 - 10, 10, Program.Width / 5, Program.Height / 5), Vector2.Zero, 0, Color.White);
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

        /// <summary>
        /// Load project from .uproj file
        /// </summary>
        /// <param name="path">path to the .uproj file</param>
        public void LoadProject(string path)
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
                errorHandler.Send(new Error(2, "Project could not be loaded !"));
            }
        }

        /// <summary>
        /// Save current game objects and scenes
        /// </summary>
        public void SaveProject()
        {
            if (UData.CurrentProject != null)
            {
                string[] jsons = JsonfyGos(CurrentScene.GameObjects);
                string? path;
                if (UData.CurrentProject.Path.Contains('.'))
                {
                    path = Path.GetDirectoryName(UData.CurrentProject.Path);
                }
                else
                {
                    path = UData.CurrentProject.Path;
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
        public void CreateProject(string path, string name)
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

                Scene defaultScene = new(new List<GameObject3D> { ucamera });
                List<Scene> scenes = new() { defaultScene };
                CurrentProject = new Project(name, path + "\\" + name + ".uproj", scenes);
                Ressource = new Ressource();
                CurrentScene = CurrentProject.GetScene(0);
                Selection.Clear();

                TraceLog(TraceLogLevel.Warning, "Project \"" + name + "\" has been created");
            }
            catch
            {
                errorHandler.Send(new Error(1, "Project " + name + " could not be created"));
            }
        }

        /// <summary>
        /// String definition of the uniray class
        /// </summary>
        /// <returns>Message</returns>
        public override string ToString()
        {
            return "Uniray is a game engine by Evan Comtesse";
        }

        /// <summary>
        /// Convert game objects list to json according to the ressource dictionaries
        /// </summary>
        /// <param name="gos">Game objects list</param>
        /// <returns></returns>
        public string[] JsonfyGos(List<GameObject3D> gos)
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
        public List<Scene> LoadScenes(string directory)
        {
            string[] scenesPath = Directory.GetDirectories(directory + "\\scenes");
            List<Scene> scenes = new();
            for (int i = 0; i < scenesPath.Length; i++)
            {
                // Import stored camera
                StreamReader rCam = new(scenesPath[i] + "\\camera.json");
                string camJson = rCam.ReadToEnd();
                List<UCamera> ucameras = JsonConvert.DeserializeObject<List<UCamera>>(camJson);
                rCam.Close();
                // Import stored game objects
                StreamReader rGos = new(scenesPath[i] + "\\locs.json");
                string gosJson = rGos.ReadToEnd();
                List<UModel>? umodels = JsonConvert.DeserializeObject<List<UModel>>(gosJson);
                rGos.Close();

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
                }

                // Transfer all the objects into a single GameObject3D list
                List<GameObject3D> gos = new List<GameObject3D>();
                gos.AddRange(ucameras);
                gos.AddRange(umodels);

                scenes.Add(new Scene(gos));
            }
            return scenes;
        }
        
        /// <summary>
        /// Translate a 3-Dimensional vector according to the application standards in Dev environement
        /// </summary>
        /// <param name="position">3-Dimensional vector representing the position to translate</param>
        /// <returns></returns>
        public Vector3 TranslateObject(Vector3 position)
        {
            if (IsKeyDown(KeyboardKey.X))
            {
                position.X += (GetCameraRight(ref EnvCamera) * GetMouseDelta().X / 500 * Vector3Distance(position, EnvCamera.Position)).X;
            }
            else if (IsKeyDown(KeyboardKey.Y))
            {
                position.Z += (GetCameraForward(ref EnvCamera) * GetMouseDelta().X / 500 * Vector3Distance(position, EnvCamera.Position)).X;
            }
            else if (IsKeyDown(KeyboardKey.Z))
            {
                position.Y -= (GetCameraUp(ref EnvCamera) * GetMouseDelta().Y / 500 * Vector3Distance(position, EnvCamera.Position)).Y;
            }
            else
            {
                position += GetCameraRight(ref EnvCamera) * GetMouseDelta().X / 500 * Vector3Distance(position, EnvCamera.Position);
                position -= GetCameraUp(ref EnvCamera) * GetMouseDelta().Y / 500 * Vector3Distance(position, EnvCamera.Position);
            }
            HideCursor();
            return position;
        }

        /// <summary>
        /// Build the current project loaded in Uniray
        /// </summary>
        public void BuildProject(string path)
        {
            // Build and run project here...
            string? projectPath = Path.GetDirectoryName(path);
            string commmand = "/C cd " + projectPath + " && dotnet run --project uniray_Project.csproj";
            System.Diagnostics.Process.Start("CMD.exe", commmand);
        }
        /// <summary>
        /// Check if a collision occurs between the mouse (screen) and an object of the world (Game object) and 
        /// returns the index of the selected object
        /// </summary>
        /// <param name="go">Game object</param>
        /// <param name="mesh">Mesh</param>
        /// <param name="mousePos">2-Dimensional position of the mouse</param>
        /// <returns>Selected object index (-1 if nothing found)</returns>
        public int CheckCollisionScreenToWorld(GameObject3D go, Mesh mesh, Vector2 mousePos, Matrix4x4 transform)
        {
            if (mousePos.X > UI.Components["gameManager"].X + UI.Components["gameManager"].Width && mousePos.Y < UI.Components["fileManager"].Y - 10 && IsMouseButtonPressed(MouseButton.Left))
            {

                goCollision = GetRayCollisionMesh(mouseRay, mesh, transform);
                if (goCollision.Hit)
                {
                    return CurrentScene.GameObjects.IndexOf(go);
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// Check distance from camera between two game objects
        /// </summary>
        /// <param name="index1">Index of the first object</param>
        /// <param name="index2">Index of the second object</param>
        /// <returns>The index to change or keep</returns>
        public short CheckDistance(short index1, short index2)
        {
            if (index2 != -1)
            {
                if (index1 != -1)
                {
                    // Retrive the position data of the conflicted game objects
                    Vector3 iPos1 = CurrentScene.GameObjects[index1].Position;
                    Vector3 iPos2 = CurrentScene.GameObjects[index2].Position;

                    if (Vector3Distance(iPos1, EnvCamera.Position) > Vector3Distance(iPos2, EnvCamera.Position))
                    {
                        // Set the new selected game object as closer
                        return index2;
                    }
                    else
                    {
                        return index1;
                    }
                }
                // Set the first potential selected element
                return index2;
            }
            // Keep the old one 
            return index1;
        }
        /// <summary>
        /// Rotate a model according to the mouse delta
        /// </summary>
        /// <param name="model">UModel to rotate</param>
        public void RotateObject(ref UModel model)
        {
            // Rotate the model according to the 3-Dimensional axes
            Vector2 mouse = GetMouseDelta();
            if (IsKeyDown(KeyboardKey.X))
            {
                model.Pitch += mouse.Y;
            }
            if (IsKeyDown(KeyboardKey.Y))
            {
                model.Roll += mouse.Y;
            }
            if (IsKeyDown(KeyboardKey.Z))
            {
                model.Yaw += mouse.X;
            }
            // Set the model tranform
            Matrix4x4 nm = MatrixRotateXYZ(new Vector3(model.Pitch / RAD2DEG, model.Yaw / RAD2DEG, model.Roll / RAD2DEG));

            nm.M14 = model.Position.X;
            nm.M24 = model.Position.Y;
            nm.M34 = model.Position.Z;

            model.Transform = nm;
        }
        /// <summary>
        /// Rotate a camera according to the mouse delta
        /// </summary>
        /// <param name="camera">UCamrea to rotate</param>
        public void RotateObject(ref UCamera camera)
        {
            // Rotate the camera according to the 3-Dimensional axes
            Vector2 mouse = GetMouseDelta();
            if (IsKeyDown(KeyboardKey.X))
            {
                camera.Pitch += mouse.Y;
            }
            if (IsKeyDown(KeyboardKey.Y))
            {
                camera.Roll += mouse.Y;
            }
            if (IsKeyDown(KeyboardKey.Z))
            {
                camera.Yaw += mouse.X;
            }
            // Set the camera model Transform
            Matrix4x4 nm = MatrixRotateXYZ(new Vector3(camera.Pitch / RAD2DEG, camera.Yaw / RAD2DEG, camera.Roll / RAD2DEG));

            nm.M14 = camera.Position.X;
            nm.M24 = camera.Position.Y;
            nm.M34 = camera.Position.Z;

            camera.Transform = nm;
            // Set the camera Target according to the rotation matrix

        }
    }
}

namespace Raylib_cs.Complements
{
    public static class RaylibComplements
    {
        public static double LastTimeButtonPressed;

        public static bool FirstLoopEntry = true;

        // Fonction pour détecter un double-clic
        public static bool IsMouseButtonDoubleClicked(MouseButton button, string source)
        {
            double currentFrame = GetTime();

            if (IsMouseButtonPressed(button))
            {
                Console.WriteLine("Je viens de " + source);
                Console.WriteLine("1: " + (currentFrame - LastTimeButtonPressed));
                if (FirstLoopEntry && (currentFrame - LastTimeButtonPressed) >= 0.25)
                {
                    LastTimeButtonPressed = currentFrame;
                    FirstLoopEntry = false;
                }
                Console.WriteLine("2: " + (currentFrame - LastTimeButtonPressed));
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