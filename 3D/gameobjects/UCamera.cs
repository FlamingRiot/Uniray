using Raylib_cs;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Uniray
{
    public class UCamera : GameObject3D
    {
        /// <summary>
        /// Yaw rotation
        /// </summary>
        private float yaw;
        /// <summary>
        /// Pitch rotation
        /// </summary>
        private float pitch;
        /// <summary>
        /// Roll rotation
        /// </summary>
        private float roll;
        /// <summary>
        /// 3-Dimensional camera
        /// </summary>
        private Camera3D camera;
        /// <summary>
        /// Yaw rotation
        /// </summary>
        public float Yaw { get { return yaw; } set { yaw = value; } }
        /// <summary>
        /// Pitch rotation
        /// </summary>
        public float Pitch { get { return pitch; } set { pitch = value; } }
        /// <summary>
        /// Roll rotation
        /// </summary>
        public float Roll { get { return roll; } set { roll = value; } }
        /// <summary>
        /// 3-Dimensional camera
        /// </summary>
        public Camera3D Camera { get { return camera; } set { camera = value; } }
        /// <summary>
        /// 3-Dimensional target of the camera
        /// </summary>
        public Vector3 Target { get { return camera.Target; } set { camera.Target = value; } }
        /// <summary>
        /// 3-Dimensional position of the camera
        /// </summary>
        public override Vector3 Position { get { return camera.Position; } set { position = value; camera.Position = value; } }
        /// <summary>
        /// UCamera default Constructor
        /// </summary>
        public UCamera() : base() { }
        /// <summary>
        /// UCamera Constructor
        /// </summary>
        /// <param name="name">Object name</param>
        /// <param name="position">Object position</param>
        /// <param name="camera">Object camera</param>
        public UCamera(string name, Camera3D camera):base(name, camera.Position) 
        {
            Camera = camera;
        }
        /// <summary>
        /// Send object informations
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Name: " + Name + " Position: " + Position + " Target: " + Camera.Target;
        }
    }
}