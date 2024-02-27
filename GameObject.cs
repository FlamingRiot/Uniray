using Raylib_cs;
using System.Numerics;

namespace Uniray
{
    public class GameObject
    {
        /// <summary>
        /// Position of the object
        /// </summary>
        private Vector3 position;

        /// <summary>
        /// Rotation of the object
        /// </summary>
        private Vector3 rotation;

        /// <summary>
        /// Scale of the object
        /// </summary>
        private Vector3 scale;

        /// <summary>
        /// Model of the object
        /// </summary>
        private Model model;

        /// <summary>
        /// Name of the object
        /// </summary>
        private string name;

        public float X { get { return position.X; } set { position.X = value; } }
        public float Y { get { return position.Y; } set { position.Y = value; } }
        public float Z { get { return position.Z; } set { position.Z = value; } }
        public float Rx { get { return rotation.X; } set { rotation.X = value; } }
        public float Ry { get { return rotation.Y; } set { rotation.Y = value; } }
        public float Rz { get { return rotation.Z; } set { rotation.Z = value; } }
        public float Sx { get { return scale.X; } set { scale.X = value; } }
        public float Sy { get { return scale.Y; } set { scale.Y = value; } }
        public float Sz { get { return scale.Z; } set { scale.Z = value; } }

        public Model Model { get { return model; } set { model = value; } }
        public Vector3 Position { get { return position; } set { position = value; } }
        public Vector3 Rotation { get { return rotation; } set { rotation = value; } }
        public Vector3 Scale { get { return scale; } set { scale = value; } }
        public string Name  { get { return name; } set { name = value; } }


        /// <summary>
        /// Constructor 1
        /// </summary>
        /// <param name="transform">Transform of the object</param>
        public GameObject(Vector3 position, Vector3 rotation, Vector3 scale, string name)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            this.name = name;
            this.model = new Model();
        }

        /// <summary>
        /// Constructor 2
        /// </summary>
        /// <param name="transform">Transform of the object</param>
        /// <param name="model">Model of the object</param>
        public GameObject(Vector3 position, Vector3 rotation, Vector3 scale, string name,Model model)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            this.name = name;
            this.model = model;
        }

        public override string ToString()
        {
            return "X : " + X.ToString() + "\n\nY : " + Y.ToString() + "\n\nZ : " + Z.ToString();
        }
    }
}
