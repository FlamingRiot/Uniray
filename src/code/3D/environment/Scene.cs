using Raylib_cs;
using Raylib_cs.Complements;
using System.Numerics;

namespace Uniray
{
    /// <summary>Represents an instance of Uniray <see cref="Scene"/>.</summary>
    public class Scene
    {
        internal string SkyboxID;

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
            SkyboxID = "";
        }

        /// <summary>Creates an instance of Uniray <see cref="Scene"/>.</summary>
        /// <param name="gameObjects">Scene game objects.</param>
        public Scene(string name, List<GameObject3D> gameObjects, string skyboxKey)
        {
            GameObjects = gameObjects;
            if (gameObjects.Count != 0) Camera = (UCamera)gameObjects.Where(x => x is UCamera).ToList().First();
            else Camera = new UCamera();
            Name = name;
            SkyboxID = skyboxKey;
            LoadSkybox();
        }

        /// <summary>Loads the skybox object of the scene.</summary>
        public void LoadSkybox()
        {
            if (Skybox.DEFAULT_CUBEMAP_TEXTURE.Contains(SkyboxID)) Skybox = RaylibComplements.LoadSkybox(Skybox.DEFAULT_CUBEMAP_TEXTURE, EnvDirection.North);
            else Skybox = RaylibComplements.LoadSkybox(Uniray.Ressource.GetTexture(SkyboxID), SkyboxID, EnvDirection.North);
        }

        /// <summary>Returns informations about the current instance.</summary>
        /// <returns>Informations as a <see langword="string"/>.</returns>
        public override string ToString()
        {
            return $"The scene {Name} contains {GameObjects.Count} elements";
        }
    }
}