using Raylib_cs;
using System.Numerics;

namespace Uniray
{
    /// <summary>Reprsents an instance of <see cref="GameObject3D"/>.</summary>
    public abstract class GameObject3D
    {
        // -----------------------------------------------------------
        // Public attributes
        // -----------------------------------------------------------
        /// <summary>Transformative matrix of the object.</summary>
        public Matrix4x4 Transform;
        /// <summary>Name of the object.</summary>
        public string Name;
        /// <summary>Behvior of the object.</summary>
        public Behavior? Behavior;

        // -----------------------------------------------------------
        // Public properties
        // -----------------------------------------------------------
        /// <summary>3-Dimensional object position.</summary>
        abstract public Vector3 Position { get; set; }
        /// <summary>X Position of the object.</summary>
        public float X { get { return Transform.M14; } set { Transform.M14 = value; } }
        /// <summary>X Position of the object.</summary>
        public float Y { get { return Transform.M24; } set { Transform.M24 = value; } }
        /// <summary>X Position of the object.</summary>
        public float Z { get { return Transform.M34; } set { Transform.M34 = value; } }

        /// <summary>Creates an instance of <see cref="GameObject3D"/>.</summary>
        public GameObject3D() 
        {
            Name = "";
            Transform = Matrix4x4.Identity; // Get identity (default) matrix
        }

        /// <summary>Creates an instance of <see cref="GameObject3D"/>.</summary>
        /// <param name="name">Object name</param>
        /// <param name="position">Object position</param>
        public GameObject3D(string name, Vector3 position)
        {
            Name = name;
            Transform = Matrix4x4.Identity; // Get identity (default) matrix
            Position = position;
        }

        /// <summary>Creates an instance of <see cref="GameObject3D"/>.</summary>
        /// <param name="name">Object name</param>
        /// <param name="position">Object position</param>
        /// <param name="behaviour">Object behaviour script</param>
        public GameObject3D(string name, Vector3 position, Behavior? behavior)
        {
            Name = name;
            Transform = Matrix4x4.Identity; // Get identity (default) matrix
            Position = position;
            Behavior = behavior;
        }

        /// <summary>Returns informations about the current instance.</summary>
        /// <returns>Informations as a <see langword="string"/>.</returns>
        public override string ToString()
        {
            return "Name : " + Name + " Position : ";
        }
    }
}