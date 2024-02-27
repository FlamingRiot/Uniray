using static Raylib_cs.Raylib;
using Raylib_cs;
using static RayGUI_cs.RayGUI;
using RayGUI_cs;
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

        public List<Button> Buttons { get { return buttons; } }
        public Container GameManager { get { return gameManager; } set { gameManager = value; } }
        public Container FileManager { get { return fileManager; } set { fileManager = value; } }

        /// <summary>
        /// Construct UI
        /// </summary>
        public Uniray(int WWindow, int HWindow, Font font)
        {
            this.wWindow = WWindow;
            this.hWindow = HWindow;
            this.baseFont = font;

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
            Button modelsButton = new Button("Models", (int)fileManager.X + 48, (int)fileManager.Y, 60, 25, APPLICATION_COLOR, FOCUS_COLOR);
            modelsButton.Type = ButtonType.Custom;
            buttons.Add(modelsButton);

            Button texturesButton = new Button("Textures", (int)fileManager.X + 164, (int)fileManager.Y, 60, 25, APPLICATION_COLOR, FOCUS_COLOR);
            texturesButton.Type = ButtonType.Custom;
            buttons.Add(texturesButton);

            Button soundsButton = new Button("Sounds", (int)fileManager.X + 260, (int)fileManager.Y, 60, 25, APPLICATION_COLOR, FOCUS_COLOR);
            soundsButton.Type = ButtonType.Custom;
            buttons.Add(soundsButton);

            Button animationsButton = new Button("Animations", (int)fileManager.X + 392, (int)fileManager.Y, 60, 25, APPLICATION_COLOR, FOCUS_COLOR);
            animationsButton.Type = ButtonType.Custom;
            buttons.Add(animationsButton);

            Button openFileButton = new Button("Open", (int)fileManager.X + fileManager.Width - 38, (int)fileManager.Y + 5, 40, 20, APPLICATION_COLOR, FOCUS_COLOR);
            openFileButton.Type = ButtonType.PathFinder;
            buttons.Add(openFileButton);

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
        /// <summary>
        /// Draw user interface of the application
        /// </summary>
        public void DrawUI()
        {
            // Draw
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

            if (!focus) { SetMouseCursor(MouseCursor.Default); }

            // Manage pressed buttons
            foreach (Button section in buttons)
            {
                if (IsButtonPressed(section))
                {
                    Container c = FileManager;
                    Label fileType = new Label((int)fileManager.X + (int)fileManager.Width / 2, (int)fileManager.Y + (int)fileManager.Height / 2, "");
                    switch (section.Text)
                    {
                        case "Models":
                            c.ExtensionFile = "m3d";
                            c.OutputFilePath = "assets/models";
                            fileType.Text = "File type : .m3d";
                            labels.RemoveAt(labels.IndexOf(labels.Last()));
                            labels.Add(fileType);
                            break;
                        case "Textures":
                            c.ExtensionFile = "png";
                            c.OutputFilePath = "assets/textures";
                            fileType.Text = "File type : .png";
                            labels.RemoveAt(labels.IndexOf(labels.Last()));
                            labels.Add(fileType);
                            break;
                        case "Sounds":
                            c.ExtensionFile = "wav";
                            c.OutputFilePath = "assets/sounds";
                            fileType.Text = "File type : .wav";
                            labels.RemoveAt(labels.IndexOf(labels.Last()));
                            labels.Add(fileType);
                            break;
                        case "Animations":
                            c.ExtensionFile = "m3d";
                            c.OutputFilePath = "assets/animations";
                            fileType.Text = "File type : .m3d";
                            labels.RemoveAt(labels.IndexOf(labels.Last()));
                            labels.Add(fileType);
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