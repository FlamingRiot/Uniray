using Raylib_cs;

namespace Uniray
{
    public class Project
    {
        /// <summary>
        /// Project name
        /// </summary>
        private string name;

        /// <summary>
        /// Absolute path to the project
        /// </summary>
        private string path;

        /// <summary>
        /// Scenes of the project
        /// </summary>
        private List<Scene> scenes;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Project name</param>
        /// <param name="path">Abosulte path to the project</param>
        /// <param name="scenes">Scenes of the project</param>
        public Project(string name, string path, List<Scene> scenes)
        {
            this.name = name;
            this.path = path;
            this.scenes = scenes;
        }

        /// <summary>
        /// Get scene according to an index
        /// </summary>
        /// <param name="index">Wanted scene index</param>
        /// <returns>Wanted scene</returns>
        public Scene GetScene(int index)
        {
            return scenes[index];
        }

        /// <summary>
        /// Overrided ToString
        /// </summary>
        /// <returns>Stringified project</returns>
        public override string ToString()
        {
            return "The project " + name + " is located at : " + path + " and has " + scenes.Count + "scenes";
        }
    }
}
