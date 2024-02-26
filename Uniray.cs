using static Raylib_cs.Raylib;
using Raylib_cs;
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

        private List<Container> containers;

        private List<Textbox> textboxes;

        private List<Button> buttons;

        /// <summary>
        /// Construct UI
        /// </summary>
        public Uniray(int WWindow, int HWindow, Font font)
        {
            this.wWindow = WWindow;
            this.hWindow = HWindow;
            this.baseFont = font;

            // Instantiate lists of components
            containers = new List<Container>();
            textboxes = new List<Textbox>();
            buttons = new List<Button>();

            // Containers
            float cont1X = wWindow - wWindow / 1.25f;
            float cont1Y = hWindow - hWindow / 3;
            Container fileManager = new Container((int)cont1X, (int)cont1Y, wWindow - (int)cont1X, hWindow - (int)cont1Y - 10, APPLICATION_COLOR, FOCUS_COLOR);
            fileManager.Type = ContainerType.FileDropper;
            fileManager.OutputFilePath = "assets/models/";
            fileManager.ExtensionFile = "m3d";
            containers.Add(fileManager);

            Container gameManager = new Container(10, 10, (int)cont1X - 20, hWindow - 20, APPLICATION_COLOR, FOCUS_COLOR);
            containers.Add(gameManager);

            // Buttons
            Button button = new Button("Evan", 1200, 400, 60, 20, APPLICATION_COLOR, FOCUS_COLOR);
            button.Type = ButtonType.PathFinder;
            buttons.Add(button);

            // Textboxes
            Textbox textbox = new Textbox("Hello World !", 140, 200, 50, 20, APPLICATION_COLOR, FOCUS_COLOR);
            textboxes.Add(textbox);
        }

        /// <summary>
        /// Draw user interface of the application
        /// </summary>
        public void DrawUI()
        {
            DrawRectangle(0, 0, (int)(wWindow - wWindow / 1.25f), hWindow, APPLICATION_COLOR);
            DrawRectangle(0, hWindow - hWindow / 3 - 10, wWindow, hWindow - (hWindow - hWindow / 3) + 10, APPLICATION_COLOR);

            foreach (Button button in buttons) { RayGUI.DrawButton(button, baseFont); }
            for (int i = 0; i < containers.Count; i++)
            {
                Container container = containers[i];
                containers[i] = RayGUI.DrawContainer(ref container);
            }
            for (int i = 0; i < textboxes.Count; i++) 
            {
                Textbox textbox = textboxes[i];
                textboxes[i] = RayGUI.DrawTextbox(ref textbox, baseFont); 
            }
        }
    }
}
