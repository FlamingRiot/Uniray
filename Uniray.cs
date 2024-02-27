using static Raylib_cs.Raylib;
using Raylib_cs;
using static RayGUI_cs.RayGUI;
using RayGUI_cs;
using System.Numerics;
namespace Uniray
{
    public partial struct Uniray
    {
        public static readonly Color APPLICATION_COLOR = new Color(30, 30, 30, 255);
        public static readonly Color FOCUS_COLOR = new Color(60, 60, 60, 255);

        private int hWindow;

        private int wWindow;

        private Font baseFont;

        private Container fileManager;

        private Container gameManager;

        private List<Textbox> textboxes;

        private List<Button> buttons;

        private List<Label> labels;

        private Scene currentScene;

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
            this.wWindow = WWindow;
            this.hWindow = HWindow;
            this.baseFont = font;
            this.currentScene = scene;

            // Instantiate lists of components
            textboxes = new List<Textbox>();
            buttons = new List<Button>();
            labels = new List<Label>();

            // Containers
            float cont1X = wWindow - wWindow / 1.25f;
            float cont1Y = hWindow - hWindow / 3;
            fileManager = new Container((int)cont1X, (int)cont1Y, wWindow - (int)cont1X - 10, hWindow - (int)cont1Y - 10, APPLICATION_COLOR, FOCUS_COLOR);
            fileManager.Type = ContainerType.FileDropper;
            fileManager.OutputFilePath = "assets/models/";
            fileManager.ExtensionFile = "m3d";

            gameManager = new Container(10, 10, (int)cont1X - 20, hWindow - 20, APPLICATION_COLOR, FOCUS_COLOR);

            // Buttons
            Button modelsButton = new Button("Models", (int)fileManager.X + 48, (int)fileManager.Y, 60, 25, APPLICATION_COLOR, FOCUS_COLOR, "modelsSection");
            modelsButton.Type = ButtonType.Custom;
            buttons.Add(modelsButton);

            Button texturesButton = new Button("Textures", (int)fileManager.X + 164, (int)fileManager.Y, 60, 25, APPLICATION_COLOR, FOCUS_COLOR, "texturesSection");
            texturesButton.Type = ButtonType.Custom;
            buttons.Add(texturesButton);

            Button soundsButton = new Button("Sounds", (int)fileManager.X + 260, (int)fileManager.Y, 60, 25, APPLICATION_COLOR, FOCUS_COLOR, "soundsSection");
            soundsButton.Type = ButtonType.Custom;
            buttons.Add(soundsButton);

            Button animationsButton = new Button("Animations", (int)fileManager.X + 392, (int)fileManager.Y, 60, 25, APPLICATION_COLOR, FOCUS_COLOR, "animationsSections");
            animationsButton.Type = ButtonType.Custom;
            buttons.Add(animationsButton);

            Button openFileButton = new Button("Open", (int)fileManager.X + fileManager.Width - 38, (int)fileManager.Y + 5, 40, 20, APPLICATION_COLOR, FOCUS_COLOR, "openExplorer");
            openFileButton.Type = ButtonType.PathFinder;
            buttons.Add(openFileButton);

            Button addGameObject = new Button("+", (int)gameManager.X + gameManager.Width - 20, (int)gameManager.Y + 10, 10, 15, APPLICATION_COLOR, FOCUS_COLOR, "addGameObject");
            buttons.Add(addGameObject);

            // Textboxes
            /*Textbox textbox = new Textbox("Hello World !", 140, 200, 50, 20, APPLICATION_COLOR, FOCUS_COLOR);
            textboxes.Add(textbox);*/

            // Labels
            Label gameLayersLabel = new Label((int)gameManager.X + 10, (int)gameManager.Y + 10, "Game Layers");
            labels.Add(gameLayersLabel);

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

            foreach (GameObject go in currentScene.GameObjects)
            {
                DrawModelEx(go.Model, go.Position, go.Rotation, 0, go.Scale, Color.Red);
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

            if (!focus) { SetMouseCursor(MouseCursor.Default); }

            // =========================================================================================================================================================
            // ============================================================= MANAGE CUSTOM BUTTONS =====================================================================
            // =========================================================================================================================================================
            foreach (Button section in buttons)
            {
                if (IsButtonPressed(section))
                {
                    Container c = FileManager;
                    Label fileType = new Label((int)fileManager.X + (int)fileManager.Width / 2, (int)fileManager.Y + (int)fileManager.Height / 2, "");
                    switch (section.Tag)
                    {
                        case "modelsSection":
                            c.ExtensionFile = "m3d";
                            c.OutputFilePath = "assets/models";
                            fileType.Text = "File type : .m3d";
                            labels.RemoveAt(labels.IndexOf(labels.Last()));
                            labels.Add(fileType);
                            break;
                        case "texturesSection":
                            c.ExtensionFile = "png";
                            c.OutputFilePath = "assets/textures";
                            fileType.Text = "File type : .png";
                            labels.RemoveAt(labels.IndexOf(labels.Last()));
                            labels.Add(fileType);
                            break;
                        case "soundsSection":
                            c.ExtensionFile = "wav";
                            c.OutputFilePath = "assets/sounds";
                            fileType.Text = "File type : .wav";
                            labels.RemoveAt(labels.IndexOf(labels.Last()));
                            labels.Add(fileType);
                            break;
                        case "animationsSections":
                            c.ExtensionFile = "m3d";
                            c.OutputFilePath = "assets/animations";
                            fileType.Text = "File type : .m3d";
                            labels.RemoveAt(labels.IndexOf(labels.Last()));
                            labels.Add(fileType);
                            break;
                        case "addGameObject":
                            Model model = LoadModelFromMesh(GenMeshCube(1f, 1f, 1f));
                            Vector3 position = new Vector3(2f, 0f, 5f);
                            Vector3 rotation = Vector3.Zero;
                            Vector3 scale = Vector3.One;
                            currentScene.AddGameObject(new GameObject(position, rotation, scale, "MonCube", model));
                            TraceLog(TraceLogLevel.Info, "Game object added");
                            break;
                    }
                    FileManager = c;
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