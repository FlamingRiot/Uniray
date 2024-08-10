﻿using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;
using Raylib_cs;
using static RayGUI_cs.RayGUI;
using RayGUI_cs;
using System.Numerics;
using System.Text;
using Newtonsoft.Json;
using Uniray._2D;
using Uniray.Managment;

namespace Uniray
{
    public unsafe partial struct Uniray
    {
        public static readonly Color APPLICATION_COLOR = new Color(30, 30, 30, 255);
        public static readonly Color FOCUS_COLOR = new Color(60, 60, 60, 255);
        public Texture2D fileTex = LoadTexture("data/file.png");

        // =========================================================================================================================================================
        // ================================================================= SEMI-CONSTANTS ========================================================================
        // =========================================================================================================================================================

        public static Ressource Ressource;

        /// <summary>
        /// Directionnal arrows model loading
        /// </summary>
        public Model arrow = LoadModelFromMesh(GenMeshCylinder(0.05f, 0.7f, 20));
        /// <summary>
        /// 3D model of the displayed scene camera
        /// </summary>
        public Model cameraModel;
        /// <summary>
        /// Material of the generic camera model used for the application
        /// </summary>
        public Material cameraMaterial;
        /// <summary>
        /// Collision with the mouse and camera
        /// </summary>
        public RayCollision cameraCollision;
        /// <summary>
        /// Y arrow box
        /// </summary>
        public BoundingBox yArrowBox = new BoundingBox(new Vector3(-0.1f, 0f, -0.1f), new Vector3(0.1f, 1.4f, 0.1f));
        /// <summary>
        /// X arrow box
        /// </summary>
        public BoundingBox xArrowBox = new BoundingBox(new Vector3(-0.1f, -0.1f, -1.4f), new Vector3(0.1f, 0.1f, 0f));
        /// <summary>
        /// Z arrow box
        /// </summary>
        public BoundingBox zArrowBox = new BoundingBox(new Vector3(0f, -0.1f, -0.1f), new Vector3(1.4f, 0.1f, 0.1f));

        // 2D related attributes
        public UI UI;

        public static UData Data;

        private int hWindow;

        private int wWindow;

        private bool openModalOpenProject;

        private bool openModalNewProject;

        private Font baseFont;

       // private Container fileManager;

       // private Container gameManager;

        private Container modalOpenProject;

        private Container modalNewProject;

        private Textbox openProjTxb;

        private Textbox newProjTxb;

        private Textbox newProjNameTxb;

        private Button closeModal;

        private Button okModalOpen;

        private Button okModalNew;

        private ErrorHandler errorHandler;

        // 3D related attributes
        private Scene currentScene;

        private List<string> modelsPathList;

        private List<string> texturesPathList;

        private List<string> soundsPathList;

        private List<string> animationsPathList;

        private List<string> scriptPathList;

        private GameObject3D? selectedElement;

        private Camera3D envCamera;

        private RenderTexture2D cameraView;

        private Rectangle cameraViewRec;

        // Collision related variables

        private Ray mouseRay;

        private RayCollision xCollision;

        private RayCollision yCollision;

        private RayCollision zCollision;

        private RayCollision goCollision;
        public Camera3D EnvCamera { get { return envCamera; } set { envCamera = value; } }
        public Scene CurrentScene { get { return currentScene; } }
        /// <summary>
        /// Main constructor
        /// </summary>
        public Uniray(int WWindow, int HWindow, Font font, Scene scene)
        {
            wWindow = WWindow;
            hWindow = HWindow;
            baseFont = font;
            currentScene = scene;
            envCamera = new Camera3D();

            cameraView = LoadRenderTexture(WWindow / 2, HWindow / 2);
            cameraViewRec = new Rectangle(0, 0, WWindow / 2, -(HWindow / 2));

            selectedElement = null;
            openModalOpenProject = false;
            openModalNewProject = false;
            mouseRay = new Ray();
            xCollision = new RayCollision();
            yCollision = new RayCollision();
            zCollision = new RayCollision();
            goCollision = new RayCollision();
            cameraCollision = new RayCollision();

            modelsPathList = new List<string>();
            texturesPathList = new List<string>();
            soundsPathList = new List<string>();
            animationsPathList = new List<string>();
            scriptPathList = new List<string>();

            // Load camera model
            cameraModel = LoadModel("data/camera.m3d");
            cameraMaterial = LoadMaterialDefault();
            SetMaterialTexture(ref cameraMaterial, MaterialMapIndex.Diffuse, LoadTexture("data/cameraTex.png"));

            Data = new UData();
            UI = new UI(WWindow, HWindow, font);
            BuildUI(WWindow, HWindow, font);
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

            // Draw 3-Dimensional models of the current scene and check for a hypothetical new selected object
            int index = -1;
            foreach (GameObject3D go in currentScene.GameObjects)
            {
                // Manage objects drawing + object selection (according to the object type)
                if (go is UModel)
                {
                    //DrawModel(((UModel)go).Model, go.Position, 1, Color.White);
                    DrawMesh(((UModel)go).Mesh, ((UModel)go).Material, ((UModel)go).Transform);
                    if (index == -1) index = CheckCollisionScreenToWorld(go, ((UModel)go).Mesh, mousePos, ((UModel)go).Transform);
                }
                else if (go is UCamera)
                {
                    //DrawModel(cameraModel, go.Position, 1, Color.White);
                    DrawMesh(cameraModel.Meshes[0], cameraMaterial, ((UCamera)go).Transform);
                    if (index == -1) index = CheckCollisionScreenToWorld(go, cameraModel.Meshes[0], mousePos, ((UCamera)go).Transform);
                }
            }

            // Assign the newly selected object to the according variable
            if (index != -1)
            {
                selectedElement = currentScene.GetGameObject(index);
            }

            // Draw directional arrows
            if (selectedElement != null) 
            {
                if (IsMouseButtonDown(MouseButton.Left))
                {
                    xCollision = GetRayCollisionBox(mouseRay, new BoundingBox(xArrowBox.Min + selectedElement.Position, xArrowBox.Max + selectedElement.Position));
                    yCollision = GetRayCollisionBox(mouseRay, new BoundingBox(yArrowBox.Min + selectedElement.Position, yArrowBox.Max + selectedElement.Position));
                    zCollision = GetRayCollisionBox(mouseRay, new BoundingBox(zArrowBox.Min + selectedElement.Position, zArrowBox.Max + selectedElement.Position));

                    if (xCollision.Hit) { currentScene.GameObjects.ElementAt(currentScene.GameObjects.IndexOf(selectedElement)).Z -= GetMouseDelta().X / 100; }
                    if (yCollision.Hit) { currentScene.GameObjects.ElementAt(currentScene.GameObjects.IndexOf(selectedElement)).Y -= GetMouseDelta().Y / 100; }
                    if (zCollision.Hit) { currentScene.GameObjects.ElementAt(currentScene.GameObjects.IndexOf(selectedElement)).X -= GetMouseDelta().X / 100; }
                }

                DrawModelEx(arrow, selectedElement.Position, Vector3.UnitZ, 270, new Vector3(2), Color.Green);
                DrawModelEx(arrow, selectedElement.Position, Vector3.UnitY, 0, new Vector3(2), Color.Red);
                DrawModelEx(arrow, selectedElement.Position, Vector3.UnitX, 270, new Vector3(2), Color.Blue);

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
            if (selectedElement is UCamera)
            {
                EndMode3D();

                BeginTextureMode(cameraView);

                ClearBackground(new Color(70, 70, 70, 255));

                BeginMode3D(((UCamera)selectedElement).Camera);

                DrawGrid(10, 10);

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
                BuildUI(GetScreenWidth(), GetScreenHeight(), baseFont);
            }
            // Draw the outline rectangles that appear behind the main panels
            DrawRectangle(0, 0, (int)(wWindow - wWindow / 1.25f), hWindow, new Color(20, 20, 20, 255));
            DrawRectangle(0, hWindow - hWindow / 3 - 10, wWindow, hWindow - (hWindow - hWindow / 3) + 10, new Color(20, 20, 20, 255));

            UI.Draw();

            // Tick the error handler for the errors to potentially disappear
            errorHandler.Tick();

            string new_File = "";
            new_File = ((Container)UI.Components["fileManager"]).GetLastFile();
            new_File = new_File.Replace('\\', '/');
            if (new_File != "" && Data.CurrentProject is not null)
            {
                switch (new_File.Split('.').Last())
                {
                    case "m3d":
                        if (modelsPathList.Count == 0)
                        {
                            modelsPathList.Add(new_File);
                        }
                        else if (modelsPathList.Last() != new_File)
                        {
                            modelsPathList.Add(new_File);
                        }
                        break;
                    case "png":
                        if (texturesPathList.Count == 0)
                        {
                            texturesPathList.Add(new_File);
                            Ressource.AddTexture(LoadTexture(new_File), new_File.Split('/').Last().Split('.')[0]);
                        }
                        else if (texturesPathList.Last() != new_File)
                        {
                            texturesPathList.Add(new_File);
                            Ressource.AddTexture(LoadTexture(new_File), new_File.Split('/').Last().Split('.')[0]);
                        }
                        break;
                    case "wav":
                        break;
                    case "cs":
                        break;
                }
            }

            switch (UI.Components["fileManager"].Tag)
            {
                case "models":
                    DrawManagerFiles(ref modelsPathList);
                    break;
                case "textures":
                    DrawManagerFiles(ref texturesPathList);
                    break;
                case "sounds":
                    DrawManagerFiles(ref soundsPathList);
                    break;
                case "animations":
                    DrawManagerFiles(ref animationsPathList);
                    break;
                case "scripts":
                    DrawManagerFiles(ref scriptPathList);
                    break;
            }

            // Render the selected camera view to the top right corner of the screen
            if (selectedElement is UCamera)
            {
                DrawRectangleLinesEx(new Rectangle(wWindow - wWindow / 5 - 11, 9, wWindow / 5 + 2, hWindow / 5 + 2), 2, Color.White);
                DrawTexturePro(cameraView.Texture, cameraViewRec, new Rectangle(wWindow - wWindow / 5 - 10, 10, wWindow / 5, hWindow / 5), Vector2.Zero, 0, Color.White);
            }

            /*if (openModalOpenProject)
            {
                DrawRectanglePro(new Rectangle(0, 0, wWindow, hWindow), Vector2.Zero, 0, new Color(0, 0, 0, 75));
                DrawContainer(ref modalOpenProject);
                DrawLabel(new Label((int)modalOpenProject.X + 20, (int)modalOpenProject.Y + 50, "Copy .uproj file link"), baseFont);
                DrawTextbox(ref openProjTxb, baseFont);
                DrawButton(closeModal, baseFont);
                DrawButton(okModalOpen, baseFont);

                if (IsButtonPressed(closeModal)) 
                {
                    openModalOpenProject = false;
                }
                else if (IsButtonPressed(okModalOpen))
                {
                    openModalOpenProject = false;
                    LoadProject(openProjTxb.Text);
                }
            }*/
            // Draw the currently displayed modal and define its state
            if (Data.CurrentModal is not null)
            {
                UI.DrawModal(Data.CurrentModal);

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

            /*if (openModalNewProject)
            {
                DrawRectanglePro(new Rectangle(0, 0, wWindow, hWindow), Vector2.Zero, 0, new Color(0, 0, 0, 75));
                DrawContainer(ref modalNewProject);
                DrawLabel(new Label((int)modalNewProject.X + 20, (int)modalNewProject.Y + 50, "Copy target directory path"), baseFont);
                DrawLabel(new Label((int)modalNewProject.X + 20, (int)modalNewProject.Y + 100, "Create your project name"), baseFont);
                DrawTextbox(ref newProjTxb, baseFont);
                DrawTextbox(ref newProjNameTxb, baseFont);
                DrawButton(closeModal, baseFont);
                DrawButton(okModalNew, baseFont);

                if (IsButtonPressed(closeModal))
                {
                    openModalNewProject = false;
                }
                else if (IsButtonPressed(okModalNew))
                {
                    openModalNewProject = false;
                    string project_path = newProjTxb.Text;
                    string project_name = newProjNameTxb.Text;
                    CreateProject(project_path, project_name);
                }
            }*/
            if (IsKeyPressed(KeyboardKey.F5) && Data.CurrentProject is not null)
            {
                BuildProject(Data.CurrentProject.Path);
            }
        }

        /// <summary>
        /// Draw and manage the files in the bottom container
        /// </summary>
        /// <param name="files">All the files in the asset directory</param>
        public void DrawManagerFiles(ref List<string> files)
        {
            if (files.Count != 0)
            {
                string directory = ((Container)UI.Components["fileManager"]).GetLastFile().Split("\\")[0];
                string aimedDirectory = files[0].Split("\\")[0];
                string name = ((Container)UI.Components["fileManager"]).GetLastFile().Split("\\").Last();

                // Check if there needs to be a recheck for the files
                if (directory == aimedDirectory)
                {
                    if (!files.Exists(e => e.EndsWith(name)))
                    {
                        string extension = ((Container)UI.Components["fileManager"]).GetLastFile().Split('.').Last();
                        if (extension == ((Container)UI.Components["fileManager"]).ExtensionFile)
                        {
                            files.Add(((Container)UI.Components["fileManager"]).GetLastFile());
                        }
                    }
                }

                for (int i = 0; i < files.Count; i++)
                {
                    int positionX = (i + 9) % 8;
                    if (positionX == 0) _ = 8;
                    int xPos = UI.Components["fileManager"].X + 150 * (i + 1) - 100;
                    int yPos = UI.Components["fileManager"].Y + 60;
                    DrawPanel(new Panel(xPos, yPos, 1, 0, fileTex, ""));
                    string[] pathArryBySlash = files[i].Split('/');
                    DrawLabel(new Label(xPos, yPos + fileTex.Height + 20, pathArryBySlash.Last()), baseFont);

                    Vector2 mouse = GetMousePosition();
                    if (mouse.X < xPos + fileTex.Width && mouse.X > xPos && mouse.Y < yPos + fileTex.Height && mouse.Y > yPos)
                    {
                        if (IsMouseButtonDown(MouseButton.Left))
                        {
                            Data.SelectedFile = files[i];
                            SetMouseCursor(MouseCursor.PointingHand);
                        }
                        if (IsMouseButtonPressed(MouseButton.Middle))
                        {
                            File.Delete(files[i]);
                            File.Delete("..\\..\\..\\" + files[i]);
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
                openModalNewProject = true;
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
                        path = path.Replace("\\", "/");
                        switch (i)
                        {
                            case 0:
                                modelsPathList.Add(path); break;
                            case 1:
                                texturesPathList.Add(path); break;
                            case 2:
                                soundsPathList.Add(path); break;
                            case 3:
                                animationsPathList.Add(path); break;
                            case 4:
                                scriptPathList.Add(path); break;
                        }
                    }
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
        /// Build the 2-Dimensional UI according to the window size
        /// <param name="WWindow">Window width</param>
        /// <param name="HWindow">Window height</param>
        /// <param name="font">Used font for the UI</param>
        /// </summary>
        public void BuildUI(int wWindow, int hWindow, Font font)
        {

            modalOpenProject = (Container)UI.Components["modalTemplate"];

            modalNewProject = (Container)UI.Components["modalTemplate"];

            closeModal = new Button("x", (int)UI.Components["modalTemplate"].X + UI.Components["modalTemplate"].Width - 30, (int)UI.Components["modalTemplate"].Y + 10, 20, 20, Color.Red, FOCUS_COLOR, "closeModal");

            okModalOpen = new Button("Proceed", (int)UI.Components["modalTemplate"].X + UI.Components["modalTemplate"].Width - 70, (int)UI.Components["modalTemplate"].Y + UI.Components["modalTemplate"].Height - 30, 60, 20, Color.Lime, FOCUS_COLOR, "okModalOpen");

            okModalNew = new Button("Proceed", (int)UI.Components["modalTemplate"].X + UI.Components["modalTemplate"].Width - 70, (int)UI.Components["modalTemplate"].Y + UI.Components["modalTemplate"].Height - 30, 60, 20, Color.Lime, FOCUS_COLOR, "okModalNew");

            openProjTxb = new Textbox((int)UI.Components["modalTemplate"].X + 20, (int)UI.Components["modalTemplate"].Y + 70, 250, 20, "", APPLICATION_COLOR, FOCUS_COLOR);

            newProjTxb = new Textbox((int)UI.Components["modalTemplate"].X + 20, (int)UI.Components["modalTemplate"].Y + 70, 250, 20, "", APPLICATION_COLOR, FOCUS_COLOR);

            newProjNameTxb = new Textbox((int)UI.Components["modalTemplate"].X + 20, (int)UI.Components["modalTemplate"].Y + 120, 250, 20, "", APPLICATION_COLOR, FOCUS_COLOR);

            // Initialize the error handler
            errorHandler = new ErrorHandler(new Vector2((UI.Components["fileManager"].X + UI.Components["fileManager"].Width / 2) - 150, UI.Components["fileManager"].Y - 60), font);

            // Update the size variables if needed
            this.wWindow = wWindow;
            this.hWindow = hWindow;
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