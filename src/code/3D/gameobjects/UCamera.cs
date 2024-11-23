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
        // Public attributes
        // -----------------------------------------------------------
        /// <summary>X Axis rotation.</summary>
        public float Pitch;
        /// <summary>Y Axis rotation</summary>
        public float Yaw;
        /// <summary>Z Axis rotation.</summary>
        public float Roll;

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

        /// <summary>Creates an empty instance of <see cref="UCamera"/>.</summary>
        public UCamera() : base() { Camera = DefaultCamera; }

        /// <summary>Creates an instance of <see cref="UCamera"/>.</summary>
        /// <param name="name">Object name.</param>
        /// <param name="camera">Object camera.</param>
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