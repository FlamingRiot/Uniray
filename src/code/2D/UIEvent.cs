using System.Numerics;
using RayGUI_cs;

namespace Uniray
{
    /// <summary>Represents the UI part hosting the related events.</summary>
    public partial class UI
    {
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

        /// <summary>Toggles file DropDown list</summary>
        private void ToggleFileDD()
        {
            if (Components.ContainsValue(fileDD)) Components.Remove("fileDD");
            else Components.Add("fileDD", fileDD);
        }

        private void ToggleAddDD()
        {
            if (Components.ContainsValue(addDD)) Components.Remove("addDD");
            else Components.Add("addDD", addDD);
        }

        // -----------------------------------------------------------
        // Add DropDown list events
        // -----------------------------------------------------------
        
        private void AddCube()
        {
            UData.CurrentScene.GameObjects.Add(new UShape("Cube", Vector3.UnitY, ShapeType.Cube));
        }

        private void AddSphere()
        {
            UData.CurrentScene.GameObjects.Add(new UShape("Sphere", Vector3.UnitY, ShapeType.Sphere));
        }

        private void AddCylinder()
        {
            UData.CurrentScene.GameObjects.Add(new UShape("Cylinder", Vector3.UnitY, ShapeType.Cylinder));
        }

        private void AddTorus()
        {
            UData.CurrentScene.GameObjects.Add(new UShape("Torus", Vector3.UnitY, ShapeType.Torus));
        }

        private void AddCone()
        {
            UData.CurrentScene.GameObjects.Add(new UShape("Cone", Vector3.UnitY, ShapeType.Cone));
        }

        private void AddHemisphere()
        {
            UData.CurrentScene.GameObjects.Add(new UShape("Hemisphere", Vector3.UnitY, ShapeType.HemiSphere));
        }
        private void AddKnot()
        {
            UData.CurrentScene.GameObjects.Add(new UShape("Knot", Vector3.UnitY, ShapeType.Knot));
        }
    }
}