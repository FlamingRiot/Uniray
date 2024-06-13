using Raylib_cs;
using System.Numerics;

namespace Uniray
{
    public class Scene
    {
        /// <summary>
        /// Objects of the scene
        /// </summary>
        private List<GameObject> gameObjects;

        /// <summary>
        /// Camera of the scene
        /// </summary>
        private Camera3D camera;

        /// <summary>
        /// Camera property
        /// </summary>
        public Camera3D Camera { get { return camera; } set { camera = value; } }

        /// <summary>
        /// Game objects list property
        /// </summary>
        public List<GameObject> GameObjects { get { return gameObjects; } }    

        /// <summary>
        /// Add game object
        /// </summary>
        /// <param name="go">Game object</param>
        public void AddGameObject(GameObject go)
        {
            gameObjects.Add(go);
        }

        /// <summary>
        /// Get game object
        /// </summary>
        /// <param name="index">Index of the object</param>
        /// <returns>Game object</returns>
        public GameObject GetGameObject(int index)
        {
            return gameObjects[index];
        }
        /// <summary>
        /// Set game object
        /// </summary>
        /// <param name="index">Index of the object</param>
        /// <param name="go">Game object</param>
        public void SetGameObject(int index, GameObject go)
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
            GameObject go = gameObjects[index];
            go.Position = position;
            SetGameObject(index, go);
        }
        /// <summary>
        /// Set game object rotation
        /// </summary>
        /// <param name="index">Index of the object</param>
        /// <param name="rotation">Game object position</param>
        public void SetGameObjectRotation(int index, Vector3 rotation)
        {
            GameObject go = gameObjects[index];
            go.Rotation = rotation;
            SetGameObject(index, go);
        }
        /// <summary>
        /// Set the new position of the camera 
        /// </summary>
        /// <param name="pos"></param>
        public void SetCameraPosition(Vector3 pos)
        {
            camera.Position = pos;
        }

        /// <summary>
        /// Construct new Scene
        /// </summary>
        /// <param name="camera"></param>
        public Scene(Camera3D camera)
        {
            gameObjects = new List<GameObject>();
            this.camera = camera;
        }

        public Scene(Camera3D camera, List<GameObject> gameObjects)
        {
            this.camera = camera;
            this.gameObjects = gameObjects;
        }

        /// <summary>
        /// Scene to string
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return "This scene contains : " + gameObjects.Count;
        }
    }
}
