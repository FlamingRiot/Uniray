using System.Windows.Input;
using Uniray.DatFiles;

namespace Uniray
{
    /// <summary>Represents an instance of <see cref="Project"/>.</summary>
    public class Project
    {
        private byte[] _encryptionKey;
        private byte[] _symmetricalVector;

        /// <summary>Project name.</summary>
        public string Name;

        /// <summary>Absolute path to the project.</summary>
        public string ProjectFile;

        /// <summary>Returns the folder of the project.</summary>
        public string ProjectFolder 
        { 
            get 
            { 
                string? path = Path.GetDirectoryName(ProjectFile);
                if (path != null) return path;
                else return "";
            } 
        }

        /// <summary>Encryption key of the project.</summary>
        public byte[] EncryptionKey { get { return _encryptionKey; } set { if (value.Length == DatEncoder.AES_KEY_LENGTH) _encryptionKey = value; } }

        /// <summary>Symmetrical vector used for project encoding.</summary>
        public byte[] SymmetricalVector { get { return _symmetricalVector; } set { if (value.Length == DatEncoder.AES_IV_LENGTH) _symmetricalVector = value; } }

        /// <summary>Scenes of the project.</summary>
        public List<Scene> Scenes;

        /// <summary>Creates a new project.</summary>
        /// <param name="name">Project name</param>
        /// <param name="path">Abosulte path to the project</param>
        /// <param name="scenes">Scenes of the project</param>
        public Project(string name, string path, List<Scene> scenes, byte[] aesKey, byte[] aesIv)
        {
            Name = name;
            ProjectFile = path.Replace('\\', '/'); // Ensure linux compatibility 
            Scenes = scenes;
            // Init cryptography keys
            _encryptionKey = aesKey;
            _symmetricalVector = aesIv;
        }

        /// <summary>Gets the scenes at a specified location.</summary>
        /// <param name="index">Scene index</param>
        /// <returns><see cref="Scene"/> at the specified location.</returns>
        public Scene GetScene(int index)
        {
            return Scenes[index];
        }

        /// <summary>Builds the project from an external window.</summary>
        public void Build()
        {
            string command = $"/C cd \"{ProjectFolder}\" && dotnet run --project uniray_Project.csproj";
            System.Diagnostics.Process.Start("CMD.exe", command); // Run project externally
        }

        /// <summary>Returns informations about the current instance.</summary>
        /// <returns>Informations as a <see langword="string"/>.</returns>
        public override string ToString()
        {
            return "The project " + Name + " is located at : " + ProjectFile + " and has " + Scenes.Count + "scenes";
        }
    }
}