using Raylib_cs;
using System.Numerics;

namespace Uniray
{
    public class Scene
    {
        /// <summary>
        /// Objects of the scene
        /// </summary>
        private List<GameObject3D> gameObjects;
        /// <summary>
        /// Game objects list property
        /// </summary>
        public List<GameObject3D> GameObjects { get { return gameObjects; } }    
        /// <summary>
        /// Add game object
        /// </summary>
        /// <param name="go">Game object</param>
        public void AddGameObject(GameObject3D go)
        {
            gameObjects.Add(go);
        }
        /// <summary>
        /// Get game object
        /// </summary>
        /// <param name="index">Index of the object</param>
        /// <returns>Game object</returns>
        public GameObject3D GetGameObject(int index)
        {
            return gameObjects[index];
        }
        /// <summary>
        /// Set game object
        /// </summary>
        /// <param name="index">Index of the object</param>
        /// <param name="go">Game object</param>
        public void SetGameObject(int index, GameObject3D go)
        {
            gameObjects[index] = go;
        }
        /// <summary>
        /// Set game object position
        /// </summary>
        /// <param name="index">Index of the object</param>
        /// <param name="position">Game object position</param>
        public void SetGameObjectPosition(int index, Vector3 position)
        {
            if (gameObjects[index] is UModel)
            {
                UModel go = (UModel)gameObjects[index];
                go.Position = position;
                gameObjects[index] = go;
            }
            else if (gameObjects[index] is UCamera)
            {
                UCamera go = (UCamera)gameObjects[index];
                go.Position = position;
                gameObjects[index] = go;
            }
        }
        /// <summary>
        /// Scene Constructor
        /// </summary>
        /// <param name="camera">Scene base camera</param>
        public Scene(UCamera camera)
        {
            gameObjects = new List<GameObject3D> 
            { 
                camera
            };
        }
        /// <summary>
        /// Scene Constructor
        /// </summary>
        /// <param name="camera">Scene base camera</param>
        /// <param name="gameObjects">Scene game objects</param>
        public Scene(UCamera camera, List<GameObject3D> gameObjects)
        {
            this.gameObjects = gameObjects;
            this.gameObjects.Add(camera);
        }
        /// <summary>
        /// Scene scene informations
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return "This scene contains : " + gameObjects.Count;
        }
    }
}