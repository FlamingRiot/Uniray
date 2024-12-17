using RayGUI_cs;
using Raylib_cs;
using System.Numerics;
using System.Reflection;

namespace Uniray
{
    /// <summary>Represents the UI part hosting the construction/rendering.</summary>
    public partial class UI
    {
        private static Container _modalTemplate = new Container(0, 0, 0, 0);

        public const string COMPONENT_IDENTIFIER = "UxComponent";
        public const string COMPONENT_GROUP_IDENTIFIER = "UxComponentGroup";

        /// <summary>The list of every components in the UI</summary>
        public Dictionary<string, Component> Components;

        /// <summary>The list of every modal in the UI</summary>
        public Dictionary<string, Modal> Modals;

        // -----------------------------------------------------------
        // Background storred DropDown lists (to avoid GB overflow on show/hide)
        // -----------------------------------------------------------

        DropDown fileDD;
        DropDown addDD;

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
            if (UData.CurrentProject is not null) // Direct project opening config
            {
                fileManager.OutputFilePath = UData.CurrentProject.ProjectFolder + "/assets/models";
            }
            else // Default config
            {
                fileManager.OutputFilePath = "/assets/models";
            }
            FileManager.CurrentFolder = FileManager.ModelFolder;
            Components.Add("fileManager", fileManager);

            // GameManager
            Container gameManager = new Container(10, 40, cont1X - 20, Program.Height - 50, "gameManager");
            Components.Add("gameManager", gameManager);

            // Template modal
            _modalTemplate = new Container(Program.Width / 2 - Program.Width / 6, Program.Height / 2 - Program.Height / 6, Program.Width / 3, Program.Height / 3, "modal");

            // Buttons
            Button fileDDButton = new Button("File", 10, 5, 60, 19, "fileDDButton");
            fileDDButton.Event = ToggleFileDD;
            Components.Add("fileDDButton", fileDDButton);

            Button displayDDButton = new Button("Display", 80, 5, 75, 19, "displayDDButton");

            Components.Add("displayDDButton", displayDDButton);

            Button addDDButton = new Button("Add", 165, 5, 60, 19, "addDDButton");
            addDDButton.Event = ToggleAddDD;
            Components.Add("addDDButton", addDDButton);

            // Play/Build button
            Button playButton = new Button("Play", 235, 5, 60, 19, "play");
            playButton.Event = Uniray.StartGameSimulation;
            Components.Add("playButton", playButton);

            Button helpButton = new Button("Help", 305, 5, 70, 19, "helpButton");
            Components.Add("helpButton", helpButton);

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
            Button openFileButton = new Button("Open", fileManager.X + fileManager.Width - 70, fileManager.Y, 60, 25, "openExplorer");
            openFileButton.Type = ButtonType.PathFinder;
            Components.Add("openFileButton", openFileButton);

            // App quitting button
            Button quitAppButton = new Button("x", Program.Width - 35, 0, 35, 29);
            quitAppButton.FontSize = 19;
            quitAppButton.BaseColor = Uniray.TOOLBAR_COLOR;
            quitAppButton.HoverColor = Color.Red;
            quitAppButton.Event = Program.CloseUniray;
            Components.Add("quitAppButton", quitAppButton);

            // App ToggleFullScreen
            Button toggleFullScreenButton = new Button("::", Program.Width - 70, 0, 35, 29);
            toggleFullScreenButton.BaseColor = Uniray.TOOLBAR_COLOR;
            toggleFullScreenButton.HoverColor = Color.Gray;
            toggleFullScreenButton.Event = Program.ReduceUniray;
            Components.Add("toggleFullScreenButton", toggleFullScreenButton);

            // App Reduce button
            Button reduceAppButton = new Button("-", Program.Width - 105, 0, 35, 29);
            reduceAppButton.FontSize = 19;
            reduceAppButton.BaseColor = Uniray.TOOLBAR_COLOR;
            reduceAppButton.HoverColor = Color.Gray;
            reduceAppButton.Event = Raylib.MinimizeWindow;
            Components.Add("reduceAppButton", reduceAppButton);

            // Upstream folder button
            Button returnButton = new Button("<", fileManager.X + 10, fileManager.Y + 40, 20, 20);
            returnButton.Event = FileManager.BackFolder;
            Components.Add("returnButton", returnButton);

            // Labels

            // Project name info label
            Label projectNameLabel = new Label(Program.Width / 2, 7, "Uniray - Empty");
            projectNameLabel.TextColor = Color.Gray;
            Components.Add("projectNameLabel", projectNameLabel); 

            // Ressource info label
            Label ressourceInfoLabel = new Label(Components["fileManager"].X + 20, Program.Height - 40, "File Type: .m3d");
            Components.Add("ressourceInfoLabel", ressourceInfoLabel);

            // Game layer label
            Label gameLayersLabel = new Label(Components["gameManager"].X + 10, Components["gameManager"].Y + 10, "Game Layers");
            Components.Add("gameLayerLabel", gameLayersLabel);

            // Game object label
            Label gameObjectLabel = new Label(Components["gameManager"].X + 10, Components["gameManager"].Y + Program.Height / 2, "Game Object");
            Components.Add("gameObjectLabel", gameObjectLabel);

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

            // -----------------------------------------------------------
            // DropDown lists instanciation
            // -----------------------------------------------------------
            // File DropDown
            List<string> fileDDS = new List<string>()
            {
                "Create project",
                "Open project",
                "Save project",
                "Publish project",
            };

            fileDD = new DropDown(10, 35, 160, fileDDS);
            //fileDD.RepeatedHeight = 5;
            fileDD.BackgroundColor = Uniray.SPECIAL_COLOR;
            // Set events
            fileDD.SetButtonEvent(0, NewProject);
            fileDD.SetButtonEvent(1, OpenProject);
            fileDD.SetButtonEvent(2, Uniray.SaveProject);
            fileDD.SetButtonEvent(3, Project.Build);

            // Add DropDown
            List<string> addDDS = new List<string>()
            {
                "Cube",
                "Sphere",
                "Cylinder",
                "Cone",
                "Hemisphere",
                "Torus",
                "Knot"
            };
            addDD = new DropDown(165, 30, 140, addDDS);
            addDD.BackgroundColor = Uniray.SPECIAL_COLOR;
            addDD.SetButtonEvent(0, AddCube);
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

        /// <summary>Displays the components.</summary>
        /// <param name="props">Component elligible properties of the object.</param>
        /// <param name="obj">Object to use.</param>
        public void DisplayComponents(PropertyInfo[] props, GameObject3D obj)
        {
            Component reference = Components["gameObjectLabel"];
            // Retrieve different groups
            // Create grouping
            Dictionary<string, List<PropertyInfo>> grouping = new Dictionary<string, List<PropertyInfo>>();
            for (int i = 0; i < props.Length; i++) 
            {
                ComponentField? attribute = props[i].GetCustomAttribute<ComponentField>(); // Get the attribute
                string group = ""; // Get the group
                if (attribute != null) group = attribute.Group;
                if (!grouping.ContainsKey(group)) grouping.Add(group, new List<PropertyInfo>());
                grouping[group].Add(props[i]);
            }

            int index = 0;
            int groupIndex = 0;
            foreach (List<PropertyInfo> gProps in grouping.Values)
            {
                gProps.Reverse();
                for (int i = 0; i < gProps.Count; i++)
                {
                    ComponentField? attribute = gProps[i].GetCustomAttribute<ComponentField>(); // Get the attribute
                    string group = ""; // Get the group
                    if (attribute != null) group = attribute.Group;
                    // Add group label
                    string lblName = $"{group}{COMPONENT_GROUP_IDENTIFIER}";
                    if (!Components.ContainsKey(lblName))
                    {
                        Components.Add(lblName, new Label(reference.X, reference.Y + 25 + index * 24 + groupIndex * 24, group));
                        groupIndex++;
                    }

                    string txbName = $"{gProps[i].Name}{COMPONENT_IDENTIFIER}";
                    Textbox component = new Textbox(reference.X, reference.Y + 25 + index * 24 + groupIndex * 24, 90, 19, $"{gProps[i].GetValue(obj)}");
                    component.Args = new string[] { gProps[i].Name, $"{gProps[i].GetValue(obj)}", obj.GetType().ToString(), gProps[i].PropertyType.ToString(), txbName };
                    /* Argument list
                     * Property name [0]
                     * Property initial value [1]
                     * Object type [2]
                     * Property type [3]
                     * Refered textbox name [4]
                     */
                    // Prepare function arguments
                    component.OnEntry = UpdateComponent;
                    Components.Add(txbName, component);

                    // Increment total index
                    index++;
                }
            }
        }

        /// <summary>Removes the component fields from the UI.</summary>
        public void HideComponents()
        {
            Components.ToList().ForEach(pair =>
            {
                if (pair.Key.Contains(COMPONENT_IDENTIFIER)) Components.Remove(pair.Key);
            });
        }

        /// <summary>Updates a component's values to the <see cref="GameObject3D"/>.</summary>
        /// <param name="args">Arguments of the component.</param>
        /// <param name="value">New value of the component.</param>
        public void UpdateComponent(string[] args, string value)
        {
            Console.WriteLine($"Property {args[0]} updated");
            Type? type = Type.GetType(args[2]);
            if (type is not null)
            {
                PropertyInfo? prop = type.GetProperty(args[0]);
                var obj = Convert.ChangeType(UData.Selection.First(), type);
                // Try convert value
                Type? propType = Type.GetType(args[3]);
                if (propType is not null) 
                {
                    try
                    {
                        var convertedValue = Convert.ChangeType(value, propType);
                        prop?.SetValue(obj, convertedValue);
                        // Update arguments
                        args[1] = value;
                    }
                    catch
                    {
                        ((Textbox)Components[args[4]]).Text = args[1];
                    }
                }
            }
        }

        /// <summary>Draws a modal and its components.</summary>
        /// <param name="key">Key of the modal to draw</param>
        public void DrawModal(string key, ref bool focus)
        {
            Raylib.DrawRectangle(0, 0, Program.Width, Program.Height, new Color(0, 0, 0, 75));
            RayGUI.DrawContainer(_modalTemplate);
            RayGUI.DrawGUIList(Modals[key].Components.Values.ToList(), ref focus);
        }
    }
}