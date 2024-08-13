using RayGUI_cs;
using Raylib_cs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Uniray
{
    public class UI
    {
        /// <summary>
        /// Width of the UI
        /// </summary>
        private int width;
        /// <summary>
        /// Height of the UI 
        /// </summary>
        private int height;
        /// <summary>
        /// The list of every components in the UI
        /// </summary>
        public Dictionary<string, Component> Components;
        /// <summary>
        /// The list of every modals in the UI that are accessible
        /// </summary>
        public Dictionary<string, Modal> Modals;
        /// <summary>
        /// UI Font
        /// </summary>
        private Font font;
        /// <summary>
        /// UI Font
        /// </summary>
        public Font Font { get { return font; } set { font = value; } }
        /// <summary>
        /// Width of the UI
        /// </summary>
        public int Width { get { return width; } set { width = value; } }
        /// <summary>
        /// Height of the UI
        /// </summary>
        public int Height { get { return height; } set { height = value; } }
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
            // Instanciate the modals dictionary
            Modals = new Dictionary<string, Modal>();
            // Instanciate font and size of the UI
            this.font = font;
            this.width = width;
            this.height = height;

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
            openProjectButton.Event = OpenProject;
            Components.Add("openProjectButton", openProjectButton);

            // New project button
            Button newProjectButton = new Button("New project", fileManager.X + fileManager.Width - 303, fileManager.Y + 5, 50, 20, Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR, "newProject");
            newProjectButton.Event = NewProject;
            Components.Add("newProjectButton", newProjectButton);

            // Play/Build button
            Button playButton = new Button("Play", (width - gameManager.Width - 20) / 2 + gameManager.Width + 20, 10, 100, 30, Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR, "play");
            playButton.Event = BuildProject;
            Components.Add("playButton", playButton);

            // Labels
            // Ressource info label
            Label ressourceInfoLabel = new Label(Components["fileManager"].X + Components["fileManager"].Width / 2, Components["fileManager"].Y + Components["fileManager"].Height / 2, "");
            ressourceInfoLabel.Text = "File type: .m3d";
            Components.Add("ressourceInfoLabel", ressourceInfoLabel);

            // Game layer label
            Label gameLayersLabel = new Label(Components["gameManager"].X + 10, Components["gameManager"].Y + 10, "Game Layers");
            Components.Add("gameLayerLabel", gameLayersLabel);

            // Texture field label
            Label goTextureLabel = new Label(Components["gameManager"].X + 20, Components["gameManager"].Y + Components["gameManager"].Height / 2 + 300, "Texture");
            Components.Add("textureLabel", goTextureLabel);

            // Game object label
            Label gameObjectLabel = new Label(Components["gameManager"].X + 10, Components["gameManager"].Y + height / 2, "Game Object");
            Components.Add("gameObjectLabel", gameObjectLabel);

            // Textboxes
            // Texture field textbox
            Textbox txb = new Textbox(Components["gameManager"].X + 100, Components["gameManager"].Y + Components["gameManager"].Height / 2 + 290, Components["gameManager"].Width - 120, 40, "", Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR);
            Components.Add("textureTextbox", txb);

            // Modals
            Button okModalButton = new Button("Proceed", Components["modalTemplate"].X + Components["modalTemplate"].Width - 70, Components["modalTemplate"].Y + Components["modalTemplate"].Height - 30, 60, 20, Color.Lime, Uniray.FOCUS_COLOR);
            okModalButton.Event = OkModal;
            Button closeModalButton = new Button("x", Components["modalTemplate"].X + Components["modalTemplate"].Width - 30, Components["modalTemplate"].Y + 10, 20, 20, Color.Red, Uniray.FOCUS_COLOR);
            closeModalButton.Event = CloseModal;
            // Open project modal
            Dictionary<string, Component> openProjModalComponents = new Dictionary<string, Component>();
            Label openProjectLabel = new Label(Components["modalTemplate"].X + 20, Components["modalTemplate"].Y + 50, "Copy .uproj link");
            openProjModalComponents.Add("openProjectLabel", openProjectLabel);
            Textbox openProjectTextbox = new Textbox(Components["modalTemplate"].X + 20, Components["modalTemplate"].Y + 70, 250, 20, "", Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR);
            openProjModalComponents.Add("openProjectTextbox", openProjectTextbox);
            openProjModalComponents.Add("closeModalButton", closeModalButton);
            openProjModalComponents.Add("okModalButton", okModalButton);

            Modals.Add("openProjectModal", new Modal(openProjModalComponents));

            // New project modal
            Dictionary<string, Component> newProjModalComponents = new Dictionary<string, Component>();
            Label newProjectLabel1 = new Label(Components["modalTemplate"].X + 20, Components["modalTemplate"].Y + 50, "Copy target directory path");
            newProjModalComponents.Add("newProjectLabel1", newProjectLabel1);
            Label newProjectLabel2 = new Label(Components["modalTemplate"].X + 20, Components["modalTemplate"].Y + 100, "Choose your project name");
            newProjModalComponents.Add("newProjectLabel2", newProjectLabel2);
            Textbox newProjectTextbox1 = new Textbox(Components["modalTemplate"].X + 20, Components["modalTemplate"].Y + 70, 250, 20, "", Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR);
            newProjModalComponents.Add("newProjectTextbox1", newProjectTextbox1);
            Textbox newProjectTextbox2 = new Textbox(Components["modalTemplate"].X + 20, Components["modalTemplate"].Y + 120, 250, 20, "", Uniray.APPLICATION_COLOR, Uniray.FOCUS_COLOR);
            newProjModalComponents.Add("newProjectTextbox2", newProjectTextbox2);
            newProjModalComponents.Add("closeModalButton", closeModalButton);
            newProjModalComponents.Add("okModalButton", okModalButton);

            Modals.Add("newProjectModal", new Modal(newProjModalComponents));
        }
        /// <summary>
        /// Draw the UI of the application
        /// </summary>
        public void Draw()
        {
            bool focus = false;
            DrawComponents(Components, ref focus);
            if (Uniray.Data.CurrentModal is not null) DrawModal(Uniray.Data.CurrentModal, ref focus);
            if (!focus && Uniray.Data.SelectedFile is null) { Raylib.SetMouseCursor(MouseCursor.Default); }
        }
        private void DrawComponents(Dictionary<string, Component> components, ref bool focus)
        {
            foreach (KeyValuePair<string, Component> component in components)
            {
                switch (component.Value)
                {
                    case Button button:
                        RayGUI.DrawButton(button, Font);
                        if (RayGUI.Hover(button.X, button.Y, button.Width, button.Height)) { focus = true; }
                        break;
                    case Textbox textbox:
                        RayGUI.DrawTextbox(ref textbox, Font);
                        components[component.Key] = textbox;
                        if (RayGUI.Hover(textbox.X, textbox.Y, textbox.Width, textbox.Height)) { focus = true; }
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
                        if (container.Tag != "modal") RayGUI.DrawContainer(ref container);
                        Components[component.Key] = container;
                        break;

                }
            }
        }
        /// <summary>
        /// Draw a give modal
        /// </summary>
        /// <param name="key">Key of the modal to draw</param>
        public void DrawModal(string key, ref bool focus)
        {
            Raylib.DrawRectangle(0, 0, width, height, new Color(0, 0, 0, 75));
            RayGUI.DrawContainer((Container)Components["modalTemplate"]);
            DrawComponents(Modals[key].Components, ref focus);
        }
        /// <summary>
        /// Set the file manager to the models page
        /// </summary>
        private void UpdateToModel()
        {
            ((Container)Components["fileManager"]).ExtensionFile = "m3d";
            ((Container)Components["fileManager"]).Tag = "models";
            if (Uniray.Data.CurrentProject is not null) 
                ((Container)Components["fileManager"]).OutputFilePath = Path.GetDirectoryName(Uniray.Data.CurrentProject.Path) + "/assets/models";
            ((Label)Components["ressourceInfoLabel"]).Text = "File type: .m3d";
        }
        /// <summary>
        /// Set the file manager to the textures page
        /// </summary>
        private void UpdateToTexture()
        {
            ((Container)Components["fileManager"]).ExtensionFile = "png";
            ((Container)Components["fileManager"]).Tag = "textures";
            if (Uniray.Data.CurrentProject is not null)
                ((Container)Components["fileManager"]).OutputFilePath = Path.GetDirectoryName(Uniray.Data.CurrentProject.Path) + "/assets/textures";
            ((Label)Components["ressourceInfoLabel"]).Text = "File type: .png";
        }
        /// <summary>
        /// Set the file manager to the sounds page
        /// </summary>
        private void UpdateToSound()
        {
            ((Container)Components["fileManager"]).ExtensionFile = "wav";
            ((Container)Components["fileManager"]).Tag = "sounds";
            if (Uniray.Data.CurrentProject is not null)
                ((Container)Components["fileManager"]).OutputFilePath = Path.GetDirectoryName(Uniray.Data.CurrentProject.Path) + "/assets/sounds";
            ((Label)Components["ressourceInfoLabel"]).Text = "File type: .wav";
        }
        /// <summary>
        /// Set the file manager to the animations page
        /// </summary>
        private void UpdateToAnimation()
        {
            ((Container)Components["fileManager"]).ExtensionFile = "m3d";
            ((Container)Components["fileManager"]).Tag = "animations";
            if (Uniray.Data.CurrentProject is not null)
                ((Container)Components["fileManager"]).OutputFilePath = Path.GetDirectoryName(Uniray.Data.CurrentProject.Path) + "/assets/animations";
            ((Label)Components["ressourceInfoLabel"]).Text = "File type: .m3d";
        }
        /// <summary>
        /// Set the file manager to the scripts page
        /// </summary>
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
        /// <summary>
        /// Send exit code for positive response on open project modal
        /// </summary>
        private void CloseModal()
        {
            Uniray.Data.LastModalExitCode = 0;
        }
        /// <summary>
        /// Send exit code for negative response on new project modal
        /// </summary>
        private void OkModal()
        {
            Uniray.Data.LastModalExitCode = 1;
        }
        /// <summary>
        /// Open the 'new project' modal
        /// </summary>
        private void OpenProject()
        {
            Uniray.Data.CurrentModal = "openProjectModal";
        }
        /// <summary>
        /// Open the 'open project' modal
        /// </summary>
        private void NewProject()
        {
            Uniray.Data.CurrentModal = "newProjectModal";
        }
    }
}