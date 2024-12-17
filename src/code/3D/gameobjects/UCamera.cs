using Raylib_cs;
using System.Numerics;

namespace Uniray
{
    /// <summary>Represents an instance of <see cref="UCamera"/>.</summary>
    public class UCamera : GameObject3D, IRotation
    {
        // -----------------------------------------------------------
        // Private attributes
        // -----------------------------------------------------------
        private float _pitch;
        private float _yaw;
        private float _roll;

        /// <summary>Default <see cref="Camera3D"/> configuration.</summary>
        public static readonly Camera3D DefaultCamera = new Camera3D()
        {
            Position = Vector3.UnitY,
            Target = new Vector3(0, 1, 1),
            Up = Vector3.UnitY,
            Projection = CameraProjection.Perspective,
            FovY = 90
        };

        // -----------------------------------------------------------
        // Public attributes
        // -----------------------------------------------------------
        /// <summary>3-Dimensional camera.</summary>
        public Camera3D Camera;

        // -----------------------------------------------------------
        // Public properties
        // -----------------------------------------------------------

        /// <summary>3-Dimensional position of the camera.</summary>
        public override Vector3 Position 
        { 
            get 
            { 
                return Camera.Position; 
            } 
            set 
            { 
                Camera.Position = value;
                Transform.M14 = value.X;
                Transform.M24 = value.Y;
                Transform.M34 = value.Z;
            }
        }

        /// <summary>X Axis rotation angle.</summary>
        [ComponentField(Group = "Rotation")]
        public float Pitch { get { return _pitch; } set { _pitch = value; } }
        /// <summary>Y Axis rotation angle.</summary>
        [ComponentField(Group = "Rotation")]
        public float Yaw { get { return _yaw; } set { _yaw = value; } }
        /// <summary>Z Axis rotation angle.</summary>
        [ComponentField(Group = "Rotation")]
        public float Roll { get { return _roll; } set {  _roll = value; } }

        /// <summary>Creates an empty instance of <see cref="UCamera"/>.</summary>
        public UCamera() : base() { Camera = DefaultCamera; }

        /// <summary>Creates an instance of <see cref="UCamera"/>.</summary>
        /// <param name="name">Object name.</param>
        /// <param name="camera">Object camera.</param>
        public UCamera(string name, Camera3D camera):base(name, camera.Position) 
        {
            Camera = camera;
        }

        /// <summary>Draws the camera object to the screen.</summary>
        public unsafe override void Draw()
        {
            Raylib.DrawMesh(HardRessource.Models["camera"].Meshes[0], Uniray.Shaders.OutlineMaterial, Transform);
        }

        /// <summary>Returns informations about the current instance.</summary>
        /// <returns>Informations as a <see langword="string"/>.</returns>
        public override string ToString()
        {
            return "Name: " + Name + " Position: " + Position + " Target: " + Camera.Target;
        }
    }
}