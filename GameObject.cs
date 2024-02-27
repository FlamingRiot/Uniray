using Raylib_cs;
using System.Numerics;

namespace Uniray
{
    public class GameObject
    {
        /// <summary>
        /// Transform of the object
        /// </summary>
        private Transform transform;

        /// <summary>
        /// Model of the object
        /// </summary>
        private Model model;

        public float X { get { return transform.Translation.X; } set { transform.Translation.X = value; } }
        public float Y { get { return transform.Translation.Y; } set { transform.Translation.Y = value; } }
        public float Z { get { return transform.Translation.Z; } set { transform.Translation.Z = value; } }
        public float Rx { get { return transform.Rotation.X; } set { transform.Rotation.X = value; } }
        public float Ry { get { return transform.Rotation.Y; } set { transform.Rotation.Y = value; } }
        public float Rz { get { return transform.Rotation.Z; } set { transform.Rotation.Z = value; } }
        public float Sx { get { return transform.Scale.X; } set { transform.Scale.X = value; } }
        public float Sy { get { return transform.Scale.Y; } set { transform.Scale.Y = value; } }
        public float Sz { get { return transform.Scale.Z; } set { transform.Scale.Z = value; } }

        public Model Model { get { return model; } set { model = value; } }

        /// <summary>
        /// Constructor 1
        /// </summary>
        /// <param name="transform">Transform of the object</param>
        public GameObject(Transform transform)
        {
            this.transform = transform;
            this.model = new Model();
        }

        /// <summary>
        /// Constructor 2
        /// </summary>
        /// <param name="transform">Transform of the object</param>
        /// <param name="model">Model of the object</param>
        public GameObject(Transform transform, Model model)
        {
            this.transform = transform;
            this.model = model;
        }
    }
}
