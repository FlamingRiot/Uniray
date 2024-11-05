namespace Uniray
{
    /// <summary>Represents an instance of <see cref="Project"/>.</summary>
    public class Project
    {
        /// <summary>Project name.</summary>
        public string Name;

        /// <summary>Absolute path to the project.</summary>
        public string Path;

        /// <summary>Scenes of the project.</summary>
        public List<Scene> Scenes;

        /// <summary>Creates a new project.</summary>
        /// <param name="name">Project name</param>
        /// <param name="path">Abosulte path to the project</param>
        /// <param name="scenes">Scenes of the project</param>
        public Project(string name, string path, List<Scene> scenes)
        {
            Name = name;
            Path = path.Replace('\\', '/');
            Scenes = scenes;
        }

        /// <summary>Gets the scenes at a specified location.</summary>
        /// <param name="index">Scene index</param>
        /// <returns><see cref="Scene"/> at the specified location.</returns>
        public Scene GetScene(int index)
        {
            return Scenes[index];
        }

        /// <summary>Returns informations about the current instance.</summary>
        /// <returns>Informations as a <see langword="string"/>.</returns>
        public override string ToString()
        {
            return "The project " + Name + " is located at : " + Path + " and has " + Scenes.Count + "scenes";
        }
    }
}