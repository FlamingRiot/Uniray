using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;
using static Raylib_cs.Rlgl;
using Raylib_cs;
using static RayGUI_cs.RayGUI;
using RayGUI_cs;
using System.Numerics;
using System.Text;
using Newtonsoft.Json;
using System.Windows.Input;

namespace Uniray
{
    public unsafe struct Uniray
    {
        /// <summary>
        /// Public application main color
        /// </summary>
        public static readonly Color APPLICATION_COLOR = new Color(30, 30, 30, 255);
        /// <summary>
        /// Public application secondary color
        /// </summary>
        public static readonly Color FOCUS_COLOR = new Color(60, 60, 60, 255);

        /// <summary>
        /// The object containing all the assets that have been loaded in the RAM
        /// </summary>
        public static Ressource Ressource;
        /// <summary>
        /// The object containing all the short-life/interactive informations of the application
        /// </summary>
        public static UData Data;

        // 2D related attributes
        /// <summary>
        /// The UI of the application
        /// </summary>
        public UI UI;
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

        private List<UStorage> modelsPathList;

        private List<UStorage> texturesPathList;

        private List<UStorage> soundsPathList;

        private List<UStorage> animationsPathList;

        private List<UStorage> scriptPathList;

        // 3D related attributes
        /// <summary>
        /// The currently displayed scene
        /// </summary>
        private Scene currentScene;
        /// <summary>
        /// The currently selected GameObject
        /// </summary>
        private GameObject3D? selectedElement;
        /// <summary>
        /// The used camera for rendering the 3D of the application
        /// </summary>
        private Camera3D envCamera;
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
        public Camera3D EnvCamera { get { return envCamera; } set { envCamera = value; } }
        /// <summary>
        /// 3D model of the skybox
        /// </summary>
        public Mesh Skybox { get { return skybox; } set { skybox = value; } }
        /// <summary>
        /// The currently used scene
        /// </summary>
        public Scene CurrentScene { get { return currentScene; } }
        /// <summary>
        /// The built-in shaders of Uniray
        /// </summary>
        public UShaders Shaders { get { return shaders; } }
        /// <summary>
        /// Uniray constructor
        /// </summary>
        /// <param name="WWindow">The width of the window</param>
        /// <param name="HWindow">The height of the window</param>
        /// <param name="font">The base font that will be used for the application UI</param>
        /// <param name="scene">A default scene to be used until the user loads/create a project</param>
        public Uniray(int WWindow, int HWindow, Font font, Scene scene)
        {
            // Intitialize the Uniray shaders
            shaders = new UShaders();
            panorama = LoadTexture("data/shaders/skyboxes/industrial.hdr");
            cubemap = shaders.GenTexureCubemap(panorama, 256, PixelFormat.UncompressedR8G8B8A8);
            shaders.SetCubemap(cubemap);
            // Unload useless texture
            UnloadTexture(panorama);

            // Intitialize the default scene and the render camera for Uniray
            currentScene = scene;
            envCamera = new Camera3D();

            // Create the render texture for preview of a camera's POV
            cameraView = LoadRenderTexture(WWindow / 2, HWindow / 2);
            cameraViewRec = new Rectangle(0, 0, WWindow / 2, -(HWindow / 2));

            // 3D-2D Collision variables
            selectedElement = null;
            mouseRay = new Ray();
            goCollision = new RayCollision();

            // Intitialize all the assets lists of filepath
            modelsPathList = new List<UStorage>();
            texturesPathList = new List<UStorage>();
            soundsPathList = new List<UStorage>();
            animationsPathList = new List<UStorage>();
            scriptPathList = new List<UStorage>();

            // Load the Uniray camera model and apply its texture
            cameraModel = LoadModel("data/camera.m3d");
            cameraMaterial = LoadMaterialDefault();
            SetMaterialTexture(ref cameraMaterial, MaterialMapIndex.Diffuse, LoadTexture("data/cameraTex.png"));
            // Load some required textures
            fileTex = LoadTexture("data/img/file.png");
            folderTex = LoadTexture("data/img/folder.png");
            // Load the skybox model
            skybox = GenMeshCube(1.0f, 1.0f, 1.0f);

            // Initialize the Data
            Data = new UData();
            // Intitialize UI
            UI = new UI(WWindow, HWindow, font);
            // Initialize the error handler
            errorHandler = new ErrorHandler(new Vector2((UI.Components["fileManager"].X + UI.Components["fileManager"].Width / 2) - 150, UI.Components["fileManager"].Y - 60), font);

        }
        public void DrawScene()
        {
            // =========================================================================================================================================================
            // ================================================================ MANAGE 3D DRAWING ======================================================================
            // =========================================================================================================================================================

            // Update the selected element from the reference list
            if (selectedElement != null) { selectedElement = currentScene.GameObjects.ElementAt(currentScene.GameObjects.IndexOf(selectedElement)); }

            // Define a mouse ray for collision check
            Vector2 mousePos = GetMousePosition();
            mouseRay = GetMouseRay(mousePos, EnvCamera);

            // Render the outlined selected GameObject and deactive the Raylib culling to make it possible
            SetCullFace(ZERO);
            switch (selectedElement)
            {
                case UModel model:
                    DrawMesh(model.Mesh, shaders.OutlineMaterial, model.Transform);
                    break;
                case UCamera camera:
                    DrawMesh(cameraModel.Meshes[0], shaders.OutlineMaterial, camera.Transform);
                    break;
            }
            SetCullFace(ONE);

            // Draw 3-Dimensional models of the current scene and check for a hypothetical new selected object
            int index = -1;
            foreach (GameObject3D go in currentScene.GameObjects)
            {
                // Manage objects drawing + object selection (according to the object type)
                if (go is UModel)
                {
                    DrawMesh(((UModel)go).Mesh, ((UModel)go).Material, ((UModel)go).Transform);
                    if (index == -1) index = CheckCollisionScreenToWorld(go, ((UModel)go).Mesh, mousePos, ((UModel)go).Transform);
                }
                else if (go is UCamera)
                {
                    DrawMesh(cameraModel.Meshes[0], cameraMaterial, ((UCamera)go).Transform);
                    if (index == -1) index = CheckCollisionScreenToWorld(go, cameraModel.Meshes[0], mousePos, ((UCamera)go).Transform);
                }
            }

            // Assign the newly selected object to the according variable
            if (index != -1)
            {
                selectedElement = currentScene.GetGameObject(index);
            }
            
            // Manage GameObjects interaction
            if (selectedElement != null) 
            {
                if (IsKeyPressed(KeyboardKey.Delete))
                {
                    currentScene.GameObjects.Remove(selectedElement);
                    selectedElement = null;
                }
                if (IsKeyPressed(KeyboardKey.Escape))
                {
                    selectedElement = null;
                }

                // Manage GameObjects transformations effects
                if (selectedElement is not null)
                {
                    // Translate the currently selected object, indpendently of its type
                    if (IsKeyDown(KeyboardKey.G))
                    {
                        Vector3 newPos = TranslateObject(selectedElement.Position);
                        currentScene.SetGameObjectPosition(currentScene.GameObjects.IndexOf(selectedElement), newPos);
                    }
                    // Rotate the currently selected game object
                    else if (IsKeyDown(KeyboardKey.R))
                    {
                        // Cast the object to apply the appropriate rotation effects
                        if (selectedElement is UModel)
                        {
                            UModel model = (UModel)currentScene.GameObjects.ElementAt(currentScene.GameObjects.IndexOf(selectedElement));
                            RotateObject(ref model);                            
                        }
                        else if (selectedElement is UCamera)
                        {
                            UCamera camera = (UCamera)currentScene.GameObjects.ElementAt(currentScene.GameObjects.IndexOf(selectedElement));
                            RotateObject(ref camera);
                        }
                        HideCursor();
                    }
                    else ShowCursor();
                }
            }

            // Draw the scene all over again for the camera render, if activated
            if (selectedElement is UCamera cam)
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

                foreach (GameObject3D go in currentScene.GameObjects)
                {
                    // Manage objects drawing + object selection (according to the object type)
                    if (go is UModel)
                    {
                        DrawMesh(((UModel)go).Mesh, ((UModel)go).Material, ((UModel)go).Transform);
                    }
                }

                EndMode3D();
                
                EndTextureMode();

                BeginMode3D(envCamera);
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
                UI = new UI(GetScreenWidth(), GetScreenHeight(), UI.Font);   
            }
            // Draw the outline rectangles that appear behind the main panels
            DrawRectangle(0, 0, (int)(UI.Width - UI.Width / 1.25f), UI.Height, new Color(20, 20, 20, 255));
            DrawRectangle(0, UI.Height - UI.Height / 3 - 10, UI.Width, UI.Height - (UI.Height - UI.Height / 3) + 10, new Color(20, 20, 20, 255));

            // Draw the entire UI and handle the events related to it
            UI.Draw();

            // Tick the error handler for the errors to potentially disappear
            errorHandler.Tick();

            string new_File = "";
            new_File = ((Container)UI.Components["fileManager"]).GetLastFile();
            new_File = new_File.Replace('\\', '/');
            UFile file = new UFile(new_File);
            if (((Container)UI.Components["fileManager"]).GetLastFile() != ((Container)UI.Components["fileManager"]).LastFile && Data.CurrentProject is not null)
            {
                switch (new_File.Split('.').Last())
                {
                    case "m3d":
                        if (modelsPathList.Count == 0)
                        {
                            modelsPathList.Add(file);
                        }
                        else if (modelsPathList.Last().Name != file.Name)
                        {
                            modelsPathList.Add(file);
                        }
                        break;
                    case "png":
                        if (texturesPathList.Count == 0)
                        {
                            texturesPathList.Add(file);
                            Ressource.AddTexture(LoadTexture(new_File), new_File.Split('/').Last().Split('.')[0]);
                        }
                        else if (texturesPathList.Last().Name != file.Name)
                        {
                            texturesPathList.Add(file);
                            Ressource.AddTexture(LoadTexture(new_File), new_File.Split('/').Last().Split('.')[0]);
                        }
                        break;
                    case "wav":
                        break;
                    case "cs":
                        break;
                }
                ((Container)UI.Components["fileManager"]).ClearFiles();
                ((Container)UI.Components["fileManager"]).LastFile = "";
            }
            // Draw the files along with their name in the file manager
            int index = 0;
            switch (UI.Components["fileManager"].Tag)
            {
                case "models":
                    DrawManagerFiles(ref modelsPathList, ref index);
                    break;
                case "textures":
                    DrawManagerFiles(ref texturesPathList, ref index);
                    break;
                case "sounds":
                    DrawManagerFiles(ref soundsPathList, ref index);
                    break;
                case "animations":
                    DrawManagerFiles(ref animationsPathList, ref index);
                    break;
                case "scripts":
                    DrawManagerFiles(ref scriptPathList, ref index);
                    break;
            }

            // Render the selected camera POV to the top right corner of the screen
            if (selectedElement is UCamera)
            {
                DrawRectangleLinesEx(new Rectangle(UI.Width - UI.Width / 5 - 11, 9, UI.Width / 5 + 2, UI.Height / 5 + 2), 2, Color.White);
                DrawTexturePro(cameraView.Texture, cameraViewRec, new Rectangle(UI.Width - UI.Width / 5 - 10, 10, UI.Width / 5, UI.Height / 5), Vector2.Zero, 0, Color.White);
            }

            // Draw the currently displayed modal and define its state
            if (Data.CurrentModal is not null)
            {
                // If the user closed the modal without proceeding
                if (Data.LastModalExitCode == 0)
                {
                    Data.CurrentModal = null;
                    Data.LastModalExitCode = null;
                }
                // If the user closed the modal by proceeding
                else if (Data.LastModalExitCode == 1)
                {
                    switch (Data.CurrentModal)
                    {
                        // Open project modal
                        case "openProjectModal":
                            LoadProject(((Textbox)UI.Modals[Data.CurrentModal].Components["openProjectTextbox"]).Text);
                            Data.CurrentModal = null;
                            Data.LastModalExitCode = null;
                            break;
                        // Create project modal
                        case "newProjectModal":
                            string path = ((Textbox)UI.Modals[Data.CurrentModal].Components["newProjectTextbox1"]).Text;
                            string name = ((Textbox)UI.Modals[Data.CurrentModal].Components["newProjectTextbox2"]).Text;
                            CreateProject(path, name);
                            Data.CurrentModal = null;
                            Data.LastModalExitCode = null;
                            break;
                    }
                }
            }
            // Check the shortcut for game building
            if (IsKeyPressed(KeyboardKey.F5) && Data.CurrentProject is not null)
            {
                BuildProject(Data.CurrentProject.Path);
            }
        }

        /// <summary>
        /// Draw and manage the files in the bottom container
        /// </summary>
        /// <param name="files">All the files in the asset directory</param>
        public void DrawManagerFiles(ref List<UStorage> files, ref int index)
        {
            if (files.Count != 0)
            {
                string directory = ((Container)UI.Components["fileManager"]).GetLastFile().Split("\\")[0];
                string aimedDirectory = files[0].Path.Split("\\")[0];
                string name = ((Container)UI.Components["fileManager"]).GetLastFile().Split("\\").Last();

                // Check if there needs to be a recheck for the files
                if (directory == aimedDirectory)
                {
                    if (!files.Exists(e => e.Path.EndsWith(name)))
                    {
                        string extension = ((Container)UI.Components["fileManager"]).GetLastFile().Split('.').Last();
                        if (extension == ((Container)UI.Components["fileManager"]).ExtensionFile)
                        {
                            files.Add(new UFile(((Container)UI.Components["fileManager"]).GetLastFile()));
                        }
                    }
                }

                for (int i = 0; i < files.Count; i++)
                {
                    // Check if the passed unit storage is a folder or not
                    int positionX = (i + 9) % 8;
                    if (positionX == 0) _ = 8;
                    int xPos = UI.Components["fileManager"].X + 150 * (index + 1) - 100;
                    int yPos = UI.Components["fileManager"].Y + 60;

                    // Shorten the text
                    string lbl = "";
                    if (files[i].Name.Length >= 10) { lbl = files[i].Name.Remove(5) + "..."; }
                    else lbl = files[i].Name;
                    if (files[i] is UFile)
                    {
                        DrawPanel(new Panel(xPos, yPos, 1, 0, fileTex, ""));
                        DrawLabel(new Label(xPos + 10 - lbl.Length / 2, yPos + fileTex.Height + 20, lbl), UI.Font);
                    }
                    else if (files[i] is UFolder)
                    {
                        DrawPanel(new Panel(xPos, yPos, 1, 0, folderTex, ""));
                        DrawLabel(new Label(xPos + 10 + files[i].Name.Length / 3, yPos + fileTex.Height + 20, lbl), UI.Font);
                    }

                    Vector2 mouse = GetMousePosition();
                    if (mouse.X < xPos + fileTex.Width && mouse.X > xPos && mouse.Y < yPos + fileTex.Height && mouse.Y > yPos)
                    {
                        if (IsMouseButtonDown(MouseButton.Left))
                        {
                            Data.SelectedFile = files[i].Path;
                            SetMouseCursor(MouseCursor.PointingHand);
                        }
                        if (IsMouseButtonPressedRepeat(MouseButton.Left))
                        {
                            if (files[i] is UFolder) Console.WriteLine("Hey you have won.. greate haha");
                        }
                        if (IsMouseButtonPressed(MouseButton.Middle))
                        {
                            File.Delete(files[i].Path);
                            switch (files[i].Path.Split('.').Last())
                            {
                                case "m3d":
                                    Ressource.DeleteModel(files[i].Name);
                                    break;
                                case "png":
                                    Ressource.DeleteTexture(files[i].Name);
                                    break;
                                case "wav":
                                    Ressource.DeleteSound(files[i].Name);
                                    break;
                                case "cs":
                                    break;
                            }
                            files.Remove(files[i]);
                        }
                    }
                    if (Data.SelectedFile is not null)
                    {
                        if (IsMouseButtonReleased(MouseButton.Left))
                        {
                            // Import model into the scene
                            if (mouse.X > UI.Components["gameManager"].X + UI.Components["gameManager"].Width + 10 && mouse.Y < UI.Components["fileManager"].Y - 10 && Data.SelectedFile.Split('.').Last() == "m3d")
                            {
                                string modelKey = Data.SelectedFile.Split('/').Last().Split('.')[0];

                                if (Ressource.ModelExists(modelKey))
                                {
                                    currentScene.AddGameObject(new UModel("[New model]", envCamera.Position + GetCameraForward(ref envCamera) * 5, Ressource.GetModel(modelKey).Meshes[0], modelKey));
                                }
                                else
                                {
                                    Model m = LoadModel(Data.SelectedFile);
                                    for (int j = 0; j < m.Meshes[0].VertexCount * 4; j++)
                                        m.Meshes[0].Colors[j] = 255;
                                    UpdateMeshBuffer(m.Meshes[0], 3, m.Meshes[0].Colors, m.Meshes[0].VertexCount * 4, 0);

                                    Ressource.AddModel(m, modelKey);
                                    currentScene.AddGameObject(new UModel("[New model]", envCamera.Position + GetCameraForward(ref envCamera) * 5, Ressource.GetModel(modelKey).Meshes[0], modelKey));
                                }
                                selectedElement = currentScene.GameObjects.Last();
                            }
                            // Import texture in game object attributes
                            else if (mouse.X > UI.Components["gameManager"].X + 100 && mouse.X < UI.Components["gameManager"].X + 350 &&
                                mouse.Y > UI.Components["gameManager"].Y + UI.Components["gameManager"].Height / 2 + 300
                                && mouse.Y < UI.Components["gameManager"].Y + UI.Components["gameManager"].Height / 2 + 320
                                && Data.SelectedFile.Split('.').Last() == "png")
                            {
                                if (selectedElement != null)
                                {
                                    string dictionaryKey = Data.SelectedFile.Split('/').Last().Split('.')[0];
                                    ((UModel)currentScene.GameObjects.ElementAt(currentScene.GameObjects.IndexOf(selectedElement))).SetTexture(dictionaryKey, Ressource.GetTexture(dictionaryKey));
                                }
                            }
                            Data.SelectedFile = null;
                        }
                    }
                    index++;
                }
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
                if (directory is not null) LoadAssets(directory);
                else throw new Exception($"Could not load the assets of project {path}");

                // Change to default assets page of the manager container
                ((Container)UI.Components["fileManager"]).ExtensionFile = "m3d";
                ((Container)UI.Components["fileManager"]).Tag = "models";
                ((Container)UI.Components["fileManager"]).OutputFilePath = directory + "/assets/models";
                ((Label)UI.Components["ressourceInfoLabel"]).Text = "File type : .m3d";

                // Load scenes along with their game objects
                Data.CurrentProject = new Project(project_name, path, LoadScenes(directory));
                currentScene = Data.CurrentProject.GetScene(0);
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
            if (Data.CurrentProject != null)
            {
                string[] jsons = JsonfyGos(currentScene.GameObjects);
                string? path;
                if (Data.CurrentProject.Path.Contains('.'))
                {
                    path = Path.GetDirectoryName(Data.CurrentProject.Path);
                }
                else
                {
                    path = Data.CurrentProject.Path;
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
                Data.CurrentModal = "newProjectModal";
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
                Data.CurrentProject = new Project(name, path + "\\" + name + ".uproj", scenes);
                Ressource = new Ressource();
                currentScene = Data.CurrentProject.GetScene(0);
                selectedElement = null;

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
        /// Load assets from given project file path (Textures and sounds to the ressource dictionary, and the models as a string path list)
        /// </summary>
        /// <param name="projectPath">File path to the project</param>
        public void LoadAssets(string projectPath)
        {
            // Load data paths from directories
            byte[] modelPath = Encoding.UTF8.GetBytes(projectPath + "/assets/models");
            byte[] texturePath = Encoding.UTF8.GetBytes(projectPath + "/assets/textures");
            byte[] soundPath = Encoding.UTF8.GetBytes(projectPath + "/assets/sounds");
            byte[] animationPath = Encoding.UTF8.GetBytes(projectPath + "/assets/animations");
            byte[] scriptPath = Encoding.UTF8.GetBytes(projectPath + "/assets/scripts");
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
                        else storage = new UFolder(path, LoadFolderArchitecture(path));
                        switch (i)
                        {
                            case 0:
                                modelsPathList.Add(storage); break;
                            case 1:
                                texturesPathList.Add(storage); break;
                            case 2:
                                soundsPathList.Add(storage); break;
                            case 3:
                                animationsPathList.Add(storage); break;
                            case 4:
                                scriptPathList.Add(storage); break;
                        }
                    }
                    UnloadDirectoryFiles(pathList);
                }
                Ressource = new Ressource(
                    texturesPathList,
                    soundsPathList,
                    modelsPathList
                    );
            }
            Console.WriteLine(Ressource.ToString());
        }
        /// <summary>
        /// Load files complete architecture of a specified directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<UStorage> LoadFolderArchitecture(string path)
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
                else storage = new UFolder(filePath, LoadFolderArchitecture(filePath));
                files.Add(storage);
            }
            UnloadDirectoryFiles(list);
            return files;
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
                position.X += (GetCameraRight(ref envCamera) * GetMouseDelta().X / 500 * Vector3Distance(position, envCamera.Position)).X;
            }
            else if (IsKeyDown(KeyboardKey.Y))
            {
                position.Z += (GetCameraForward(ref envCamera) * GetMouseDelta().X / 500 * Vector3Distance(position, envCamera.Position)).X;
            }
            else if (IsKeyDown(KeyboardKey.Z))
            {
                position.Y -= (GetCameraUp(ref envCamera) * GetMouseDelta().Y / 500 * Vector3Distance(position, envCamera.Position)).Y;
            }
            else
            {
                position += GetCameraRight(ref envCamera) * GetMouseDelta().X / 500 * Vector3Distance(position, envCamera.Position);
                position -= GetCameraUp(ref envCamera) * GetMouseDelta().Y / 500 * Vector3Distance(position, envCamera.Position);
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
        /// Check if a collision occurs between the mouse (screen) and an object of the world (Game object)
        /// </summary>
        /// <param name="go">Game object</param>
        /// <param name="mesh">Mesh</param>
        /// <param name="mousePos">2-Dimensional position of the mouse</param>
        /// <returns></returns>
        public int CheckCollisionScreenToWorld(GameObject3D go, Mesh mesh, Vector2 mousePos, Matrix4x4 transform)
        {
            if (mousePos.X > UI.Components["gameManager"].X + UI.Components["gameManager"].Width && mousePos.Y < UI.Components["fileManager"].Y - 10 && IsMouseButtonPressed(MouseButton.Left))
            {

                goCollision = GetRayCollisionMesh(mouseRay, mesh, transform);
                if (goCollision.Hit)
                {
                    return currentScene.GameObjects.IndexOf(go);
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
        /// <summary>
        /// Check if a mouse button has been pressed repeateadly
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool IsMouseButtonPressedRepeat(MouseButton button)
        {
            double currentFrame = GetTime();

            if (IsMouseButtonPressed(button))
            {
                if ((currentFrame - lastTimeButtonPressed) <= 0.3)
                {
                    lastTimeButtonPressed = 0.0;
                    return true;
                }
                lastTimeButtonPressed = currentFrame;
                return false;
            }
            return false;
        }
    }
}