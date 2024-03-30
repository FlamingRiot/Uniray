using static Raylib_cs.Raylib;
using Raylib_cs;
using static RayGUI_cs.RayGUI;
using RayGUI_cs;
using System.Numerics;
using System.Text;
using System.Diagnostics;

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

        /// <summary>
        /// Directionnal arrows model loading
        /// </summary>
        public Model arrow = LoadModelFromMesh(GenMeshCylinder(0.05f, 0.7f, 20));
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

        private bool openModal;

        private string? currentProject;

        private Font baseFont;

        private Container fileManager;

        private Container gameManager;

        private Container modal;

        private Textbox openProjTxb;

        private Button closeModal;

        private Button okModal;

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

        private GameObject? selectedElement;

        private string? selectedFile;

        private string? projectPath;

        // Collision related variables

        private Ray mouseRay;

        private RayCollision xCollision;

        private RayCollision yCollision;

        private RayCollision zCollision;

        private RayCollision goCollision;

        public List<Button> Buttons { get { return buttons; } }
        public Container GameManager { get { return gameManager; } set { gameManager = value; } }
        public Container FileManager { get { return fileManager; } set { fileManager = value; } }
        public Camera3D CurrentCamera { get { return currentScene.Camera; } set { currentScene.Camera = value; } }
        public Scene CurrentScene { get { return currentScene; } }

        /// <summary>
        /// Construct UI
        /// </summary>
        public Uniray(int WWindow, int HWindow, Font font, Scene scene)
        {
            wWindow = WWindow;
            hWindow = HWindow;
            baseFont = font;
            currentScene = scene;

            selectedElement = null;
            selectedFile = null;
            projectPath = null;
            openModal = false;
            currentProject = null;
            mouseRay = new Ray();
            xCollision = new RayCollision();
            yCollision = new RayCollision();
            zCollision = new RayCollision();
            goCollision = new RayCollision();

            // Instantiate lists of components
            textboxes = new List<Textbox>();
            buttons = new List<Button>();
            labels = new List<Label>();

            modelsPathList = new List<string>();
            texturesPathList = new List<string>();
            soundsPathList = new List<string>();
            animationsPathList = new List<string>();
            scriptPathList = new List<string>();

            // Load data paths from directories
            byte[] modelPath = Encoding.UTF8.GetBytes("assets/models");
            byte[] texturePath = Encoding.UTF8.GetBytes("assets/textures");
            byte[] soundPath = Encoding.UTF8.GetBytes("assets/sounds");
            byte[] animationPath = Encoding.UTF8.GetBytes("assets/animations");
            byte[] scriptPath = Encoding.UTF8.GetBytes("assets/scripts");
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
                        string path = new string((sbyte*)pathList.Paths[j]);
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
                Console.WriteLine(texturesPathList[0]);
            }

            // Containers
            float cont1X = wWindow - wWindow / 1.25f;
            float cont1Y = hWindow - hWindow / 3;
            fileManager = new Container((int)cont1X, (int)cont1Y, wWindow - (int)cont1X - 10, hWindow - (int)cont1Y - 10, APPLICATION_COLOR, FOCUS_COLOR, "models");
            fileManager.Type = ContainerType.FileDropper;
            fileManager.OutputFilePath = "assets/models/";
            fileManager.ExtensionFile = "m3d";

            gameManager = new Container(10, 10, (int)cont1X - 20, hWindow - 20, APPLICATION_COLOR, FOCUS_COLOR, "gameManager");

            modal = new Container(WWindow / 2 - WWindow / 6, HWindow / 2 - HWindow / 6, WWindow / 3, HWindow / 3, APPLICATION_COLOR, FOCUS_COLOR, "modal");

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

            closeModal = new Button("x", (int)modal.X + modal.Width - 30, (int)modal.Y + 10, 20, 20, Color.Red, FOCUS_COLOR, "closeModal");
            
            okModal = new Button("Proceed", (int)modal.X + modal.Width - 70, (int)modal.Y + modal.Height - 30, 60, 20, Color.Lime, FOCUS_COLOR, "okModal");

            // Textboxes
            Textbox goTexture = new Textbox("",  (int)gameManager.X + 100, (int)gameManager.Y + gameManager.Height / 2 + 290, gameManager.Width - 120, 40, APPLICATION_COLOR, FOCUS_COLOR);
            textboxes.Add(goTexture);

            openProjTxb = new Textbox("", (int)modal.X + 20, (int)modal.Y + 70, 250, 20, APPLICATION_COLOR, FOCUS_COLOR);

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

            if (selectedElement != null) { selectedElement = currentScene.GameObjects.ElementAt(currentScene.GameObjects.IndexOf(selectedElement)); }

            Vector2 mousePos = GetMousePosition();
            mouseRay = GetMouseRay(mousePos, CurrentCamera);

            int index = -1;
            foreach (GameObject go in currentScene.GameObjects)
            {
                DrawModelEx(go.Model, go.Position, go.Rotation, 0, go.Scale, Color.White);

                if (mousePos.X > gameManager.X + gameManager.Width && mousePos.Y < fileManager.Y - 10 && IsMouseButtonPressed(MouseButton.Left))
                {
                    BoundingBox box = GetModelBoundingBox(go.Model);
                    box.Min += go.Position;
                    box.Max += go.Position;

                    goCollision = GetRayCollisionBox(mouseRay, box);
                    if (goCollision.Hit)
                    {
                        index = currentScene.GameObjects.IndexOf(go);
                    }
                }
            }
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
            DrawContainer(ref fileManager);

            foreach (Label label in labels) { DrawLabel(label, baseFont); }
            foreach (Button button in buttons)
            {
                DrawButton(button, baseFont);
                if (Hover(button.X, button.Y, button.Width, button.Height)) { focus = true; }
            }
            switch (fileManager.Tag)
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
            }

            // Draw game layers infos
            foreach (GameObject go in CurrentScene.GameObjects)
            {
                DrawLabel(new Label((int)gameManager.X + 30, (int)gameManager.Y + 30 + CurrentScene.GameObjects.IndexOf(go) * 20, go.Name), baseFont);
            }
            if (!focus && selectedFile is null) { SetMouseCursor(MouseCursor.Default); }

            if (openModal)
            {
                DrawRectanglePro(new Rectangle(0, 0, wWindow, hWindow), Vector2.Zero, 0, new Color(0, 0, 0, 75));
                DrawContainer(ref modal);
                DrawLabel(new Label((int)modal.X + 20, (int)modal.Y + 50, "Copy .uproj file link"), baseFont);
                DrawTextbox(ref openProjTxb, baseFont);
                DrawButton(closeModal, baseFont);
                DrawButton(okModal, baseFont);

                if (IsButtonPressed(closeModal)) 
                {
                    openModal = false;
                }
                else if (IsButtonPressed(okModal))
                {
                    openModal = false;
                    currentProject = openProjTxb.Text.Split('\\').Last().Split('.')[0];
                }
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
                        Label fileType = new Label((int)fileManager.X + (int)fileManager.Width / 2, (int)fileManager.Y + (int)fileManager.Height / 2, "");
                        switch (section.Tag)
                        {
                            case "modelsSection":
                                c.ExtensionFile = "m3d";
                                c.Tag = "models";
                                c.OutputFilePath = "assets/models";
                                fileType.Text = "File type : .m3d";
                                labels.RemoveAt(labels.IndexOf(labels.Last()));
                                labels.Add(fileType);
                                break;
                            case "texturesSection":
                                c.ExtensionFile = "png";
                                c.Tag = "textures";
                                c.OutputFilePath = "assets/textures";
                                fileType.Text = "File type : .png";
                                labels.RemoveAt(labels.IndexOf(labels.Last()));
                                labels.Add(fileType);
                                break;
                            case "soundsSection":
                                c.ExtensionFile = "wav";
                                c.Tag = "sounds";
                                c.OutputFilePath = "assets/sounds";
                                fileType.Text = "File type : .wav";
                                labels.RemoveAt(labels.IndexOf(labels.Last()));
                                labels.Add(fileType);
                                break;
                            case "animationsSections":
                                c.ExtensionFile = "m3d";
                                c.Tag = "animations";
                                c.OutputFilePath = "assets/animations";
                                fileType.Text = "File type : .m3d";
                                labels.RemoveAt(labels.IndexOf(labels.Last()));
                                labels.Add(fileType);
                                break;
                            case "scriptsSections":
                                c.ExtensionFile = "cs";
                                c.Tag = "scripts";
                                c.OutputFilePath = "assets/scripts";
                                fileType.Text = "File type : .cs";
                                labels.RemoveAt(labels.IndexOf(labels.Last()));
                                labels.Add(fileType);
                                break;
                            case "addGameObject":
                                Model model = LoadModelFromMesh(GenMeshCube(1f, 1f, 1f));
                                Vector3 position = new Vector3(2f, 0f, 5f);
                                Vector3 rotation = Vector3.Zero;
                                Vector3 scale = Vector3.One;
                                currentScene.AddGameObject(new GameObject(position, rotation, scale, "MonCube", model));
                                selectedElement = currentScene.GameObjects.Last();
                                TraceLog(TraceLogLevel.Info, "Game object added");
                                break;
                            case "play":
                                /*var p = new Process();
                                p.StartInfo = new ProcessStartInfo(@"C:\Users\ComtesseE1\Desktop\Crossy Road\bin\Debug\net7.0\crossy_road.exe");
                                p.Start();*/
                                break;
                            case "openProject":
                                openModal = true;
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

            for (int i = 1; i < files.Count; i++)
            {
                int positionX = (i + 8) % 8;
                if (positionX == 0) _ = 8;
                int xPos = (int)fileManager.X + 150 * (i) - 100;
                int yPos = (int)fileManager.Y + 60;
                DrawPanel(new Panel(xPos, yPos, 1, 0, fileTex, ""));
                string[] pathArryBySlash = files[i].Split('\\');
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
                            currentScene.AddGameObject(new GameObject(Vector3.Zero, Vector3.Zero, new Vector3(1, 1, 1), "[New model]", m));
                            selectedElement = currentScene.GameObjects.Last();
                        }
                        // Import texture in game object attributes
                        else if (mouse.X > gameManager.X + 100 && mouse.X < gameManager.X + 350 && mouse.Y > gameManager.Y + gameManager.Height / 2 + 300 && mouse.Y < gameManager.Y + gameManager.Height / 2 + 320 && selectedFile.Split('.').Last() == "png")
                        {
                            if (selectedElement != null)
                            {
                                foreach (GameObject go in currentScene.GameObjects)
                                {
                                    if (go == selectedElement)
                                    {
                                        go.SetTexture(LoadTexture(selectedFile));
                                    }
                                }
                            }
                        }
                        selectedFile = null;
                    }
                }
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
    }
}