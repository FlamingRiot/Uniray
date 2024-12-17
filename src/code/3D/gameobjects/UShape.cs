using System.Numerics;
using System.Reflection.Metadata.Ecma335;
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
    public class UShape : GameObject3D
    {
        // -----------------------------------------------------------
        // Private attributes
        // -----------------------------------------------------------
        private ShapeType shapeType;

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

        /// <summary>Type of the shape object.</summary>
        public ShapeType Type { get { return shapeType; } set { shapeType = value; } }

        /// <summary>Creates an empty <see cref="UShape"/> instance.</summary>
        public UShape() : base()
        {
            // Empty shape object
            Type = ShapeType.Cube; // Default
            Mesh = GenMesh(Type);
            Material = Raylib.LoadMaterialDefault();
        }

        /// <summary>Creates an instance of <see cref="UShape"/>.</summary>
        /// <param name="name">Name of shape</param>
        /// <param name="position">Position of the shape.</param>
        /// <param name="type">Type of the shape object.</param>
        public UShape(string name, Vector3 position, ShapeType type) : base(name, position)
        {
            Type = type;
            Mesh = GenMesh(Type);
            Material = Raylib.LoadMaterialDefault();
        }

        /// <summary>Generates the shape mesh from type.</summary>
        /// <param name="type">Type of the mesh.</param>
        public static Mesh GenMesh(ShapeType type)
        {
            switch (type) 
            { 
                case ShapeType.Cube:
                    return Raylib.GenMeshCube(1, 1, 1);
                case ShapeType.Sphere:
                    return Raylib.GenMeshSphere(0.5f, 20, 20);
                case ShapeType.Torus:
                    return Raylib.GenMeshTorus(0.5f, 1, 20, 20);
                case ShapeType.Cone:
                    return Raylib.GenMeshCone(0.5f, 1, 20);
                case ShapeType.Cylinder:
                    return Raylib.GenMeshCylinder(0.5f, 1, 20);
                case ShapeType.Knot:
                    return Raylib.GenMeshKnot(0.5f, 1, 20, 20);
                case ShapeType.HemiSphere:
                    return Raylib.GenMeshHemiSphere(0.5f, 15, 15);
            }
            return Raylib.GenMeshCube(1, 1, 1);
        }
    }
}