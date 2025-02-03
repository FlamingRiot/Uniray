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
        Torus,
        HollowCylinder
    }

    /// <summary>Represents a <see cref="UShape"/> game object.</summary>
    public class UShape : GameObject3D, IRotation, IScalable, IClonable<UShape>
    {
        // -----------------------------------------------------------
        // Private attributes
        // -----------------------------------------------------------
        private ShapeType shapeType;
        private float _pitch;
        private float _yaw;
        private float _roll;

        private float _width = 1;
        private float _height = 1;
        private float _depth = 1;

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
                ComponentField.SetProperty(ClampAngle(value), ref _pitch, nameof(Pitch));
                UpdateTransform();
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
                UpdateTransform();
            }
        }
        /// <summary>Z Axis rotation.</summary>
        [ComponentField(Group = "Rotation")]
        public float Roll
        {
            get { return _roll; }
            set
            {
                ComponentField.SetProperty(ClampAngle(value), ref _roll, nameof(Roll));
                UpdateTransform();
            }
        }

        /// <summary>X Axis Scale.</summary>
        [ComponentField(Group = "Scale")]
        public float Width
        {
            get { return _width; }
            set
            {
                ComponentField.SetProperty(value, ref _width, nameof(Width));
                UpdateTransform();
            }
        }
        /// <summary>Y Axis Scale</summary>
        [ComponentField(Group = "Scale")]
        public float Height
        {
            get { return _height; }
            set
            {
                ComponentField.SetProperty(value, ref _height, nameof(Height));
                UpdateTransform();
            }
        }
        /// <summary>Z Axis Scale.</summary>
        [ComponentField(Group = "Scale")]
        public float Depth
        {
            get { return _depth; }
            set
            {
                ComponentField.SetProperty(value, ref _depth, nameof(Depth));
                UpdateTransform();
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

        /// <summary>Draws the shape's outline to the screen.</summary>
        public override void DrawOutline()
        {
            Rlgl.SetCullFace(Rlgl.ZERO);
            Raylib.DrawMesh(Mesh, Uniray.Shaders.OutlineMaterial, Transform);
            Rlgl.SetCullFace(Rlgl.ONE);
        }

        /// <summary>Creates a clone of the current game object.</summary>
        /// <returns>The generated clone.</returns>
        public UShape Clone()
        {
            return (UShape)MemberwiseClone();
        }

        /// <summary>Updates the mesh used for rendering using the current type.</summary>
        internal void UpdateMesh()
        {
            Mesh = HardRessource.ShapeMeshes[Type];
        }

        /// <summary>Updates the entire transform matrix of the object.</summary>
        private void UpdateTransform()
        {
            // Calculate matrix rotation
            Matrix4x4 rm = Raymath.MatrixRotateXYZ(new Vector3(_pitch / Raylib.RAD2DEG, _yaw / Raylib.RAD2DEG, _roll / Raylib.RAD2DEG));
            Matrix4x4 sm = Raymath.MatrixScale(_width, _height, _depth);
            Matrix4x4 pm = Raymath.MatrixTranslate(X, Y, Z);
            // Multiply matrices
            Transform = pm * sm * rm;
        }
    }
}