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
            Container fileManager = new Container(cont1X, cont1Y, width - cont1X - 10, height - cont1Y - 10, Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR, "models");
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
            // Models section
            Button modelButton = new Button("Models", fileManager.X + 48, fileManager.Y, 60, 25, Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR, "modelsSection");
            modelButton.Event = UpdateToModel;
            Components.Add("modelsButton", modelButton);

            // Textures section
            Button texturesButton = new Button("Textures", fileManager.X + 164, fileManager.Y, 60, 25, Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR, "texturesSection");
            texturesButton.Event = UpdateToTexture;
            Components.Add("texturesButton", texturesButton);

            // Sounds section
            Button soundsButton = new Button("Sounds", fileManager.X + 260, fileManager.Y, 60, 25, Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR, "soundsSection");
            soundsButton.Event = UpdateToSound;
            Components.Add("soundsButton", soundsButton);

            // Animations section
            Button animationsButton = new Button("Animations", fileManager.X + 392, fileManager.Y, 60, 25, Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR, "animationsSections");
            animationsButton.Event = UpdateToAnimation;
            Components.Add("animationsButton", animationsButton);

            // Scripts section
            Button scriptsButton = new Button("Scripts", fileManager.X + 492, fileManager.Y, 60, 25, Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR, "scriptsSections");
            scriptsButton.Event = UpdateToScript;
            Components.Add("scriptsButton", scriptsButton);

            // Open File button
            Button openFileButton = new Button("Open", fileManager.X + fileManager.Width - 38, fileManager.Y + 5, 40, 20, Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR, "openExplorer") { Type = ButtonType.PathFinder };
            Components.Add("openFileButton", openFileButton);

            // Open project button
            Button openProjectButton = new Button("Open project", fileManager.X + fileManager.Width - 175, fileManager.Y + 5, 125, 20, Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR, "openProject");
            Components.Add("openProjectButton", openProjectButton);

            // New project button
            Button newProjectButton = new Button("New project", fileManager.X + fileManager.Width - 303, fileManager.Y + 5, 50, 20, Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR, "newProject");
            Components.Add("newProjectButton", newProjectButton);

            // Play/Build button
            Button playButton = new Button("Play", (width - gameManager.Width - 20) / 2 + gameManager.Width + 20, 10, 100, 30, Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR, "play");
            playButton.Event = BuildProject;
            Components.Add("playButton", playButton);

            // Labels
            // Ressource info label
            Label ressourceInfoLabel = new Label(Components["fileManager"].X + Components["fileManager"].Width / 2, Components["fileManager"].Y + Components["fileManager"].Height / 2, "");
            Components.Add("ressourceInfoLabel", ressourceInfoLabel);

        }
        /// <summary>
        /// Draw the UI of the application
        /// </summary>
        public void Draw()
        {
            bool focus = false;
            foreach (KeyValuePair<string, Component> component in Components)
            {
                switch (component.Value)
                {
                    case Button button:
                        RayGUI.DrawButton(button, Font);
                        if (RayGUI.Hover(button.X, button.Y, button.Width, button.Height)) { focus = true; }
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
            if (!focus && Uniray.Data.SelectedFile is null) { Raylib.SetMouseCursor(MouseCursor.Default); }
        }
        /// <summary>
        /// Update the UI components and check interactions
        /// </summary>
        public void Update()
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                foreach (KeyValuePair<string, Component> component in Components)
                {
                    switch (component.Value)
                    {
                        case Button button:
                            button.Activate();
                            break;
                    }
                }
            }
        }
        
        private void UpdateToModel()
        {
            ((Container)Components["fileManager"]).ExtensionFile = "m3d";
            ((Container)Components["fileManager"]).Tag = "models";
            if (Uniray.Data.CurrentProject is not null) 
                ((Container)Components["fileManager"]).OutputFilePath = Path.GetDirectoryName(Uniray.Data.CurrentProject.Path) + "/assets/models";
            ((Label)Components["ressourceInfoLabel"]).Text = "File type: .m3d";
        }
        private void UpdateToTexture()
        {
            ((Container)Components["fileManager"]).ExtensionFile = "png";
            ((Container)Components["fileManager"]).Tag = "textures";
            if (Uniray.Data.CurrentProject is not null)
                ((Container)Components["fileManager"]).OutputFilePath = Path.GetDirectoryName(Uniray.Data.CurrentProject.Path) + "/assets/textures";
            ((Label)Components["ressourceInfoLabel"]).Text = "File type: .png";
        }
        private void UpdateToSound()
        {
            ((Container)Components["fileManager"]).ExtensionFile = "wav";
            ((Container)Components["fileManager"]).Tag = "sounds";
            if (Uniray.Data.CurrentProject is not null)
                ((Container)Components["fileManager"]).OutputFilePath = Path.GetDirectoryName(Uniray.Data.CurrentProject.Path) + "/assets/sounds";
            ((Label)Components["ressourceInfoLabel"]).Text = "File type: .wav";
        }
        private void UpdateToAnimation()
        {
            ((Container)Components["fileManager"]).ExtensionFile = "m3d";
            ((Container)Components["fileManager"]).Tag = "animations";
            if (Uniray.Data.CurrentProject is not null)
                ((Container)Components["fileManager"]).OutputFilePath = Path.GetDirectoryName(Uniray.Data.CurrentProject.Path) + "/assets/animations";
            ((Label)Components["ressourceInfoLabel"]).Text = "File type: .m3d";
        }
        private void UpdateToScript()
        {
            ((Container)Components["fileManager"]).ExtensionFile = "cs";
            ((Container)Components["fileManager"]).Tag = "scripts";
            if (Uniray.Data.CurrentProject is not null)
                ((Container)Components["fileManager"]).OutputFilePath = Path.GetDirectoryName(Uniray.Data.CurrentProject.Path) + "/assets/scripts";
            ((Label)Components["ressourceInfoLabel"]).Text = "File type: .cs";
        }
        private void BuildProject()
        {
            if (Uniray.Data.CurrentProject is not null)
            {
                // Build and run project here...
                string? projectPath = Path.GetDirectoryName(Uniray.Data.CurrentProject.Path);
                string commmand = "/C cd " + projectPath + " && dotnet run --project uniray_Project.csproj";
                System.Diagnostics.Process.Start("CMD.exe", commmand);
            }
        }
    }
}