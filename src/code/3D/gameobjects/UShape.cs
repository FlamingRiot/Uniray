using System.Numerics;
using Raylib_cs;

namespace Uniray
{
    /// <summary>Defines the type of the <see cref="UShape"/> object.</summary>
    public enum ShapeType
    {
        Cube,
        Sphere,
        Cylinder,
        Cone,
        HemiSphere,
        Knot,
        Torus
    }

    /// <summary>Represents a <see cref="UShape"/> game object.</summary>
    public class UShape : GameObject3D, IRotation, IClonable<UShape>
    {
        // -----------------------------------------------------------
        // Private attributes
        // -----------------------------------------------------------
        private ShapeType shapeType;
        private float _pitch;
        private float _yaw;
        private float _roll;

        // -----------------------------------------------------------
        // Public attributes
        // -----------------------------------------------------------
        public Mesh Mesh;
        public Material Material;

        // -----------------------------------------------------------
        // Public properties
        // -----------------------------------------------------------
        /// <summary>3-Dimensional position of the object.</summary>
        public override Vector3 Position
        {
            get
            {
                return new Vector3(Transform.M14, Transform.M24, Transform.M34);
            }
            set
            {
                Transform.M14 = value.X;
                Transform.M24 = value.Y;
                Transform.M34 = value.Z;
            }
        }

        /// <summary>X Axis rotation.</summary>
        [ComponentField(Group = "Rotation")]
        public float Pitch
        {
            get { return _pitch; }
            set
            {
                _pitch = value;
                SetRotation();
            }
        }
        /// <summary>Y Axis rotation</summary>
        [ComponentField(Group = "Rotation")]
        public float Yaw
        {
            get { return _yaw; }
            set
            {
                _yaw = value;
                SetRotation();
            }
        }
        /// <summary>Z Axis rotation.</summary>
        [ComponentField(Group = "Rotation")]
        public float Roll
        {
            get { return _roll; }
            set
            {
                _roll = value;
                SetRotation();
            }
        }

        /// <summary>Type of the shape object.</summary>
        public ShapeType Type { get { return shapeType; } set { shapeType = value; } }

        /// <summary>Creates an empty <see cref="UShape"/> instance.</summary>
        public UShape() : base()
        {
            // Empty shape object
            Type = ShapeType.Cube; // Default
            Mesh = HardRessource.ShapeMeshes[Type];
            Material = Raylib.LoadMaterialDefault();
        }

        /// <summary>Creates an instance of <see cref="UShape"/>.</summary>
        /// <param name="name">Name of shape</param>
        /// <param name="position">Position of the shape.</param>
        /// <param name="type">Type of the shape object.</param>
        public UShape(string name, Vector3 position, ShapeType type) : base(name, position)
        {
            Type = type;
            Mesh = HardRessource.ShapeMeshes[Type];
            Material = Raylib.LoadMaterialDefault();
        }

        /// <summary>Draws a shape object to the screen.</summary>
        public override void Draw()
        {
            Raylib.DrawMesh(Mesh, Material, Transform);
        }

        /// <summary>Creates a clone of the current game object.</summary>
        /// <returns>The generated clone.</returns>
        public UShape Clone()
        {
            return (UShape)MemberwiseClone();
        }

        /// <summary>Rotates the object with new rotation angles.</summary>
        private void SetRotation()
        {
            // Calculate matrix rotation
            Matrix4x4 rm = Raymath.MatrixRotateXYZ(new Vector3(_pitch / Raylib.RAD2DEG, _yaw / Raylib.RAD2DEG, _roll / Raylib.RAD2DEG));
            Matrix4x4 pm = Raymath.MatrixTranslate(X, Y, Z);
            // Multiply matrices
            Transform = Raymath.MatrixMultiply(rm, pm);
        }

        /// <summary>Rotates the object with new rotation angles.</summary>
        /// <param name="pitch">X Axis rotation.</param>
        /// <param name="yaw">Y Axis rotation.</param>
        /// <param name="roll">Z Axis rotation</param>
        private void SetRotation(float pitch, float yaw, float roll)
        {
            Pitch = pitch;
            Yaw = yaw;
            Roll = roll;

            // Calculate matrix rotation
            Matrix4x4 rm = Raymath.MatrixRotateXYZ(new Vector3(pitch / Raylib.RAD2DEG, yaw / Raylib.RAD2DEG, roll / Raylib.RAD2DEG));
            Matrix4x4 pm = Raymath.MatrixTranslate(X, Y, Z);
            // Multiply matrices
            Transform = Raymath.MatrixMultiply(rm, pm);
        }
    }
}