﻿using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;
using Raylib_cs;
using static RayGUI_cs.RayGUI;
using RayGUI_cs;
using System.Numerics;
using System.Text;
using Newtonsoft.Json;
using System;

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
        private int hWindow;

        private int wWindow;

        private bool openModalOpenProject;

        private bool openModalNewProject;

        private Project? currentProject;

        private Font baseFont;

        private Container fileManager;

        private Container gameManager;

        private Container modalOpenProject;

        private Container modalNewProject;

        private Textbox openProjTxb;

        private Textbox newProjTxb;

        private Textbox newProjNameTxb;

        private Button closeModal;

        private Button okModalOpen;

        private Button okModalNew;

        private List<Textbox> textboxes;

        private List<Button> buttons;

        private List<Label> labels;

        private Scene currentScene;

        // 3D related attributes

        private List<string> modelsPathList;

        private List<string> texturesPathList;

        private List<string> soundsPathList;

        private List<string> animationsPathList;

        private List<string> scriptPathList;

        private GameObject3D? selectedElement;

        private string? selectedFile;

        private Camera3D envCamera;

        // Collision related variables

        private Ray mouseRay;

        private RayCollision xCollision;

        private RayCollision yCollision;

        private RayCollision zCollision;

        private RayCollision goCollision;

        public List<Button> Buttons { get { return buttons; } }
        public Container GameManager { get { return gameManager; } set { gameManager = value; } }
        public Container FileManager { get { return fileManager; } set { fileManager = value; } }
        public Camera3D EnvCamera { get { return envCamera; } set { envCamera = value; } }
        public Scene CurrentScene { get { return currentScene; } }
        public Project CurrentProject { get { return currentProject; } }

        /// <summary>
        /// Construct UI
        /// </summary>
        public Uniray(int WWindow, int HWindow, Font font, Scene scene)
        {
            wWindow = WWindow;
            hWindow = HWindow;
            baseFont = font;
            currentScene = scene;
            envCamera = new Camera3D();

            selectedElement = null;
            selectedFile = null;
            openModalOpenProject = false;
            openModalNewProject = false;
            currentProject = null;
            mouseRay = new Ray();
            xCollision = new RayCollision();
            yCollision = new RayCollision();
            zCollision = new RayCollision();
            goCollision = new RayCollision();
            cameraCollision = new RayCollision();

            // Instantiate lists of components
            textboxes = new List<Textbox>();
            buttons = new List<Button>();
            labels = new List<Label>();

            modelsPathList = new List<string>();
            texturesPathList = new List<string>();
            soundsPathList = new List<string>();
            animationsPathList = new List<string>();
            scriptPathList = new List<string>();

            // Load camera model
            cameraModel = LoadModel("data/camera.m3d");

            // Containers
            float cont1X = wWindow - wWindow / 1.25f;
            float cont1Y = hWindow - hWindow / 3;
            fileManager = new Container((int)cont1X, (int)cont1Y, wWindow - (int)cont1X - 10, hWindow - (int)cont1Y - 10, APPLICATION_COLOR, FOCUS_COLOR, "models");
            fileManager.Type = ContainerType.FileDropper;
            fileManager.ExtensionFile = "m3d";

            gameManager = new Container(10, 10, (int)cont1X - 20, hWindow - 20, APPLICATION_COLOR, FOCUS_COLOR, "gameManager");

            Container modal_template = new Container(WWindow / 2 - WWindow / 6, HWindow / 2 - HWindow / 6, WWindow / 3, HWindow / 3, APPLICATION_COLOR, FOCUS_COLOR, "modal");

            modalOpenProject = modal_template;

            modalNewProject = modal_template;

            // Buttons
            Button modelsButton = new Button("Models", (int)fileManager.X + 48, (int)fileManager.Y, 60, 25, APPLICATION_COLOR, FOCUS_COLOR, "modelsSection");
            buttons.Add(modelsButton);

            Button texturesButton = new Button("Textures", (int)fileManager.X + 164, (int)fileManager.Y, 60, 25, APPLICATION_COLOR, FOCUS_COLOR, "texturesSection");
            buttons.Add(texturesButton);

            Button soundsButton = new Button("Sounds", (int)fileManager.X + 260, (int)fileManager.Y, 60, 25, APPLICATION_COLOR, FOCUS_COLOR, "soundsSection");
            buttons.Add(soundsButton);

            Button animationsButton = new Button("Animations", (int)fileManager.X + 392, (int)fileManager.Y, 60, 25, APPLICATION_COLOR, FOCUS_COLOR, "animationsSections");
            buttons.Add(animationsButton);

            Button scriptsButton = new Button("Scripts", (int)fileManager.X + 492, (int)fileManager.Y, 60, 25, APPLICATION_COLOR, FOCUS_COLOR, "scriptsSections");
            buttons.Add(scriptsButton);

            Button openFileButton = new Button("Open", (int)fileManager.X + fileManager.Width - 38, (int)fileManager.Y + 5, 40, 20, APPLICATION_COLOR, FOCUS_COLOR, "openExplorer") { Type = ButtonType.PathFinder };
            buttons.Add(openFileButton);

            Button openProjectButton = new Button("Open project", (int)fileManager.X + fileManager.Width - 175, (int)fileManager.Y + 5, 125, 20, APPLICATION_COLOR, FOCUS_COLOR, "openProject");
            buttons.Add(openProjectButton);

            Button newProjectButton = new Button("New project", (int)fileManager.X + fileManager.Width - 303, (int)fileManager.Y + 5, 50, 20, APPLICATION_COLOR, FOCUS_COLOR, "newProject");
            buttons.Add(newProjectButton);

            Button playButton = new Button("Play", (GetScreenWidth() - gameManager.Width - 20) / 2 + gameManager.Width + 20, 10, 100, 30, APPLICATION_COLOR, FOCUS_COLOR, "play");
            buttons.Add(playButton);

            Button addGameObject = new Button("+", (int)gameManager.X + gameManager.Width - 20, (int)gameManager.Y + 10, 10, 15, APPLICATION_COLOR, FOCUS_COLOR, "addGameObject");
            buttons.Add(addGameObject);

            closeModal = new Button("x", (int)modal_template.X + modal_template.Width - 30, (int)modal_template.Y + 10, 20, 20, Color.Red, FOCUS_COLOR, "closeModal");
            
            okModalOpen = new Button("Proceed", (int)modal_template.X + modal_template.Width - 70, (int)modal_template.Y + modal_template.Height - 30, 60, 20, Color.Lime, FOCUS_COLOR, "okModalOpen");

            okModalNew = new Button("Proceed", (int)modal_template.X + modal_template.Width - 70, (int)modal_template.Y + modal_template.Height - 30, 60, 20, Color.Lime, FOCUS_COLOR, "okModalNew");

            // Textboxes
            Textbox goTexture = new Textbox("",  (int)gameManager.X + 100, (int)gameManager.Y + gameManager.Height / 2 + 290, gameManager.Width - 120, 40, APPLICATION_COLOR, FOCUS_COLOR);
            textboxes.Add(goTexture);

            openProjTxb = new Textbox("", (int)modal_template.X + 20, (int)modal_template.Y + 70, 250, 20, APPLICATION_COLOR, FOCUS_COLOR);

            newProjTxb = new Textbox("", (int)modal_template.X + 20, (int)modal_template.Y + 70, 250, 20, APPLICATION_COLOR, FOCUS_COLOR);

            newProjNameTxb = new Textbox("", (int)modal_template.X + 20, (int)modal_template.Y + 120, 250, 20, APPLICATION_COLOR, FOCUS_COLOR);

            // Labels
            Label gameLayersLabel = new Label((int)gameManager.X + 10, (int)gameManager.Y + 10, "Game Layers");
            labels.Add(gameLayersLabel);

            Label goTextureLabel = new Label((int)gameManager.X + 20, (int)gameManager.Y + gameManager.Height / 2 + 300, "Texture");
            labels.Add(goTextureLabel);

            Label gameObjectLabel = new Label((int)gameManager.X + 10, (int)gameManager.Y + hWindow / 2, "Game Object");
            labels.Add(gameObjectLabel);

            Label fileType = new Label((int)fileManager.X + (int)fileManager.Width / 2, (int)fileManager.Y + (int)fileManager.Height / 2, "File type : .m3d");
            labels.Add(fileType);
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
                    DrawModel(((UModel)go).Model, go.Position, 1, Color.White);
                    if (index == -1) index = CheckCollisionScreenToWorld(go, ((UModel)go).Model, mousePos);
                }
                else if (go is UCamera)
                {
                    DrawModel(cameraModel, go.Position, 1, Color.White);
                    if (index == -1) index = CheckCollisionScreenToWorld(go, cameraModel, mousePos);
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
                            Matrix4x4 t = RotateObject(((UModel)selectedElement).Yaw, ((UModel)selectedElement).Pitch, ((UModel)selectedElement).Roll, ((UModel)selectedElement).Model.Transform);
                            model.SetTransform(t);
                            
                        }
                        else if (selectedElement is UCamera)
                        {
                            UCamera camera = (UCamera)currentScene.GameObjects.ElementAt(currentScene.GameObjects.IndexOf(selectedElement));
                            cameraModel.Transform = RotateObject(((UCamera)selectedElement).Yaw, ((UCamera)selectedElement).Pitch, ((UCamera)selectedElement).Roll, cameraModel.Transform);
                        }
                        HideCursor();
                    }
                    else ShowCursor();
                }
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
            DrawRectangle(0, 0, (int)(wWindow - wWindow / 1.25f), hWindow, new Color(20, 20, 20, 255));
            DrawRectangle(0, hWindow - hWindow / 3 - 10, wWindow, hWindow - (hWindow - hWindow / 3) + 10, new Color(20, 20, 20, 255));

            bool focus = false;

            DrawContainer(ref gameManager);

            string new_File = "";            
            new_File = DrawContainer(ref fileManager);
            new_File = new_File.Replace('\\', '/');
            if (new_File != "" && currentProject is not null)
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

            foreach (Label label in labels) { DrawLabel(label, baseFont); }
            foreach (Button button in buttons)
            {
                DrawButton(button, baseFont);
                if (Hover(button.X, button.Y, button.Width, button.Height)) { focus = true; }
            }switch (fileManager.Tag)
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

            for (int i = 0; i < textboxes.Count; i++) 
            {
                Textbox textbox = textboxes[i];
                textboxes[i] = DrawTextbox(ref textbox, baseFont);
                if (Hover(textbox.X, textbox.Y, textbox.Width, textbox.Height)) { focus = true; }
                if (Hover(newProjTxb.X, newProjTxb.Y, newProjTxb.Width, newProjTxb.Height)) { focus = true; }
                if (Hover(openProjTxb.X, openProjTxb.Y, openProjTxb.Width, openProjTxb.Height)) { focus = true; }
                if (Hover(newProjNameTxb.X, newProjNameTxb.Y, newProjNameTxb.Width, newProjNameTxb.Height)) { focus = true; }
            }

            if (!focus && selectedFile is null) { SetMouseCursor(MouseCursor.Default); }

            if (openModalOpenProject)
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
            }

            if (openModalNewProject)
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
            }
            if (IsKeyPressed(KeyboardKey.F5) && currentProject is not null)
            {
                BuildProject(currentProject.Path);
            }

            // =========================================================================================================================================================
            // ============================================================= MANAGE CUSTOM BUTTONS =====================================================================
            // =========================================================================================================================================================
            foreach (Button section in buttons)
            {
                if (section.Type == ButtonType.Custom)
                {
                    if (IsButtonPressed(section))
                    {
                        Container c = FileManager;
                        Label fileType = new ((int)fileManager.X + (int)fileManager.Width / 2, (int)fileManager.Y + (int)fileManager.Height / 2, "");
                        switch (section.Tag)
                        {
                            case "modelsSection":
                                c.ExtensionFile = "m3d";
                                c.Tag = "models";
                                if (currentProject is not null) c.OutputFilePath = Path.GetDirectoryName(currentProject.Path) + "/assets/models";
                                fileType.Text = "File type : .m3d";
                                labels.RemoveAt(labels.IndexOf(labels.Last()));
                                labels.Add(fileType);
                                break;
                            case "texturesSection":
                                c.ExtensionFile = "png";
                                c.Tag = "textures";
                                if (currentProject is not null) c.OutputFilePath = Path.GetDirectoryName(currentProject.Path) + "/assets/textures";
                                fileType.Text = "File type : .png";
                                labels.RemoveAt(labels.IndexOf(labels.Last()));
                                labels.Add(fileType);
                                break;
                            case "soundsSection":
                                c.ExtensionFile = "wav";
                                c.Tag = "sounds";
                                if (currentProject is not null) c.OutputFilePath = Path.GetDirectoryName(currentProject.Path) + "/assets/sounds";
                                fileType.Text = "File type : .wav";
                                labels.RemoveAt(labels.IndexOf(labels.Last()));
                                labels.Add(fileType);
                                break;
                            case "animationsSections":
                                c.ExtensionFile = "m3d";
                                c.Tag = "animations";
                                if (currentProject is not null) c.OutputFilePath = Path.GetDirectoryName(currentProject.Path) + "/assets/animations";
                                fileType.Text = "File type : .m3d";
                                labels.RemoveAt(labels.IndexOf(labels.Last()));
                                labels.Add(fileType);
                                break;
                            case "scriptsSections":
                                c.ExtensionFile = "cs";
                                c.Tag = "scripts";
                                if (currentProject is not null) c.OutputFilePath = Path.GetDirectoryName(currentProject.Path) + "/assets/scripts";
                                fileType.Text = "File type : .cs";
                                labels.RemoveAt(labels.IndexOf(labels.Last()));
                                labels.Add(fileType);
                                break;
                            case "play":
                                if (currentProject is not null)
                                {
                                    BuildProject(currentProject.Path);
                                }
                                break;
                            case "openProject":
                                openModalOpenProject = true;
                                break;
                            case "newProject":
                                openModalNewProject = true;
                                break;
                        }
                        FileManager = c;
                    }
                }
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
                string directory = fileManager.Files.Last().Split("\\")[0];
                string aimedDirectory = files[0].Split("\\")[0];
                string name = fileManager.Files.Last().Split("\\").Last();

                // Check if there needs to be a recheck for the files
                if (directory == aimedDirectory)
                {
                    if (!files.Exists(e => e.EndsWith(name)))
                    {
                        string extension = fileManager.Files.Last().Split('.').Last();
                        if (extension == fileManager.ExtensionFile)
                        {
                            files.Add(fileManager.Files.Last());
                        }
                    }
                }

                for (int i = 0; i < files.Count; i++)
                {
                    int positionX = (i + 9) % 8;
                    if (positionX == 0) _ = 8;
                    int xPos = (int)fileManager.X + 150 * (i + 1) - 100;
                    int yPos = (int)fileManager.Y + 60;
                    DrawPanel(new Panel(xPos, yPos, 1, 0, fileTex, ""));
                    string[] pathArryBySlash = files[i].Split('/');
                    DrawLabel(new Label(xPos, yPos + fileTex.Height + 20, pathArryBySlash.Last()), baseFont);

                    Vector2 mouse = GetMousePosition();
                    if (mouse.X < xPos + fileTex.Width && mouse.X > xPos && mouse.Y < yPos + fileTex.Height && mouse.Y > yPos)
                    {
                        if (IsMouseButtonDown(MouseButton.Left))
                        {
                            selectedFile = files[i];
                            SetMouseCursor(MouseCursor.PointingHand);
                        }
                        if (IsMouseButtonPressed(MouseButton.Middle))
                        {
                            File.Delete(files[i]);
                            File.Delete("..\\..\\..\\" + files[i]);
                            files.Remove(files[i]);
                        }
                    }
                    if (selectedFile is not null)
                    {
                        if (IsMouseButtonReleased(MouseButton.Left))
                        {
                            // Import model into the scene
                            if (mouse.X > gameManager.X + gameManager.Width + 10 && mouse.Y < fileManager.Y - 10 && selectedFile.Split('.').Last() == "m3d")
                            {
                                Model m = LoadModel(selectedFile);
                                for (int j = 0; j < m.Meshes[0].VertexCount * 4; j++)
                                    m.Meshes[0].Colors[j] = 255;
                                UpdateMeshBuffer(m.Meshes[0], 3, m.Meshes[0].Colors, m.Meshes[0].VertexCount * 4, 0);
                                currentScene.AddGameObject(new UModel("[New model]", envCamera.Position + GetCameraForward(ref envCamera) * 5, m, selectedFile));
                                selectedElement = currentScene.GameObjects.Last();
                            }
                            // Import texture in game object attributes
                            else if (mouse.X > gameManager.X + 100 && mouse.X < gameManager.X + 350 && mouse.Y > gameManager.Y + gameManager.Height / 2 + 300 && mouse.Y < gameManager.Y + gameManager.Height / 2 + 320 && selectedFile.Split('.').Last() == "png")
                            {
                                if (selectedElement != null)
                                {
                                    string dictionaryKey = selectedFile.Split('/').Last().Split('.')[0];
                                    ((UModel)currentScene.GameObjects.ElementAt(currentScene.GameObjects.IndexOf(selectedElement))).SetTexture(dictionaryKey, Ressource.GetTexture(dictionaryKey));
                                }
                            }
                            selectedFile = null;
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
            /*try
            {*/
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
                Label fileType = new Label((int)fileManager.X + (int)fileManager.Width / 2, (int)fileManager.Y + (int)fileManager.Height / 2, "");
                fileManager.ExtensionFile = "m3d";
                fileManager.Tag = "models";
                fileManager.OutputFilePath = directory + "/assets/models";
                fileType.Text = "File type : .m3d";
                labels.RemoveAt(labels.IndexOf(labels.Last()));
                labels.Add(fileType);

                // Load scenes along with their game objects
                currentProject = new Project(project_name, path, LoadScenes(directory));
                currentScene = currentProject.GetScene(0);
                SetWindowTitle("Uniray - " + project_name);

                TraceLog(TraceLogLevel.Info, "Project has been loaded successfully !");
            //}
            //catch
            //{
              //  TraceLog(TraceLogLevel.Warning, "Project could not be loaded !");
            //}
        }

        /// <summary>
        /// Save current game objects and scenes
        /// </summary>
        public void SaveProject()
        {
            if (currentProject != null)
            {
                string[] jsons = JsonfyGos(currentScene.GameObjects);
                string? path;
                if (currentProject.Path.Contains('.'))
                {
                    path = Path.GetDirectoryName(currentProject.Path);
                }
                else
                {
                    path = currentProject.Path;
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
                fileManager.OutputFilePath = path + "\\assets\\models\\";

                StreamWriter cs = new StreamWriter(path + "\\scenes\\new_scene\\camera.json");
                string newCam = "[{" + "X: " + 0 + ",Y: " + 0 + ",Z: " + 0 + ",Yaw: " + 0 +
                    ",Pitch: " + 0 + ",Roll: " + 0 + "}]";
                cs.Write(newCam);
                cs.Close();

                SetWindowTitle("Uniray - " + name);

                Camera3D camera = new()
                {
                    Position = Vector3.Zero,
                    Target = Vector3.Zero,
                    Up = Vector3.UnitY,
                    Projection = CameraProjection.Perspective,
                    FovY = 90
                };
                UCamera ucamera = new UCamera("Camera", camera);

                Scene defaultScene = new(new List<GameObject3D> { ucamera });
                List<Scene> scenes = new() { defaultScene };
                currentProject = new Project(name, path + "\\" + name + ".uproj", scenes);
                Ressource = new Ressource();
                currentScene = currentProject.GetScene(0);
                selectedElement = null;

                TraceLog(TraceLogLevel.Warning, "Project \"" + name + "\" has been created");
            }
            catch
            {
                TraceLog(TraceLogLevel.Warning, "Project " + name + " could not be created");
            }
        }

        /// <summary>
        /// String definition of the uniray class
        /// </summary>
        /// <returns>Message</returns>
        public override string ToString()
        {
            return "Uniray is a game engine developped by Evan Comtesse";
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
                    ",Pitch: " + model.Pitch + ",Roll: " + model.Roll + ",ModelPath: \"" + model.ModelPath + "\",TextureID: \"" + model.TextureID + "\"},";
                }
                else if (go is UCamera camera)
                {
                    cameraJson += "{" + "X: " + camera.X + ",Y: " + camera.Y + ",Z: " + camera.Z + ",Yaw: " + camera.Yaw +
                    ",Pitch: " + camera.Pitch + ",Roll: " + camera.Roll + "},";
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
                foreach (UCamera u in ucameras) Console.WriteLine(u.ToString());
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
                        if (go.ModelPath != "" && go.ModelPath is not null)
                        {
                            Model m = LoadModel(go.ModelPath);
                            for (int j = 0; j < m.Meshes[0].VertexCount * 4; j++)
                                m.Meshes[0].Colors[j] = 255;
                            UpdateMeshBuffer(m.Meshes[0], 3, m.Meshes[0].Colors, m.Meshes[0].VertexCount * 4, 0);
                            go.Model = m;
                            if (go.TextureID != "") go.SetTexture(go.TextureID, Ressource.GetTexture(go.TextureID));
                        }
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
                    soundsPathList
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
        /// <param name="model">Model</param>
        /// <param name="mousePos">2-Dimensional position of the mouse</param>
        /// <returns></returns>
        public int CheckCollisionScreenToWorld(GameObject3D go, Model model, Vector2 mousePos)
        {
            if (mousePos.X > gameManager.X + gameManager.Width && mousePos.Y < fileManager.Y - 10 && IsMouseButtonPressed(MouseButton.Left))
            {
                BoundingBox box = GetModelBoundingBox(model);
                box.Min += go.Position;
                box.Max += go.Position;

                goCollision = GetRayCollisionBox(mouseRay, box);
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
        /// Rotate a 3-Dimensional model according to the X/Y/Z keys
        /// </summary>
        /// <param name="yaw">Y axis angle</param>
        /// <param name="pitch">X axis angle</param>
        /// <param name="roll">Z axis angle</param>
        /// <param name="transform">4x4 matrix to transform</param>
        /// <returns></returns>
        public Matrix4x4 RotateObject(float yaw, float pitch, float roll, Matrix4x4 transform)
        {
            if (IsKeyDown(KeyboardKey.X))
            {
                Matrix4x4 newTransform = MatrixRotateX(pitch / RAD2DEG);
                Matrix4x4 resultTransform = MatrixMultiply(newTransform, transform);

                return resultTransform;
            }
            else if (IsKeyDown(KeyboardKey.Z))
            {
                Matrix4x4 newTransform = MatrixRotateY(yaw / RAD2DEG);
                Matrix4x4 resultTransform = MatrixMultiply(newTransform, transform);

                return resultTransform;
            }
            else if (IsKeyDown(KeyboardKey.Y))
            {
                Matrix4x4 newTransform = MatrixRotateZ(roll / RAD2DEG);
                Matrix4x4 resultTransform = MatrixMultiply(newTransform, transform);

                return resultTransform;
            }
            else
            {
                return transform;
            }
        }
    }
}