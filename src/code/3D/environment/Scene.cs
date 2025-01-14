using Raylib_cs;
using Raylib_cs.Complements;
using System.Numerics;

namespace Uniray
{
    /// <summary>Represents an instance of Uniray <see cref="Scene"/>.</summary>
    public class Scene
    {
        internal string SkyboxPath;

        /// <summary>Name of the scene.</summary>
        public string Name;

        /// <summary>Camera of the scene.</summary>
        public UCamera Camera;

        /// <summary>Skybox object of the scene.</summary>
        public Skybox Skybox;

        /// <summary>Game objects of the scene.</summary>
        public List<GameObject3D> GameObjects;

        /// <summary>Creates an empty instance of Uniray <see cref="Scene"/>.</summary>
        public Scene()
        {
            GameObjects = new List<GameObject3D>();
            Camera = new UCamera();
            Name = "";
            SkyboxPath = "";
        }

        /// <summary>Creates an instance of Uniray <see cref="Scene"/>.</summary>
        /// <param name="gameObjects">Scene game objects.</param>
        public Scene(string name, List<GameObject3D> gameObjects, string skyboxPath)
        {
            GameObjects = gameObjects;
            if (gameObjects.Count != 0) Camera = (UCamera)gameObjects.Where(x => x is UCamera).ToList().First();
            else Camera = new UCamera();
            Name = name;
            SkyboxPath = skyboxPath;
            if (skyboxPath == Skybox.DEFAULT_CUBEMAP_TEXTURE) LoadSkybox(); // Load if default passed (simplicity.)
        }

        /// <summary>Loads the skybox object of the scene.</summary>
        public void LoadSkybox()
        {
            Skybox = RaylibComplements.LoadSkybox(SkyboxPath, EnvDirection.North);
        }

        /// <summary>Returns informations about the current instance.</summary>
        /// <returns>Informations as a <see langword="string"/>.</returns>
        public override string ToString()
        {
            return $"The scene {Name} contains {GameObjects.Count} elements";
        }
    }
}