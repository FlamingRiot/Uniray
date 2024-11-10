using RayGUI_cs;
using Raylib_cs;

namespace Uniray
{
    /// <summary>Represents an instance of the application's <see cref="UI"/>.</summary>
    public class UI
    {
        private static Container _modalTemplate = new Container(0, 0, 0, 0);

        /// <summary>The list of every components in the UI</summary>
        public Dictionary<string, Component> Components;

        /// <summary>The list of every modal in the UI</summary>
        public Dictionary<string, Modal> Modals;

        /// <summary>Creates an instance of a <see cref="UI"/>.</summary>
        public UI()
        {
            // Instanciate the components dictionary
            Components = new Dictionary<string, Component>();
            // Instanciate the modals dictionary
            Modals = new Dictionary<string, Modal>();

            // Create the UI components
            int cont1X = (int)(Program.Width - Program.Width / 1.25f);
            int cont1Y = (int)(Program.Height - Program.Height / 3f);
            // Containers
            // FileManager
            Container fileManager = new Container(cont1X, cont1Y, Program.Width - cont1X - 10, Program.Height - cont1Y - 10, "models");
            fileManager.Type = ContainerType.FileDropper;
            fileManager.ExtensionFile = "m3d";
            fileManager.OutputFilePath = "/assets/models";
            FileManager.CurrentFolder = FileManager.ModelFolder;
            Components.Add("fileManager", fileManager);

            // GameManager
            Container gameManager = new Container(10, 10, cont1X - 20, Program.Height - 20, "gameManager");
            Components.Add("gameManager", gameManager);

            // Template modal
            _modalTemplate = new Container(Program.Width / 2 - Program.Width / 6, Program.Height / 2 - Program.Height / 6, Program.Width / 3, Program.Height / 3, "modal");

            // Buttons
            // Models section
            Button modelButton = new Button("Models", fileManager.X + 10, fileManager.Y, 60, 25, "modelsSection");
            modelButton.Event = UpdateToModel;
            Components.Add("modelsButton", modelButton);

            // Textures section
            Button texturesButton = new Button("Textures", fileManager.X + 80, fileManager.Y, 80, 25, "texturesSection");
            texturesButton.Event = UpdateToTexture;
            Components.Add("texturesButton", texturesButton);

            // Sounds section
            Button soundsButton = new Button("Sounds", fileManager.X + 170, fileManager.Y, 60, 25, "soundsSection");
            soundsButton.Event = UpdateToSound;
            Components.Add("soundsButton", soundsButton);

            // Animations section
            Button animationsButton = new Button("Animations", fileManager.X + 240, fileManager.Y, 90, 25, "animationsSections");
            animationsButton.Event = UpdateToAnimation;
            Components.Add("animationsButton", animationsButton);

            // Scripts section
            Button scriptsButton = new Button("Scripts", fileManager.X + 340, fileManager.Y, 70, 25, "scriptsSections");
            scriptsButton.Event = UpdateToScript;
            Components.Add("scriptsButton", scriptsButton);

            // Open File button
            Button openFileButton = new Button("Open", fileManager.X + fileManager.Width - 70, fileManager.Y, 60, 25, "openExplorer"); //{ Type = ButtonType.PathFinder };
            Components.Add("openFileButton", openFileButton);

            // Open project button
            Button openProjectButton = new Button("Open project", fileManager.X + fileManager.Width - 200, fileManager.Y, 125, 25, "openProject");
            openProjectButton.Event = OpenProject;
            Components.Add("openProjectButton", openProjectButton);

            // New project button
            Button newProjectButton = new Button("New project", fileManager.X + fileManager.Width - 328, fileManager.Y, 120, 25, "newProject");
            newProjectButton.Event = NewProject;
            Components.Add("newProjectButton", newProjectButton);

            // Play/Build button
            Button playButton = new Button("Play", (Program.Width - gameManager.Width - 20) / 2 + gameManager.Width + 20, 10, 100, 30, "play");
            playButton.Event = Uniray.StartGameSimulation;
            Components.Add("playButton", playButton);

            // Upstream folder button
            Button returnButton = new Button("<", fileManager.X + 10, fileManager.Y + 40, 20, 20);
            returnButton.Event = FileManager.BackFolder;
            Components.Add("returnButton", returnButton);

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
            Label gameObjectLabel = new Label(Components["gameManager"].X + 10, Components["gameManager"].Y + Program.Height / 2, "Game Object");
            Components.Add("gameObjectLabel", gameObjectLabel);

            // Textboxes
            // Texture field textbox
            Textbox txb = new Textbox(Components["gameManager"].X + 100, Components["gameManager"].Y + Components["gameManager"].Height / 2 + 290, Components["gameManager"].Width - 120, 40, "");
            Components.Add("textureTextbox", txb);

            // Modals
            Button okModalButton = new Button("Proceed", _modalTemplate.X + _modalTemplate.Width - 70, _modalTemplate.Y + _modalTemplate.Height - 30, 60, 20);
            okModalButton.BaseColor = Color.Lime;
            okModalButton.Event = OkModal;
            Button closeModalButton = new Button("x", _modalTemplate.X + _modalTemplate.Width - 30, _modalTemplate.Y + 10, 20, 20);
            closeModalButton.BaseColor = Color.Red;
            closeModalButton.Event = CloseModal;
            // Open project modal
            Dictionary<string, Component> openProjModalComponents = new Dictionary<string, Component>();
            Label openProjectLabel = new Label(_modalTemplate.X + 20, _modalTemplate.Y + 50, "Copy .uproj link");
            openProjModalComponents.Add("openProjectLabel", openProjectLabel);
            Textbox openProjectTextbox = new Textbox(_modalTemplate.X + 20, _modalTemplate.Y + 70, 250, 20, "");
            openProjModalComponents.Add("openProjectTextbox", openProjectTextbox);
            openProjModalComponents.Add("closeModalButton", closeModalButton);
            openProjModalComponents.Add("okModalButton", okModalButton);

            Modals.Add("openProjectModal", new Modal(openProjModalComponents));

            // New project modal
            Dictionary<string, Component> newProjModalComponents = new Dictionary<string, Component>();
            Label newProjectLabel1 = new Label(_modalTemplate.X + 20, _modalTemplate.Y + 50, "Copy target directory path");
            newProjModalComponents.Add("newProjectLabel1", newProjectLabel1);
            Label newProjectLabel2 = new Label(_modalTemplate.X + 20, _modalTemplate.Y + 100, "Choose your project name");
            newProjModalComponents.Add("newProjectLabel2", newProjectLabel2);
            Textbox newProjectTextbox1 = new Textbox(_modalTemplate.X + 20, _modalTemplate.Y + 70, 250, 20, "");
            newProjModalComponents.Add("newProjectTextbox1", newProjectTextbox1);
            Textbox newProjectTextbox2 = new Textbox(_modalTemplate.X + 20, _modalTemplate.Y + 120, 250, 20, "");
            newProjModalComponents.Add("newProjectTextbox2", newProjectTextbox2);
            newProjModalComponents.Add("closeModalButton", closeModalButton);
            newProjModalComponents.Add("okModalButton", okModalButton);

            Modals.Add("newProjectModal", new Modal(newProjModalComponents));
        }

        /// <summary>Draws the full 2D UI of the application.</summary>
        public void Draw()
        {
            bool focus = false;
            bool oModal = false;
            if (UData.CurrentModal is not null)
            {
                DrawModal(UData.CurrentModal, ref focus);
                if (focus) oModal = true;
                RayGUI.DeactivateList();
            }
            RayGUI.DrawGUIList(Components.Values.ToList(), ref focus);
            RayGUI.ActivateList();

            // Check focus
            if (!focus && !oModal) Raylib.SetMouseCursor(MouseCursor.Default);
        }

        /// <summary>Draws a modal and its components.</summary>
        /// <param name="key">Key of the modal to draw</param>
        public void DrawModal(string key, ref bool focus)
        {
            Raylib.DrawRectangle(0, 0, Program.Width, Program.Height, new Color(0, 0, 0, 75));
            RayGUI.DrawContainer(_modalTemplate);
            RayGUI.DrawGUIList(Modals[key].Components.Values.ToList(), ref focus);
        }

        /// <summary>
        /// Set the file manager to the models page
        /// </summary>
        private void UpdateToModel()
        {
            ((Container)Components["fileManager"]).ExtensionFile = "m3d";
            ((Container)Components["fileManager"]).Tag = "models";
            if (UData.CurrentProject is not null) 
            {
                ((Container)Components["fileManager"]).OutputFilePath = UData.CurrentProject.ProjectFolder + "/assets/models";
                FileManager.CurrentFolder = FileManager.ModelFolder;
            }
            ((Label)Components["ressourceInfoLabel"]).Text = "File type: .m3d";
        }
        /// <summary>
        /// Set the file manager to the textures page
        /// </summary>
        private void UpdateToTexture()
        {
            ((Container)Components["fileManager"]).ExtensionFile = "png";
            ((Container)Components["fileManager"]).Tag = "textures";
            if (UData.CurrentProject is not null)
            {
                ((Container)Components["fileManager"]).OutputFilePath = UData.CurrentProject.ProjectFolder + "/assets/textures";
                FileManager.CurrentFolder = FileManager.TextureFolder;
            }
            ((Label)Components["ressourceInfoLabel"]).Text = "File type: .png";
        }
        /// <summary>
        /// Set the file manager to the sounds page
        /// </summary>
        private void UpdateToSound()
        {
            ((Container)Components["fileManager"]).ExtensionFile = "wav";
            ((Container)Components["fileManager"]).Tag = "sounds";
            if (UData.CurrentProject is not null)
            {
                ((Container)Components["fileManager"]).OutputFilePath = UData.CurrentProject.ProjectFolder + "/assets/sounds";
                FileManager.CurrentFolder = FileManager.SoundFolder;
            }
            ((Label)Components["ressourceInfoLabel"]).Text = "File type: .wav";
        }
        /// <summary>
        /// Set the file manager to the animations page
        /// </summary>
        private void UpdateToAnimation()
        {
            ((Container)Components["fileManager"]).ExtensionFile = "m3d";
            ((Container)Components["fileManager"]).Tag = "animations";
            if (UData.CurrentProject is not null)
            {
                ((Container)Components["fileManager"]).OutputFilePath = UData.CurrentProject.ProjectFolder + "/assets/animations";
                FileManager.CurrentFolder = FileManager.AnimationFolder;
            }
            ((Label)Components["ressourceInfoLabel"]).Text = "File type: .m3d";
        }
        /// <summary>
        /// Set the file manager to the scripts page
        /// </summary>
        private void UpdateToScript()
        {
            ((Container)Components["fileManager"]).ExtensionFile = "cs";
            ((Container)Components["fileManager"]).Tag = "scripts";
            if (UData.CurrentProject is not null)
            {
                ((Container)Components["fileManager"]).OutputFilePath = UData.CurrentProject.ProjectFolder + "/assets/scripts";
                FileManager.CurrentFolder = FileManager.ScriptFolder;
            }
            ((Label)Components["ressourceInfoLabel"]).Text = "File type: .cs";
        }

        /// <summary>
        /// Send exit code for positive response on open project modal
        /// </summary>
        private void CloseModal()
        {
            UData.LastModalExitCode = 0;
        }
        /// <summary>
        /// Send exit code for negative response on new project modal
        /// </summary>
        private void OkModal()
        {
            UData.LastModalExitCode = 1;
        }
        /// <summary>
        /// Open the 'new project' modal
        /// </summary>
        private void OpenProject()
        {
            UData.CurrentModal = "openProjectModal";
        }
        /// <summary>
        /// Open the 'open project' modal
        /// </summary>
        private void NewProject()
        {
            UData.CurrentModal = "newProjectModal";
        }
    }
}