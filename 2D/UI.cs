using RayGUI_cs;
using Raylib_cs;

namespace Uniray._2D
{
    public class UI
    {
        /// <summary>
        /// The list of every components in the UI
        /// </summary>
        public Dictionary<string, Component> Components;
        /// <summary>
        /// UI Font
        /// </summary>
        private Font font;
        /// <summary>
        /// UI Font
        /// </summary>
        public Font Font { get { return font; } set { font = value; } }
        /// <summary>
        /// UI Constructor
        /// </summary>
        /// <param name="width">Window width</param>
        /// <param name="height">Window height</param>
        /// <param name="font">Font</param>
        public UI(int width, int height, Font font)
        {
            // Instanciate the components dictionary
            Components = new Dictionary<string, Component>();
            this.font = font;

            // Create the UI components
            int cont1X = (int)(width - width / 1.25f);
            int cont1Y = (int)(height - height / 3f);
            // Containers
            // FileManager
            Container fileManager = new Container(cont1X, cont1Y, width - cont1X - 10, height - cont1Y - 10, Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR, "fileManager");
            fileManager.Type = ContainerType.FileDropper;
            fileManager.ExtensionFile = "m3d";
            fileManager.OutputFilePath = "/assets/models";
            Components.Add("fileManager", fileManager);

            // GameManager
            Container gameManager = new Container(10, 10, cont1X - 20, height - 20, Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR, "gameManager");
            Components.Add("gameManager", gameManager);

            // Modal Template
            Container modalTemplate = new Container(width / 2 - width / 6, height / 2 - height / 6, width / 3, height / 3, Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR, "modal");
            Components.Add("modalTemplate", modalTemplate);

            // Buttons


        }
        public void Draw()
        {
            foreach (KeyValuePair<string, Component> component in Components)
            {
                switch (component.Value)
                {
                    case Button button:
                        RayGUI.DrawButton(button, Font);
                        break;
                    case Textbox textbox:
                        RayGUI.DrawTextbox(ref textbox, Font);
                        Components[component.Key] = textbox;
                        break;
                    case Label label:
                        RayGUI.DrawLabel(label, Font);
                        break;
                    case Panel panel:
                        RayGUI.DrawPanel(panel);
                        break;
                    case Tickbox tickbox:
                        RayGUI.DrawTickbox(ref tickbox);
                        Components[component.Key] = tickbox;
                        break;
                    case DragDropBox dragDropBox:
                        RayGUI.DrawDragDropBox(dragDropBox);
                        break;
                    case Container container:
                        RayGUI.DrawContainer(ref container);
                        Components[component.Key] = container;
                        break;

                }
            }
        }
    }
}