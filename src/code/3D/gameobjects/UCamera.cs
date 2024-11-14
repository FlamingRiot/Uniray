using Raylib_cs;
using System.Numerics;

namespace Uniray
{
    /// <summary>Represents an instance of <see cref="UCamera"/>.</summary>
    public class UCamera : GameObject3D
    {
        /// <summary>Default <see cref="Camera3D"/> configuration.</summary>
        public static readonly Camera3D DefaultCamera = new Camera3D()
        {
            Position = Vector3.Zero,
            Target = Vector3.Zero,
            Up = Vector3.UnitY,
            Projection = CameraProjection.Perspective,
            FovY = 90
        };

        // -----------------------------------------------------------
        // Private attributes
        // -----------------------------------------------------------

        private Vector3 _rotation;
        private Camera3D _camera;

        // -----------------------------------------------------------
        // Public properties
        // -----------------------------------------------------------

        /// <summary>Pitch rotation.</summary>
        public float Pitch { get { return _rotation.X; } set { _rotation.X = value; } }
        /// <summary>Yaw rotation.</summary>
        public float Yaw { get { return _rotation.Y; } set { _rotation.Y = value; } }
        /// <summary>Roll rotation.</summary>
        public float Roll { get { return _rotation.Z; } set { _rotation.Z = value; } }
        /// <summary>3-Dimensional camera.</summary>
        public Camera3D Camera { get { return _camera; } set { _camera = value; } }
        /// <summary>
        /// 3-Dimensional position of the camera
        /// </summary>
        public override Vector3 Position 
        { 
            get 
            { 
                return camera.Position; 
            } 
            set 
            { 
                camera.Position = value;
                transform.M14 = value.X;
                transform.M24 = value.Y;
                transform.M34 = value.Z;
            }
        }
        /// <summary>Creates an empty instance of <see cref="UCamera"/>.</summary>
        public UCamera() : base() { }

        /// <summary>Creates an instance of <see cref="UCamera"/>.</summary>
        /// <param name="name">Object name</param>
        /// <param name="position">Object position</param>
        /// <param name="camera">Object camera</param>
        public UCamera(string name, Camera3D camera):base(name, camera.Position) 
        {
            Camera = camera;
        }

        /// <summary>Returns informations about the current instance.</summary>
        /// <returns>Informations as a <see langword="string"/>.</returns>
        public override string ToString()
        {
            return "Name: " + Name + " Position: " + Position + " Target: " + Camera.Target;
        }
    }
}