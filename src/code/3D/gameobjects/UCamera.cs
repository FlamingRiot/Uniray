using Raylib_cs;
using System.Numerics;

namespace Uniray
{
    /// <summary>Represents an instance of <see cref="UCamera"/>.</summary>
    public class UCamera : GameObject3D
    {
        // -----------------------------------------------------------
        // Private attributes
        // -----------------------------------------------------------
        private float _pitch;
        private float _yaw;

        /// <summary>Default <see cref="Camera3D"/> configuration.</summary>
        public static readonly Camera3D DefaultCamera = new Camera3D()
        {
            Position = Vector3.UnitY,
            Target = new Vector3(0, 0, 1),
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
                Vector3 delta = value - Camera.Position;
                Camera.Position = value;
                Camera.Target += delta;

                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }

        /// <summary>X Axis rotation.</summary>
        [ComponentField(Group = "Rotation")]
        public float Pitch
        {
            get { return _pitch; }
            set
            {
                ComponentField.SetProperty(Raymath.Clamp(ClampAngle(value), 90, 270), ref _pitch, nameof(Pitch));
                UpdateRotation();
            }
        }
        /// <summary>Y Axis rotation</summary>
        [ComponentField(Group = "Rotation")]
        public float Yaw
        {
            get { return _yaw; }
            set
            {
                ComponentField.SetProperty(ClampAngle(value), ref _yaw, nameof(Yaw));
                UpdateRotation();
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

        /// <summary>Draws the camera object to the screen.</summary>
        public unsafe override void Draw()
        {
            Raylib.DrawBillboard(Conceptor3D.EnvCamera, HardRessource.Textures["camera"], Camera.Position, 1, Color.White);
        }

        /// <summary>Draws the camera's outline to the screen.</summary>
        public unsafe override void DrawOutline()
        {
            Rlgl.SetCullFace(Rlgl.ZERO);
            //Raylib.DrawBil
            Rlgl.SetCullFace(Rlgl.ONE);
        }

        /// <summary>Updates the rotation of the camera model.</summary>
        private void UpdateRotation()
        {
            // Update camera target
            Vector3 vertical = Raymath.Vector3RotateByAxisAngle(DefaultCamera.Target, Vector3.UnitX, _pitch / Raylib.RAD2DEG);
            Vector3 horizontal = Raymath.Vector3RotateByAxisAngle(DefaultCamera.Target, Vector3.UnitY, _yaw / Raylib.RAD2DEG) * (1.01f - Math.Abs(vertical.Y));
            Camera.Target = Camera.Position + new Vector3(horizontal.X, vertical.Y, horizontal.Z);

        }
        /// <summary>Returns informations about the current instance.</summary>
        /// <returns>Informations as a <see langword="string"/>.</returns>
        public override string ToString()
        {
            return "Name: " + Name + " Position: " + Position + " Target: " + Camera.Target;
        }
    }
}